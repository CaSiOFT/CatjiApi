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

//GET:api/user/followers
[HttpGet("followers")]
public async Task<IActionResult> GetVfollowers()
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }
    var user_follower = await _context.User.FindAsync(FollowerNum);
    if (user_follower = null)
    {
        return NotFound();
    }
    var followerid = await _context.Follow.FindAsync(FollowUsid);
<<<<<<< Updated upstream
    var user_followers = _context.User.where(x => x.Usid == followerid);
=======
    var user_followers = _context.User.where(x => x.Usid == followerid).Skip(offset);
>>>>>>> Stashed changes
    var result = user_followers.Select(x => new
    {
        v_Usid = x.Usid,
        v_Nickname = x.Nickname,
        v_Signature = x.Signature,
        v_Avatar = x.Avatar
    });



    return Ok(result);
}
<<<<<<< Updated upstream
=======
public async Task<IActionResult> GetVfollowings()
{
    if(!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }
    var user_followings = await _context.User.FindAsync(FollowerNum);
    if(user_followings =null)
    {
        return NotFound();
    }
    var followingid = await _context.Follow.FindAsync(Usid);
    var user_following = _context.User.where(x => x.FollowUsid == followingid).Skip(offset);
    var result = user_following.Select(x => new
    {
        v_Usid = x.Usid,
        v_Nickname = x.Nickname,
        v_Signature = x.Signature,
        v_Avatar = x.Avatar
    });
    return Ok(result);
}
>>>>>>> Stashed changes
