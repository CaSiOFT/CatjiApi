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
        // GET: api/Likevideocomments/like
        [HttpGet("like")]
        public async Task<IActionResult> GetVCLike()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var Lvc = _context.Likevideocomment;

            var result = Lvc.Select(x => new {
                Vcid = x.Vcid,

                Usid = x.Usid
            });
            return Ok(result);
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