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
    public class LikevideosController : ControllerBase
    {
        private readonly ModelContext _context;

        public LikevideosController(ModelContext context)
        {
            _context = context;
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetLikevideo(int id, int id2)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var likevideo = await _context.Likevideo.FindAsync(id, id2);

            if (likevideo == null)
            {
                return NotFound();
            }

            return Ok();
        }
        
        // GET: api/Likevideos
        [HttpGet]
        public IEnumerable<Likevideo> GetLikevideo()
        {
            return _context.Likevideo;
        }
        //POST:api/Likevideos/addLikeV

        [HttpPost("addLikeV")]
        public async Task<IActionResult> addLikeV(Likevideo lv)
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

            var Likevideos = _context.Likevideo.Where(x => x.Usid== user.Usid && x.Vid==lv.Vid );

            if (Likevideos.Count() != 0)
                return BadRequest(new { status = "已点赞", data = ModelState });

            var likevideo1 = new Likevideo();
            likevideo1.Usid = user.Usid;
            likevideo1.Vid = lv.Vid;
            try
            {
                _context.Likevideo.Add(likevideo1);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return NotFound(new { status = "Create failed.", data = e.ToString() });
            }

            return Ok(new { status = "ok", data = new { usid = likevideo1.Usid,vid= likevideo1.Vid } });
        }

        [HttpPost("UnlikeV")]
        public async Task<IActionResult> UnlikeV(Likevideo lv)
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

            var Likevideos = await _context.Likevideo.FirstOrDefaultAsync(x => x.Usid == user.Usid && x.Vid == lv.Vid);

            if (Likevideos == null)
                return BadRequest(new { status = "未点赞" });

            try
            {
                _context.Likevideo.Remove(Likevideos);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return NotFound(new { status = "Remove failed.", data = e.ToString() });
            }

            return Ok(new { status = "ok" });
        }

        // GET: api/Likevideos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLikevideo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var likevideo = await _context.Likevideo.FindAsync(id);

            if (likevideo == null)
            {
                return NotFound();
            }

            return Ok(likevideo);
        }

        // PUT: api/Likevideos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLikevideo([FromRoute] int id, [FromBody] Likevideo likevideo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != likevideo.Usid)
            {
                return BadRequest();
            }

            _context.Entry(likevideo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LikevideoExists(id))
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

        // POST: api/Likevideos
        [HttpPost]
        public async Task<IActionResult> PostLikevideo([FromBody] Likevideo likevideo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Likevideo.Add(likevideo);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LikevideoExists(likevideo.Usid))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLikevideo", new { id = likevideo.Usid }, likevideo);
        }

        // DELETE: api/Likevideos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLikevideo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var likevideo = await _context.Likevideo.FindAsync(id);
            if (likevideo == null)
            {
                return NotFound();
            }

            _context.Likevideo.Remove(likevideo);
            await _context.SaveChangesAsync();

            return Ok(likevideo);
        }

        private bool LikevideoExists(int id)
        {
            return _context.Likevideo.Any(e => e.Usid == id);
        }
    }
}