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
    public class VideocommentsController : ControllerBase
    {
        private readonly ModelContext _context;

        public VideocommentsController(ModelContext context)
        {
            _context = context;
        }

        // GET: api/Videocomments
        [HttpGet]
        public IEnumerable<Videocomment> GetVideocomment()
        {
            return _context.Videocomment;
        }

        // GET: api/Videocomments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVideocomment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var videocomment = await _context.Videocomment.FindAsync(id);

            if (videocomment == null)
            {
                return NotFound();
            }

            return Ok(videocomment);
        }

        // PUT: api/Videocomments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVideocomment([FromRoute] int id, [FromBody] Videocomment videocomment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != videocomment.Vcid)
            {
                return BadRequest();
            }

            _context.Entry(videocomment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VideocommentExists(id))
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

        // POST: api/Videocomments
        [HttpPost]
        public async Task<IActionResult> PostVideocomment([FromBody] Videocomment videocomment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Videocomment.Add(videocomment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVideocomment", new { id = videocomment.Vcid }, videocomment);
        }

        // DELETE: api/Videocomments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideocomment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var videocomment = await _context.Videocomment.FindAsync(id);
            if (videocomment == null)
            {
                return NotFound();
            }

            _context.Videocomment.Remove(videocomment);
            await _context.SaveChangesAsync();

            return Ok(videocomment);
        }

        private bool VideocommentExists(int id)
        {
            return _context.Videocomment.Any(e => e.Vcid == id);
        }
    }
}