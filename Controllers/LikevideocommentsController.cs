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
    public class LikevideocommentsController : ControllerBase
    {
        private readonly ModelContext _context;

        public LikevideocommentsController(ModelContext context)
        {
            _context = context;
        }

        // GET: api/Likevideocomments
        [HttpGet]
        public IEnumerable<Likevideocomment> GetLikevideocomment()
        {
            return _context.Likevideocomment;
        }
       
        // GET: api/Likevideocomments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLikevideocomment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var likevideocomment = await _context.Likevideocomment.FindAsync(id);

            if (likevideocomment == null)
            {
                return NotFound();
            }

            return Ok(likevideocomment);
        }
        //POST:api/Likevideocomments/addLikeVc

        [HttpPost("addLikeVc")]
        public async Task<IActionResult> addLikeVc(Likevideocomment Lbc)
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
            var Likevideocomments = _context.Likevideocomment.Where(x => x.Usid == user.Usid && x.Vcid == Lbc.Vcid);

            if (Likevideocomments.Count() != 0)
                return BadRequest(new { status = "Already liked!" });

            var likevideocomment0 = new Likevideocomment();
            likevideocomment0.Usid = user.Usid;
            likevideocomment0.Vcid = Lbc.Vcid;
            try
            {
                _context.Likevideocomment.Add(likevideocomment0);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return NotFound(new { status = "Create failed.", data = e.ToString() });
            }
            return Ok(new { status = "ok", data = new { usid = likevideocomment0.Usid, vcid = likevideocomment0.Vcid } });
        }

        [HttpPost("UnlikeVc")]
        public async Task<IActionResult> UnikeVc(Likevideocomment Lbc)
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
            var Likevideocomments = await _context.Likevideocomment.FirstOrDefaultAsync(x => x.Usid == user.Usid && x.Vcid == Lbc.Vcid);

            if (Likevideocomments == null)
                return BadRequest(new { status = "Not already liked!" });

            try
            {
                _context.Likevideocomment.Remove(Likevideocomments);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return NotFound(new { status = "Remove failed.", data = e.ToString() });
            }
            return Ok(new { status = "ok" });
        }

        // PUT: api/Likevideocomments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLikevideocomment([FromRoute] int id, [FromBody] Likevideocomment likevideocomment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != likevideocomment.Usid)
            {
                return BadRequest();
            }

            _context.Entry(likevideocomment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LikevideocommentExists(id))
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

        // POST: api/Likevideocomments
        [HttpPost]
        public async Task<IActionResult> PostLikevideocomment([FromBody] Likevideocomment likevideocomment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Likevideocomment.Add(likevideocomment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LikevideocommentExists(likevideocomment.Usid))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLikevideocomment", new { id = likevideocomment.Usid }, likevideocomment);
        }

        // DELETE: api/Likevideocomments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLikevideocomment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var likevideocomment = await _context.Likevideocomment.FindAsync(id);
            if (likevideocomment == null)
            {
                return NotFound();
            }

            _context.Likevideocomment.Remove(likevideocomment);
            await _context.SaveChangesAsync();

            return Ok(likevideocomment);
        }

        private bool LikevideocommentExists(int id)
        {
            return _context.Likevideocomment.Any(e => e.Usid == id);
        }
    }
}