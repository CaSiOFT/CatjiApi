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
    public class LikeblogcommentsController : ControllerBase
    {
        private readonly ModelContext _context;

        public LikeblogcommentsController(ModelContext context)
        {
            _context = context;
        }

        // GET: api/Likeblogcomments
        [HttpGet]
        public IEnumerable<Likeblogcomment> GetLikeblogcomment()
        {
            return _context.Likeblogcomment;
        }

        // GET: api/Likeblogcomments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLikeblogcomment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var likeblogcomment = await _context.Likeblogcomment.FindAsync(id);

            if (likeblogcomment == null)
            {
                return NotFound();
            }

            return Ok(likeblogcomment);
        }
        //POST:api/Likeblogcomments/addLikeBc

        [HttpPost("addLikeBc")]
        public async Task<IActionResult> addLikeBc(Likeblogcomment Lbc)
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
            var Likeblogcomments = _context.Likeblogcomment.Where(x => x.Usid == user.Usid && x.Bcid == Lbc.Bcid);

            if (Likeblogcomments.Count() != 0)
                return BadRequest(new { status = "Already liked!" });

            var likeblogcomment0 = new Likeblogcomment();
            likeblogcomment0.Usid = user.Usid;
            likeblogcomment0.Bcid = Lbc.Bcid;
            try
            {
                _context.Likeblogcomment.Add(likeblogcomment0);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return NotFound(new { status = "Create failed.", data = e.ToString() });
            }
            return Ok(new { status = "ok", data = new { usid = likeblogcomment0.Usid, vcid = likeblogcomment0.Bcid } });
        }

        [HttpPost("UnlikeBc")]
        public async Task<IActionResult> UlikeBc(Likeblogcomment Lbc)
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
            var Likeblogcomments = await _context.Likeblogcomment.FirstOrDefaultAsync(x => x.Usid == user.Usid && x.Bcid == Lbc.Bcid);

            if (Likeblogcomments == null)
                return BadRequest(new { status = "Not already liked!" });

            try
            {
                _context.Likeblogcomment.Remove(Likeblogcomments);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return NotFound(new { status = "Remove failed.", data = e.ToString() });
            }
            return Ok(new { status = "ok" });
        }

        // PUT: api/Likeblogcomments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLikeblogcomment([FromRoute] int id, [FromBody] Likeblogcomment likeblogcomment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != likeblogcomment.Usid)
            {
                return BadRequest();
            }

            _context.Entry(likeblogcomment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LikeblogcommentExists(id))
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

        // POST: api/Likeblogcomments
        [HttpPost]
        public async Task<IActionResult> PostLikeblogcomment([FromBody] Likeblogcomment likeblogcomment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Likeblogcomment.Add(likeblogcomment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LikeblogcommentExists(likeblogcomment.Usid))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLikeblogcomment", new { id = likeblogcomment.Usid }, likeblogcomment);
        }

        // DELETE: api/Likeblogcomments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLikeblogcomment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var likeblogcomment = await _context.Likeblogcomment.FindAsync(id);
            if (likeblogcomment == null)
            {
                return NotFound();
            }

            _context.Likeblogcomment.Remove(likeblogcomment);
            await _context.SaveChangesAsync();

            return Ok(likeblogcomment);
        }

        private bool LikeblogcommentExists(int id)
        {
            return _context.Likeblogcomment.Any(e => e.Usid == id);
        }
    }
}