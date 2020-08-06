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
    public class WatchhistoriesController : ControllerBase
    {
        private readonly ModelContext _context;

        public WatchhistoriesController(ModelContext context)
        {
            _context = context;
        }

        // GET: api/Watchhistories
        [HttpGet]
        public IEnumerable<Watchhistory> GetWatchhistory()
        {
            return _context.Watchhistory;
        }

        // GET: api/Watchhistories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWatchhistory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var watchhistory = await _context.Watchhistory.FindAsync(id);

            if (watchhistory == null)
            {
                return NotFound();
            }

            return Ok(watchhistory);
        }

        // PUT: api/Watchhistories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWatchhistory([FromRoute] int id, [FromBody] Watchhistory watchhistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != watchhistory.Usid)
            {
                return BadRequest();
            }

            _context.Entry(watchhistory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WatchhistoryExists(id))
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

        // POST: api/Watchhistories
        [HttpPost]
        public async Task<IActionResult> PostWatchhistory([FromBody] Watchhistory watchhistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Watchhistory.Add(watchhistory);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (WatchhistoryExists(watchhistory.Usid))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetWatchhistory", new { id = watchhistory.Usid }, watchhistory);
        }

        // DELETE: api/Watchhistories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWatchhistory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var watchhistory = await _context.Watchhistory.FindAsync(id);
            if (watchhistory == null)
            {
                return NotFound();
            }

            _context.Watchhistory.Remove(watchhistory);
            await _context.SaveChangesAsync();

            return Ok(watchhistory);
        }

        private bool WatchhistoryExists(int id)
        {
            return _context.Watchhistory.Any(e => e.Usid == id);
        }
    }
}