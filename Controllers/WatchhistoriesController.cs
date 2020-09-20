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
    public class WatchhistoriesController : ControllerBase
    {
        private readonly ModelContext _context;

        public WatchhistoriesController(ModelContext context)
        {
            _context = context;
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetWatchInfo(int usid, int offset)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var videos = _context.Watchhistory.Where(x => x.Usid == usid).OrderByDescending(x => x.CreateTime).Skip(offset).Take(10).Join(_context.Video, x => x.Vid, y => y.Vid, (x, y) => x);

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
                    vid = y.V.Vid,
                    title = y.V.Title,
                    cover = baseUrl + "images/" + y.V.Cover,
                    description = y.V.Description,
                    path = baseUrl + "videos/" + y.V.Path,
                    create_time = y.V.CreateTime.ToTimestamp(),
                    time = y.V.Time,
                    like_num = y.V.LikeNum,
                    favorite_num = y.V.FavoriteNum,
                    watch_num = y.V.WatchNum,
                    is_banned = y.V.IsBanned,
                    up = new
                    {
                        usid = y.V.Us.Usid,
                        name = y.V.Us.Nickname,
                        desc = y.V.Us.Signature,
                        follow_num = y.V.Us.FollowerNum,
                        avatar = y.V.Us.Avatar,
                        ifollow = FList.Contains(y.V.Us.Usid) ? 1 : 0
                    }
                }
            });

            return Ok(new
            {
                status = "ok",
                data = new
                {
                    count = _context.Watchhistory.Where(z => z.Usid == usid).Count(),
                    result
                }
            });
        }

        // GET: api/Watchhistories
        [HttpGet]
        public IEnumerable<Watchhistory> GetWatchhistory()
        {
            return _context.Watchhistory;
        }

        // GET: api/Watchhistories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWatchhistory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var watchhistory = await _context.Watchhistory.FindAsync(id);

            if (watchhistory == null)
            {
                return NotFound();
            }

            return Ok(watchhistory);
        }

        // PUT: api/Watchhistories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWatchhistory([FromRoute] int id, [FromBody] Watchhistory watchhistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != watchhistory.Usid)
            {
                return BadRequest();
            }

            _context.Entry(watchhistory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WatchhistoryExists(id))
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

        // POST: api/Watchhistories
        [HttpPost]
        public async Task<IActionResult> PostWatchhistory([FromBody] Watchhistory watchhistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Watchhistory.Add(watchhistory);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (WatchhistoryExists(watchhistory.Usid))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetWatchhistory", new { id = watchhistory.Usid }, watchhistory);
        }

        // DELETE: api/Watchhistories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWatchhistory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var watchhistory = await _context.Watchhistory.FindAsync(id);
            if (watchhistory == null)
            {
                return NotFound();
            }

            _context.Watchhistory.Remove(watchhistory);
            await _context.SaveChangesAsync();

            return Ok(watchhistory);
        }

        private bool WatchhistoryExists(int id)
        {
            return _context.Watchhistory.Any(e => e.Usid == id);
        }
    }
}