using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CatjiApi.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace CatjiApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ModelContext _context;

        public TagsController(ModelContext context)
        {
            _context = context;
        }

        [HttpGet("hotlist")]
        public async Task<IActionResult> GetTagTop()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var taglist = await _context.Tag.ToListAsync();

            var rndList = Tools.RandomList(10, taglist.Count);

            List<Tag> tag_top = new List<Tag>();

            foreach (var v in rndList)
                tag_top.Add(taglist[v]);

            var result = tag_top.Select(x => new
            {
                tag_id = x.TagId,
                cat_id = x.CatId,
                name = x.Name
            });

            return Ok(new { status = "ok", data = result });
        }

        [HttpGet("videos")]
        public async Task<IActionResult> GetVideo(int tag_id, int offset)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            bool isLogin = false;
            int myid = -1;
            List<int> LikeList = new List<int>();
            List<int> FavList = new List<int>();

            var auth = await HttpContext.AuthenticateAsync();
            if (auth.Succeeded)
            {
                var claim = User.FindFirstValue("User");
                if (int.TryParse(claim, out myid))
                    isLogin = true;
            }

            if (isLogin)
            {
                LikeList = await _context.Likevideo.Where(x => x.Usid == myid).Select(x => x.Vid).ToListAsync();
                FavList = await _context.Favorite.Where(x => x.Usid == myid).Select(x => x.Vid).ToListAsync();
            }

            string baseUrl = Request.Scheme + "://" + Request.Host + "/";

            var result = _context.Videotag.Where(x => x.TagId == tag_id).Skip(offset).Take(10).Join(_context.Video, x => x.Vid, y => y.Vid, (x, y) => new
            {
                vid = y.Vid,
                title = y.Title,
                cover = baseUrl + "images/" + y.Cover,
                description = y.Description,
                path = baseUrl + "videos/" + y.Path,
                create_time = y.CreateTime.ToTimestamp(),
                time = y.Time,
                like_num = y.LikeNum,
                favorite_num = y.FavoriteNum,
                watch_num = y.WatchNum,
                is_banned = y.IsBanned,
                ilike = LikeList.Contains(x.Vid) ? 1 : 0,
                ifavorite = FavList.Contains(x.Vid) ? 1 : 0
            });

            return Ok(new
            {
                status = "ok",
                data = new
                {
                    count = _context.Videotag.Where(x => x.TagId == tag_id).Count(),
                    result
                }
            });
        }

        [HttpGet("blogs")]
        public async Task<IActionResult> GetBlog(int tag_id, int offset)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var tag = await _context.Tag.FindAsync(tag_id);

            if (tag == null)
                return NotFound(new { status = "Tag not found!" });

            var blogs = _context.Blog.Where(x => x.Content.Contains("#" + tag.Name + "#")).Skip(offset).Take(10);

            foreach (var v in blogs)
            {
                v.Us = await _context.Users.FindAsync(v.Usid);
                v.Blogimage = await _context.Blogimage.Where(x => x.Bid == v.Bid).ToListAsync();
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
                LikeList = await _context.Likeblog.Where(x => x.Usid == myid).Select(x => x.Bid).ToListAsync();
            }

            string baseUrl = Request.Scheme + "://" + Request.Host + "/";

            var result = blogs.Select(x => new
            {
                bid = x.Bid,
                time = x.CreateTime.ToTimestamp(),
                content = x.Content,
                up = new
                {
                    usid = x.Us.Usid,
                    name = x.Us.Nickname,
                    avatar = baseUrl + "images/" + x.Us.Avatar
                },
                transmit_num = x.TransmitNum,
                comment_num = x.CommentNum,
                like_num = x.LikeNum,
                images = x.Blogimage.Select(y => baseUrl + "images/" + y.ImgUrl),
                ilike = LikeList.Contains(x.Bid) ? 1 : 0
            });

            return Ok(new { status = "ok", data = result });
        }

        // GET: api/Tags/name
        [HttpGet("name")]
        public async Task<IActionResult> GetTagName(int tag_id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var tag = await _context.Tag.FindAsync(tag_id);

            if (tag == null)
            {
                return NotFound(new { status = "Not found!" });
            }

            return Ok(new { status = "ok", data = new { name = tag.Name } });
        }

        // GET: api/Tags
        [HttpGet]
        public IEnumerable<Tag> GetTag()
        {
            return _context.Tag;
        }

        // GET: api/Tags/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTag([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tag = await _context.Tag.FindAsync(id);

            if (tag == null)
            {
                return NotFound();
            }

            return Ok(tag);
        }
        //Get:api/tags/search
        [HttpGet("search")]
        public async Task<IActionResult> Gettagsearch(int offset, string keyword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var keys = _context.Tag.Where(x => x.Name.Contains(keyword)).Skip(offset).Take(10);

            var result = keys.Select(x => new
            {
                tag_id = x.TagId,
                name = x.Name,
                catid = x.CatId
            });

            bool isLogin = false;
            int myid = -1;

            var auth = await HttpContext.AuthenticateAsync();
            if (auth.Succeeded)
            {
                var claim = User.FindFirstValue("User");
                if (int.TryParse(claim, out myid))
                    isLogin = true;
            }

            if (isLogin)
            {
                try
                {
                    var v = await _context.Searchhistory.FirstOrDefaultAsync(x => x.Usid == myid && x.Content == keyword);
                    if (v != null)
                        _context.Searchhistory.Remove(v);
                    v = new Searchhistory();
                    v.CreateTime = DateTime.Now;
                    v.Content = keyword;
                    v.Usid = myid;
                    _context.Searchhistory.Add(v);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    return Ok(new { status = "Create history failed!", data = result });
                }
            }

            return Ok(new { status = "ok", data = result });
        }

        // PUT: api/Tags/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTag([FromRoute] int id, [FromBody] Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tag.TagId)
            {
                return BadRequest();
            }

            _context.Entry(tag).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TagExists(id))
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

        // POST: api/Tags
        [HttpPost]
        public async Task<IActionResult> PostTag([FromBody] Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Tag.Add(tag);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTag", new { id = tag.TagId }, tag);
        }

        // DELETE: api/Tags/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tag = await _context.Tag.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            _context.Tag.Remove(tag);
            await _context.SaveChangesAsync();

            return Ok(tag);
        }

        private bool TagExists(int id)
        {
            return _context.Tag.Any(e => e.TagId == id);
        }
    }
}