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
    public class BlogtagsController : ControllerBase
    {
        private readonly ModelContext _context;

        public BlogtagsController(ModelContext context)
        {
            _context = context;
        }

        // GET: api/Blogtags
        [HttpGet]
        public IEnumerable<Blogtag> GetBlogtag()
        {
            return _context.Blogtag;
        }

        // GET: api/Blogtags/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogtag([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var blogtag = await _context.Blogtag.FindAsync(id);

            if (blogtag == null)
            {
                return NotFound();
            }

            return Ok(blogtag);
        }

        // PUT: api/Blogtags/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlogtag([FromRoute] int id, [FromBody] Blogtag blogtag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != blogtag.Bid)
            {
                return BadRequest();
            }

            _context.Entry(blogtag).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogtagExists(id))
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

        // POST: api/Blogtags
        [HttpPost]
        public async Task<IActionResult> PostBlogtag([FromBody] Blogtag blogtag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Blogtag.Add(blogtag);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BlogtagExists(blogtag.Bid))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBlogtag", new { id = blogtag.Bid }, blogtag);
        }

        // DELETE: api/Blogtags/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogtag([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var blogtag = await _context.Blogtag.FindAsync(id);
            if (blogtag == null)
            {
                return NotFound();
            }

            _context.Blogtag.Remove(blogtag);
            await _context.SaveChangesAsync();

            return Ok(blogtag);
        }

        private bool BlogtagExists(int id)
        {
            return _context.Blogtag.Any(e => e.Bid == id);
        }
    }
}