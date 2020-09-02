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
        // GET: api/Likevideos/like
        [HttpGet("like")]
        public async Task<IActionResult> GetVLike()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var Lv = _context.Likevideo;

            var result = Lv.Select(x => new {
                Vid = x.Vid,

                Usid = x.Usid
            });
            return Ok(result);
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