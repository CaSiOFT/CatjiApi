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
    public class CatsController : ControllerBase
    {
        private readonly ModelContext _context;

        public CatsController(ModelContext context)
        {
            _context = context;
        }

        // GET: api/Cats
        [HttpGet]
        public IEnumerable<Cat> GetCat()
        {
            return _context.Cat;
        }

        [HttpGet("videos")]
        public async Task<IActionResult> GetVideo(int cat_id, int offset)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var t = await _context.Tag.FirstOrDefaultAsync(x => x.CatId == cat_id);

            if (t == null)
                return NotFound(new { status = "该猫咪不存在对应的tag" });

            var tag_id = t.TagId;

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
                cover = y.Cover,
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

            return Ok(new { status = "ok", data = result });
        }

        [HttpGet("blogs")]
        public async Task<IActionResult> GetBlog(int cat_id, int offset)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var t = await _context.Tag.FirstOrDefaultAsync(x => x.CatId == cat_id);

            if (t == null)
                return NotFound(new { status = "该猫咪不存在对应的tag" });

            var tag_id = t.TagId;

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
                images = x.Blogimage.Select(y => y.ImgUrl),
                ilike = LikeList.Contains(x.Bid) ? 1 : 0
            });

            return Ok(new { status = "ok", data = result });
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetInfo(int cat_id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var c = await _context.Cat.FindAsync(cat_id);

            if (c == null)
                return NotFound(new { status = "猫咪不存在" });

            var result = new
            {
                cat_id = cat_id,
                name = c.Name,
                description = c.Description,
                banner = c.Banner,
                usid = c.Usid
            };

            return Ok(new { status = "ok", data = result });
        }

        // GET: api/Cats/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCat([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cat = await _context.Cat.FindAsync(id);

            if (cat == null)
            {
                return NotFound();
            }

            return Ok(cat);
        }
        // GET: api/Cats/hotlist
        [HttpGet("hotlist")]
        public async Task<IActionResult> GetCTop()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var catList = await _context.Cat.ToListAsync();

            var rndList = Tools.RandomList(10, catList.Count);

            List<Cat> cat_top = new List<Cat>();

            foreach (var v in rndList)
                cat_top.Add(catList[v]);

            var result = cat_top.Select(x => new
            {
                cat_id = x.CatId,
                banner = x.Banner,
                name = x.Name
            });

            return Ok(new { status = "ok", data = result });
        }

        // PUT: api/Cats/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCat([FromRoute] int id, [FromBody] Cat cat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cat.CatId)
            {
                return BadRequest();
            }

            _context.Entry(cat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CatExists(id))
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

        // POST: api/Cats
        [HttpPost]
        public async Task<IActionResult> PostCat([FromBody] Cat cat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Cat.Add(cat);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CatExists(cat.CatId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCat", new { id = cat.CatId }, cat);
        }

        // DELETE: api/Cats/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCat([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cat = await _context.Cat.FindAsync(id);
            if (cat == null)
            {
                return NotFound();
            }

            _context.Cat.Remove(cat);
            await _context.SaveChangesAsync();

            return Ok(cat);
        }

        private bool CatExists(int id)
        {
            return _context.Cat.Any(e => e.CatId == id);
        }
    }
}