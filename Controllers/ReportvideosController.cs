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
    public class ReportvideosController : ControllerBase
    {
        private readonly ModelContext _context;

        public ReportvideosController(ModelContext context)
        {
            _context = context;
        }

        // GET: api/Reportvideos
        [HttpGet]
        public IEnumerable<Reportvideo> GetReportvideo()
        {
            return _context.Reportvideo;
        }

        // GET: api/Reportvideos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReportvideo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reportvideo = await _context.Reportvideo.FindAsync(id);

            if (reportvideo == null)
            {
                return NotFound();
            }

            return Ok(reportvideo);
        }

        // PUT: api/Reportvideos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReportvideo([FromRoute] int id, [FromBody] Reportvideo reportvideo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != reportvideo.Vrid)
            {
                return BadRequest();
            }

            _context.Entry(reportvideo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportvideoExists(id))
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

        // POST: api/Reportvideos
        [HttpPost]
        public async Task<IActionResult> PostReportvideo([FromBody] Reportvideo reportvideo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Reportvideo.Add(reportvideo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReportvideo", new { id = reportvideo.Vrid }, reportvideo);
        }

        // DELETE: api/Reportvideos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReportvideo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reportvideo = await _context.Reportvideo.FindAsync(id);
            if (reportvideo == null)
            {
                return NotFound();
            }

            _context.Reportvideo.Remove(reportvideo);
            await _context.SaveChangesAsync();

            return Ok(reportvideo);
        }

        private bool ReportvideoExists(int id)
        {
            return _context.Reportvideo.Any(e => e.Vrid == id);
        }
    }
}