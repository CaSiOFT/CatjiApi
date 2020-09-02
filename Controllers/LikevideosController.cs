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
        public async Task<IActionResult> addLikeV( Likevideo Lv)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }
            
            var Likevideos = _context.Likevideo.Where(x => x.Usid==Lv.Usid && x.Vid==Lv.Vid);

            if (Likevideos.Count() != 0)
                return BadRequest();

            var likevideo1 = new Likevideo();
            likevideo1.Usid = Lv.Usid;
            likevideo1.Vid = Lv.Vid;
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