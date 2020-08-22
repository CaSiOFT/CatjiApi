using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        // GET: api/Videos/comment
        [HttpGet("comment")]
        public async Task<IActionResult> GetVideoComment(int vid, int offset)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
                create_time = x.CreateTime,
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
                    create_time = irv.CreateTime
                })
            });

            return Ok(result);
        }
        [HttpGet("comment2")]
        public async Task<IActionResult> GetVideoComment2(int vid, int offset)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
                create_time = x.CreateTime,
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
                    create_time = irv.CreateTime
                })
            });

            return Ok(result);
        }
        // GET: api/Videos/top
        [HttpGet("top")]
        public async Task<IActionResult> GetVTop()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var video_top = _context.Video.OrderByDescending(x => x.CommentNum + x.FavoriteNum * 2 + x.LikeNum + x.WatchNum).Take(10);
            //



            foreach (var vt in video_top)
            {
                vt.Us = await _context.Users.FindAsync(vt.Usid);
                //有些用户没有猫咪  没想好怎么搞
                //if(vt.Us.CatId!=null)//vt.Cat = await _context.Cat.FindAsync(vt.Us.CatId);
            }
            

            var result = video_top.Select(x => new
            {
                v_title = x.Title,
                v_user = x.Us.Nickname,
                //v_cat=x.Cat.Name
            }) ;

            return Ok(result);
        }


        // GET: api/Videos/info
        [HttpGet("info")]
        public async Task<IActionResult> GetVideoInfo(int id)
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

            var tags = _context.Videotag
                .Where(x => x.Vid == video.Vid)
                .Join(_context.Tag, x => x.TagId, y => y.TagId, (x, y) => new
                {
                    name = y.Name,
                    tag_id = x.TagId
                });

            var user = await _context.Users.FindAsync(video.Usid);

            var result = new
            {
                vid = video.Vid,
                titl = video.Title,
                desc = video.Description,
                cover = video.Cover,
                view_num = video.WatchNum,
                comment_num = video.CommentNum,
                upload_time = video.CreateTime,
                url = video.Path,
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
                    avatar = user.Avatar
                }
            };


            return Ok(result);
        }

        // GET: api/Videos
        [HttpGet]
        public IEnumerable<Video> GetVideo()
        {
            return _context.Video;
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