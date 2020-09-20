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
    public class FavoritesController : ControllerBase
    {
        private readonly ModelContext _context;

        public FavoritesController(ModelContext context)
        {
            _context = context;
        }

        [HttpPost("addFav")]
        public async Task<IActionResult> addFav(Favorite lv)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var auth = await HttpContext.AuthenticateAsync();
            if (!auth.Succeeded)
            {
                return NotFound(new { status = "not login" });
            }

            var claim = User.FindFirstValue("User");

            if (!Int32.TryParse(claim, out var loginUsid))
            {
                return BadRequest(new { status = "validation failed" });
            }

            var user = await _context.Users.FindAsync(loginUsid);

            var Fav = await _context.Favorite.FirstOrDefaultAsync(x => x.Usid == user.Usid && x.Vid == lv.Vid);

            if (Fav != null)
                return BadRequest(new { status = "已收藏", data = ModelState });

            Fav = new Favorite();
            Fav.Usid = user.Usid;
            Fav.Vid = lv.Vid;
            try
            {
                await _context.Favorite.AddAsync(Fav);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return NotFound(new { status = "Create failed.", data = e.ToString() });
            }

            return Ok(new { status = "ok" });
        }

        [HttpPost("UnFav")]
        public async Task<IActionResult> UnFav(Favorite lv)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var auth = await HttpContext.AuthenticateAsync();
            if (!auth.Succeeded)
            {
                return NotFound(new { status = "not login" });
            }

            var claim = User.FindFirstValue("User");

            if (!Int32.TryParse(claim, out var loginUsid))
            {
                return BadRequest(new { status = "validation failed" });
            }

            var user = await _context.Users.FindAsync(loginUsid);

            var Fav = await _context.Favorite.FirstOrDefaultAsync(x => x.Usid == user.Usid && x.Vid == lv.Vid);

            if (Fav == null)
                return BadRequest(new { status = "未收藏", data = ModelState });

            try
            {
                _context.Favorite.Remove(Fav);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return NotFound(new { status = "Remove failed.", data = e.ToString() });
            }

            return Ok(new { status = "ok" });
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetFavoriteInfo(int usid, int offset)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var videos = _context.Favorite.Where(x => x.Usid == usid).Skip(offset).Take(10).Join(_context.Video, x => x.Vid, y => y.Vid, (x, y) => y);

            string baseUrl = Request.Scheme + "://" + Request.Host + "/";

            bool isLogin = false;
            int myid = -1;
            List<int> FList = new List<int>();

            var auth = await HttpContext.AuthenticateAsync();
            if (auth.Succeeded)
            {
                var claim = User.FindFirstValue("User");
                if (int.TryParse(claim, out myid))
                    isLogin = true;
            }

            if (isLogin)
            {
                FList = await _context.Follow.Where(x => x.Usid == myid).Select(x => x.FollowUsid).ToListAsync();
            }

            var result = _context.Users.Join(videos, x => x.Usid, y => y.Usid, (x, y) => new
            {
                watch_time = y.CreateTime.ToTimestamp(),
                video = new
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
                    up = new
                    {
                        usid = y.Us.Usid,
                        name = y.Us.Nickname,
                        desc = y.Us.Signature,
                        follow_num = y.Us.FollowerNum,
                        avatar = y.Us.Avatar,
                        ifollow = FList.Contains(y.Us.Usid) ? 1 : 0
                    }
                },
            });

            return Ok(new
            {
                status = "ok",
                data = new
                {
                    count = _context.Favorite.Where(z => z.Usid == usid).Count(),
                    result
                }
            });
        }

        // GET: api/Favorites
        [HttpGet]
        public IEnumerable<Favorite> GetFavorite()
        {
            return _context.Favorite;
        }

        // GET: api/Favorites/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFavorite([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var favorite = await _context.Favorite.FindAsync(id);

            if (favorite == null)
            {
                return NotFound();
            }

            return Ok(favorite);
        }

        // PUT: api/Favorites/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFavorite([FromRoute] int id, [FromBody] Favorite favorite)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != favorite.Usid)
            {
                return BadRequest();
            }

            _context.Entry(favorite).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FavoriteExists(id))
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

        // POST: api/Favorites
        [HttpPost]
        public async Task<IActionResult> PostFavorite([FromBody] Favorite favorite)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Favorite.Add(favorite);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FavoriteExists(favorite.Usid))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFavorite", new { id = favorite.Usid }, favorite);
        }

        // DELETE: api/Favorites/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFavorite([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var favorite = await _context.Favorite.FindAsync(id);
            if (favorite == null)
            {
                return NotFound();
            }

            _context.Favorite.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok(favorite);
        }

        private bool FavoriteExists(int id)
        {
            return _context.Favorite.Any(e => e.Usid == id);
        }
    }
}