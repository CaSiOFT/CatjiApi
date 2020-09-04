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
    public class FollowsController : ControllerBase
    {
        private readonly ModelContext _context;

        public FollowsController(ModelContext context)
        {
            _context = context;
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

            var result = followers.Select(x => new
            {
                usid = x.Usid,
                nickname = x.Us.Nickname,
                signature = x.Us.Signature,
                avatar = x.Us.Avatar,
                gender = x.Us.Gender
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

            var result = followings.Select(x => new
            {
                usid = x.FollowUsid,
                nickname = x.FollowUs.Nickname,
                signature = x.FollowUs.Signature,
                avatar = x.FollowUs.Avatar,
                gender = x.FollowUs.Gender
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