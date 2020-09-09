﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using CatjiApi.Models;

namespace CatjiApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly ModelContext _context;

        public VideosController(ModelContext context)
        {
            _context = context;
        }

        [HttpPost("release"), DisableRequestSizeLimit]
        public async Task<IActionResult> Upload(IFormCollection paras)
        {
            var auth = await HttpContext.AuthenticateAsync();
            if (!auth.Succeeded)
            {
                return BadRequest(new { status = "not login" });
            }

            var claim = User.FindFirstValue("User");
            int usid;

            if (!Int32.TryParse(claim, out usid))
            {
                return BadRequest(new { status = "validation failed" });
            }

            string pathToSave;
            string coverFileName = Guid.NewGuid().ToString() + '.' + paras.Files["cover"].FileName.Split('.').Last();
            string videoFileName = Guid.NewGuid().ToString() + '.' + paras.Files["video"].FileName.Split('.').Last();

            pathToSave = "wwwroot/images" + "/" + coverFileName;
            using (var stream = System.IO.File.Create(pathToSave))
            {
                await paras.Files["cover"].CopyToAsync(stream);
            }

            pathToSave = "wwwroot/videos" + "/" + videoFileName;
            using (var stream = System.IO.File.Create(pathToSave))
            {
                await paras.Files["video"].CopyToAsync(stream);
            }

            var videoPO = new Video();
            videoPO.Usid = usid;
            videoPO.Cover = coverFileName;
            videoPO.Path = videoFileName;
            videoPO.Title = paras["title"];
            videoPO.Description = paras["desc"];
            videoPO.CreateTime = DateTime.Now;

            try
            {
                using (System.Diagnostics.Process pro = new System.Diagnostics.Process())
                {
                    pro.StartInfo.UseShellExecute = false;
                    pro.StartInfo.ErrorDialog = false;
                    pro.StartInfo.RedirectStandardError = true;

                    pro.StartInfo.FileName = "../ffmpeg/tools/ffmpeg/bin/ffmpeg.exe";
                    pro.StartInfo.Arguments = " -i " + pathToSave;

                    pro.Start();
                    System.IO.StreamReader errorreader = pro.StandardError;
                    pro.WaitForExit(1000);

                    string result = errorreader.ReadToEnd();
                    if (!string.IsNullOrEmpty(result))
                    {
                        result = result.Substring(result.IndexOf("Duration: ") + ("Duration: ").Length, ("00:00:00").Length);
                        var v = result.Split(':');
                        int t = Convert.ToInt32(v[0]) * 3600 + Convert.ToInt32(v[1]) * 60 + Convert.ToInt32(v[2]);
                        videoPO.Time = t;
                    }
                }
            }
            catch { }

            try
            {
                await _context.Video.AddAsync(videoPO);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    status = "Create failed.",
                    data = e.ToString()
                });
            }

            try
            {
                foreach (var v in paras["tags"])
                {
                    var tag = await _context.Tag.FirstOrDefaultAsync(x => x.Name == v);
                    if (tag == null)
                    {
                        tag = new Tag();
                        tag.Name = v;
                        await _context.Tag.AddAsync(tag);
                    }
                    var vt = new Videotag();
                    vt.TagId = tag.TagId;
                    vt.Vid = videoPO.Vid;
                    await _context.Videotag.AddAsync(vt);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    status = "Create tag failed.",
                    data = e.ToString()
                });
            }

            return Ok(new { status = "ok" });
        }

        [HttpGet("own")]
        public async Task<IActionResult> GetVideoOwn(int usid, int offset)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "validation failed" });
            }

            var result = _context.Video.Where(x => x.Usid == usid).OrderByDescending(x => x.CreateTime).Skip(offset).Take(10).Select(x => new
            {
                vid = x.Vid,
                title = x.Title,
                cover = x.Cover,
                description = x.Description,
                path = x.Path,
                create_time = x.CreateTime.ToTimestamp(),
                time = x.Time,
                like_num = x.LikeNum,
                favorite_num = x.FavoriteNum,
                watch_num = x.WatchNum,
                is_banned = x.IsBanned
            });

            return Ok(new { status = "ok", data = result });
        }

        // 
        [HttpGet("test")]
        public async Task<IActionResult> Test(int vid)
        {
            var video = await _context.Video.FindAsync(vid);
            try
            {
                video.Description = "Test";
                await _context.SaveChangesAsync();
            }
            catch
            {
                return NotFound("Failed");
            }
            return Ok();
        }

        // GET: /api/Videos/comments
        [HttpGet("comments")]
        public async Task<IActionResult> GetVideoComment(int vid, int offset)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "validation failed" });
            }

            var comments = _context.Videocomment.Where(vc => vc.Vid == vid).OrderBy(x => x.CreateTime).Skip(offset).Take(10);

            foreach (var comment in comments)
            {
                comment.InverseParentVc = await _context.Videocomment.Where(vc => comment.Vcid == vc.ParentVcid).ToListAsync();
                foreach (var irv in comment.InverseParentVc)
                {
                    irv.Us = await _context.Users.FindAsync(irv.Usid);
                }
            }

            foreach (var comment in comments)
            {
                comment.Us = await _context.Users.FindAsync(comment.Usid);
            }

            bool isLogin = false;
            int myid = -1;
            List<int> LikeList = new List<int>();

            var auth = await HttpContext.AuthenticateAsync();
            if (auth.Succeeded)
            {
                var claim = User.FindFirstValue("User");
                if (int.TryParse(claim, out myid))
                    isLogin = true;
            }

            if (isLogin)
            {
                LikeList = await _context.Likevideocomment.Where(x => x.Usid == myid).Select(x => x.Vcid).ToListAsync();
            }

            var result = comments.Select(x => new
            {
                vcid = x.Vcid,
                content = x.Content,
                user = new
                {
                    usid = x.Us.Usid,
                    name = x.Us.Nickname,
                    avatar = x.Us.Avatar
                },
                like_num = x.LikeNum,
                create_time = x.CreateTime.ToTimestamp(),
                ilike = LikeList.Contains(x.Vcid) ? 1 : 0,
                replys = x.InverseParentVc.Select(irv => new
                {
                    vcid = irv.Vcid,
                    content = irv.Content,
                    user = new
                    {
                        usid = irv.Us.Usid,
                        name = irv.Us.Nickname,
                        avatar = irv.Us.Avatar
                    },
                    like_num = irv.LikeNum,
                    create_time = irv.CreateTime.ToTimestamp(),
                    ilike = LikeList.Contains(irv.Vcid) ? 1 : 0,
                })
            });

            return Ok(new { status = "ok", data = result });
        }

        // GET: api/Videos/hotlist
        [HttpGet("hotlist")]
        public async Task<IActionResult> GetVTop()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "validation failed" });
            }

            var video_top = _context.Video.OrderByDescending(x => x.CommentNum + x.FavoriteNum * 2 + x.LikeNum + x.WatchNum).Take(10);

            foreach (var vt in video_top)
            {
                vt.Us = await _context.Users.FindAsync(vt.Usid);
            }

            string baseUrl = Request.Scheme + "://" + Request.Host + "/";

            var result = video_top.Select(x => new
            {
                vid = x.Vid,
                name = x.Title,
                up = new
                {
                    usid = x.Us.Usid,
                    name = x.Us.Nickname,
                    avatar = baseUrl + "images/" + x.Us.Avatar
                },
                cover = baseUrl + "images/" + x.Cover
            });

            return Ok(new { status = "ok", data = result });
        }

        // GET: api/Videos/hotlist
        [HttpGet("newlist")]
        public async Task<IActionResult> GetVNew()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "validation failed" });
            }

            var video_top = _context.Video.OrderByDescending(x => x.CreateTime).Take(10);

            foreach (var vt in video_top)
            {
                vt.Us = await _context.Users.FindAsync(vt.Usid);
            }

            string baseUrl = Request.Scheme + "://" + Request.Host + "/";

            var result = video_top.Select(x => new
            {
                vid = x.Vid,
                name = x.Title,
                up = new
                {
                    usid = x.Us.Usid,
                    name = x.Us.Nickname,
                    avatar = baseUrl + "images/" + x.Us.Avatar
                },
                cover = baseUrl + "images/" + x.Cover
            });

            return Ok(new { status = "ok", data = result });
        }

        // GET: /api/Videos/info 查询视频基本信息
        [HttpGet("info")]
        public async Task<IActionResult> GetVideoInfo(int vid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "error", data = ModelState.ToString() });
            }

            var video = await _context.Video.FindAsync(vid);

            if (video == null)
            {
                return NotFound(new { status = "not found" });
            }

            var tags = _context.Videotag
                .Where(x => x.Vid == video.Vid)
                .Join(_context.Tag, x => x.TagId, y => y.TagId, (x, y) => new
                {
                    name = y.Name,
                    tag_id = x.TagId,
                    cat_id = y.CatId
                });

            var user = await _context.Users.FindAsync(video.Usid);

            string baseUrl = Request.Scheme + "://" + Request.Host + "/";

            bool isLogin = false;
            int myid = -1;
            int ifavorite = 0;
            int ilike = 0;
            int ifollow = 0;

            var auth = await HttpContext.AuthenticateAsync();
            if (auth.Succeeded)
            {
                var claim = User.FindFirstValue("User");
                if (int.TryParse(claim, out myid))
                    isLogin = true;
            }

            if (isLogin)
            {
                ilike = (await _context.Likevideo.FindAsync(myid, vid)) == null ? 0 : 1;
                ifavorite = (await _context.Favorite.FindAsync(myid, vid)) == null ? 0 : 1;
                ifollow = (await _context.Follow.FindAsync(myid, user.Usid)) == null ? 0 : 1;
            }

            var result = new
            {
                vid = video.Vid,
                title = video.Title,
                desc = video.Description,
                cover = baseUrl + "images/" + video.Cover,
                view_num = video.WatchNum,
                comment_num = video.CommentNum,
                upload_time = video.CreateTime.ToTimestamp(),
                url = baseUrl + "videos/" + video.Path,
                like_num = video.LikeNum,
                favorite_num = video.FavoriteNum,
                share_num = video.FavoriteNum,
                tags = tags,
                up = new
                {
                    usid = user.Usid,
                    name = user.Nickname,
                    desc = user.Signature,
                    follow_num = user.FollowerNum,
                    avatar = user.Avatar,
                    ifollow
                },
                ilike,
                ifavorite
            };

            return Ok(new { status = "ok", data = result });
        }

        //Get:api/videos/search
        [HttpGet("search")]
        public async Task<IActionResult> Getvideosearch(int offset, string keyword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var keys = _context.Video.Where(x => x.Description.Contains(keyword) || x.Title.Contains(keyword)).Skip(offset).Take(10);

            var result = keys.Select(x => new
            {
                vid = x.Vid,
                title = x.Title,
                desc = x.Description,
                cover = x.Cover,
                view_num = x.WatchNum,
                comment_num = x.IsBanned,
                upload_time = x.CreateTime.ToTimestamp(),
                url = x.Path,
                like_num = x.LikeNum,
                favorite_num = x.FavoriteNum,
                share_num = 0
            });

            return Ok(new { status = "ok", data = result });
        }

        // GET: api/Videos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVideo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var video = await _context.Video.FindAsync(id);

            if (video == null)
            {
                return NotFound();
            }

            return Ok(video);
        }

        // PUT: api/Videos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVideo([FromRoute] int id, [FromBody] Video video)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != video.Vid)
            {
                return BadRequest();
            }

            _context.Entry(video).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VideoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Videos
        [HttpPost]
        public async Task<IActionResult> PostVideo([FromBody] Video video)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Video.Add(video);
            await _context.SaveChangesAsync();

            CreatedAtAction("GetVideo", new { id = video.Vid }, video);
            return Ok(video.Vid);
        }

        // DELETE: api/Videos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var video = await _context.Video.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            _context.Video.Remove(video);
            await _context.SaveChangesAsync();

            return Ok(video);
        }

        private bool VideoExists(int id)
        {
            return _context.Video.Any(e => e.Vid == id);
        }
    }
}