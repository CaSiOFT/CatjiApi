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
    public class FollowsController : ControllerBase
    {
        private readonly ModelContext _context;

        public FollowsController(ModelContext context)
        {
            _context = context;
        }

        public class USID
        {
            public int usid;
        }

        [HttpPost("follow")]
        public async Task<IActionResult> FollowSO(USID FU)
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

            if (user == null)
            {
                return BadRequest(new { status = "No user!" });
            }

            var usid = user.Usid;
            var fusid = FU.usid;

            user = await _context.Users.FindAsync(fusid);

            if (user == null)
            {
                return BadRequest(new { status = "关注的人不存在" });
            }

            var FO = await _context.Follow.FindAsync(usid, fusid);

            if (FO != null)
                return BadRequest(new { status = "已经关注此人" });

            FO = new Follow();
            FO.Usid = usid;
            FO.FollowUsid = fusid;

            try
            {
                await _context.Follow.AddAsync(FO);
                await _context.SaveChangesAsync();
            }
            catch
            {
                return BadRequest(new { status = "关注失败" });
            }

            return Ok(new { status = "ok" });
        }

        [HttpPost("unfollow")]
        public async Task<IActionResult> UnFollowSO(USID FU)
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

            if (user == null)
            {
                return BadRequest(new { status = "No user!" });
            }

            var usid = user.Usid;
            var fusid = FU.usid;

            user = await _context.Users.FindAsync(fusid);

            if (user == null)
            {
                return BadRequest(new { status = "关注的人不存在" });
            }

            var FO = await _context.Follow.FindAsync(usid, fusid);

            if (FO == null)
                return BadRequest(new { status = "未关注此人" });

            try
            {
                _context.Follow.Remove(FO);
                await _context.SaveChangesAsync();
            }
            catch
            {
                return BadRequest(new { status = "取消关注失败" });
            }

            return Ok(new { status = "ok" });
        }

        //GET:api/follows/followers
        [HttpGet("followers")]
        public async Task<IActionResult> Getfollowers(int offset, int Usid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var followers = _context.Follow.Where(x => x.FollowUsid == Usid).Skip(offset).Take(20);

            foreach (var follower in followers)
            {
                follower.Us = await _context.Users.FindAsync(follower.Usid);
            }

            bool isLogin = false;
            int myid = -1;
            List<int> FollowList = new List<int>();
            List<int> BlockList = new List<int>();

            var auth = await HttpContext.AuthenticateAsync();
            if (auth.Succeeded)
            {
                var claim = User.FindFirstValue("User");
                if (int.TryParse(claim, out myid))
                    isLogin = true;
            }

            if (isLogin)
            {
                FollowList = await _context.Follow.Where(x => x.Usid == myid).Select(x => x.FollowUsid).ToListAsync();
                BlockList = await _context.Block.Where(x => x.Usid == myid).Select(x => x.BlockUsid).ToListAsync();
            }

            string baseUrl = Request.Scheme + "://" + Request.Host + "/";

            var result = followers.Select(x => new
            {
                usid = x.Usid,
                nickname = x.Us.Nickname,
                signature = x.Us.Signature,
                avatar = baseUrl + "images/" + x.Us.Avatar,
                gender = x.Us.Gender,
                ifollow = FollowList.Contains(x.Usid) ? 1 : 0,
                iblock = BlockList.Contains(x.Usid) ? 1 : 0
            });

            return Ok(new { status = "ok", data = result });
        }

        [HttpGet("following")]
        public async Task<IActionResult> Getfollowings(int offset, int Usid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var followings = _context.Follow.Where(x => x.Usid == Usid).Skip(offset).Take(20);

            foreach (var following in followings)
            {
                following.FollowUs = await _context.Users.FindAsync(following.FollowUsid);
            }

            bool isLogin = false;
            int myid = -1;
            List<int> FollowList = new List<int>();
            List<int> BlockList = new List<int>();

            var auth = await HttpContext.AuthenticateAsync();
            if (auth.Succeeded)
            {
                var claim = User.FindFirstValue("User");
                if (int.TryParse(claim, out myid))
                    isLogin = true;
            }

            if (isLogin)
            {
                FollowList = await _context.Follow.Where(x => x.Usid == myid).Select(x => x.FollowUsid).ToListAsync();
                BlockList = await _context.Block.Where(x => x.Usid == myid).Select(x => x.BlockUsid).ToListAsync();
            }

            string baseUrl = Request.Scheme + "://" + Request.Host + "/";

            var result = followings.Select(x => new
            {
                usid = x.FollowUsid,
                nickname = x.FollowUs.Nickname,
                signature = x.FollowUs.Signature,
                avatar = baseUrl + "images/" + x.FollowUs.Avatar,
                gender = x.FollowUs.Gender,
                ifollow = FollowList.Contains(x.FollowUsid) ? 1 : 0,
                iblock = BlockList.Contains(x.FollowUsid) ? 1 : 0
            });

            return Ok(new { status = "ok", data = result });
        }

        // GET: api/Follows
        [HttpGet]
        public IEnumerable<Follow> GetFollow()
        {
            return _context.Follow;
        }

        // GET: api/Follows/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFollow([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var follow = await _context.Follow.FindAsync(id);

            if (follow == null)
            {
                return NotFound();
            }

            return Ok(follow);
        }

        // PUT: api/Follows/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFollow([FromRoute] int id, [FromBody] Follow follow)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != follow.Usid)
            {
                return BadRequest();
            }

            _context.Entry(follow).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FollowExists(id))
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

        // POST: api/Follows
        [HttpPost]
        public async Task<IActionResult> PostFollow([FromBody] Follow follow)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Follow.Add(follow);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FollowExists(follow.Usid))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFollow", new { id = follow.Usid }, follow);
        }

        // DELETE: api/Follows/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFollow([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var follow = await _context.Follow.FindAsync(id);
            if (follow == null)
            {
                return NotFound();
            }

            _context.Follow.Remove(follow);
            await _context.SaveChangesAsync();

            return Ok(follow);
        }


        private bool FollowExists(int id)
        {
            return _context.Follow.Any(e => e.Usid == id);
        }
    }
}