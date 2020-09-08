using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CatjiApi.Models;
using System.Web;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace CatjiApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeblogsController : ControllerBase
    {
        private readonly ModelContext _context;

        public LikeblogsController(ModelContext context)
        {
            _context = context;
        }

        // GET: api/Likeblogs
        [HttpGet]
        public IEnumerable<Likeblog> GetLikeblog()
        {
            return _context.Likeblog;
        }
        //POST:api/Likeblogs/addLikeB

        [HttpPost("addLikeB")]
        public async Task<IActionResult> addLikeB(Likeblog Lb)
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
            var Likeblogs = _context.Likeblog.Where(x => x.Usid == user.Usid && x.Bid == Lb.Bid);

            if (Likeblogs.Count() != 0)
                return BadRequest(new { status = "Already liked!" });

            var likeblog0 = new Likeblog();
            likeblog0.Usid = user.Usid;
            likeblog0.Bid = Lb.Bid;
            try
            {
                _context.Likeblog.Add(likeblog0);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return NotFound(new { status = "Create failed.", data = e.ToString() });
            }
            return Ok(new { status = "ok", data = new { usid = likeblog0.Usid, bid = likeblog0.Bid} });
        }

        [HttpPost("UnlikeB")]
        public async Task<IActionResult> UnlikeB(Likeblog Lb)
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
            var Likeblogs = await _context.Likeblog.FirstOrDefaultAsync(x => x.Usid == user.Usid && x.Bid == Lb.Bid);

            if (Likeblogs == null)
                return BadRequest(new { status = "Not already liked!" });

            try
            {
                _context.Likeblog.Remove(Likeblogs);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return NotFound(new { status = "Remove failed.", data = e.ToString() });
            }
            return Ok(new { status = "ok" });
        }

        // GET: api/Likeblogs/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLikeblog([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var likeblog = await _context.Likeblog.FindAsync(id);

            if (likeblog == null)
            {
                return NotFound();
            }

            return Ok(likeblog);
        }

        // PUT: api/Likeblogs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLikeblog([FromRoute] int id, [FromBody] Likeblog likeblog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != likeblog.Usid)
            {
                return BadRequest();
            }

            _context.Entry(likeblog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LikeblogExists(id))
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

        // POST: api/Likeblogs
        [HttpPost]
        public async Task<IActionResult> PostLikeblog([FromBody] Likeblog likeblog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Likeblog.Add(likeblog);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LikeblogExists(likeblog.Usid))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLikeblog", new { id = likeblog.Usid }, likeblog);
        }
        
        // DELETE: api/Likeblogs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLikeblog([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var likeblog = await _context.Likeblog.FindAsync(id);
            if (likeblog == null)
            {
                return NotFound();
            }

            _context.Likeblog.Remove(likeblog);
            await _context.SaveChangesAsync();

            return Ok(likeblog);
        }

        private bool LikeblogExists(int id)
        {
            return _context.Likeblog.Any(e => e.Usid == id);
        }
    }
}