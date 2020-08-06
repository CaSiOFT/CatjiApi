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
    public class VideotagsController : ControllerBase
    {
        private readonly ModelContext _context;

        public VideotagsController(ModelContext context)
        {
            _context = context;
        }

        // GET: api/Videotags
        [HttpGet]
        public IEnumerable<Videotag> GetVideotag()
        {
            return _context.Videotag;
        }

        // GET: api/Videotags/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVideotag([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var videotag = await _context.Videotag.FindAsync(id);

            if (videotag == null)
            {
                return NotFound();
            }

            return Ok(videotag);
        }

        // PUT: api/Videotags/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVideotag([FromRoute] int id, [FromBody] Videotag videotag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != videotag.Vid)
            {
                return BadRequest();
            }

            _context.Entry(videotag).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VideotagExists(id))
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

        // POST: api/Videotags
        [HttpPost]
        public async Task<IActionResult> PostVideotag([FromBody] Videotag videotag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Videotag.Add(videotag);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (VideotagExists(videotag.Vid))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetVideotag", new { id = videotag.Vid }, videotag);
        }

        // DELETE: api/Videotags/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideotag([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var videotag = await _context.Videotag.FindAsync(id);
            if (videotag == null)
            {
                return NotFound();
            }

            _context.Videotag.Remove(videotag);
            await _context.SaveChangesAsync();

            return Ok(videotag);
        }

        private bool VideotagExists(int id)
        {
            return _context.Videotag.Any(e => e.Vid == id);
        }
    }
}