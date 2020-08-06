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
    public class ReportblogsController : ControllerBase
    {
        private readonly ModelContext _context;

        public ReportblogsController(ModelContext context)
        {
            _context = context;
        }

        // GET: api/Reportblogs
        [HttpGet]
        public IEnumerable<Reportblog> GetReportblog()
        {
            return _context.Reportblog;
        }

        // GET: api/Reportblogs/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReportblog([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reportblog = await _context.Reportblog.FindAsync(id);

            if (reportblog == null)
            {
                return NotFound();
            }

            return Ok(reportblog);
        }

        // PUT: api/Reportblogs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReportblog([FromRoute] int id, [FromBody] Reportblog reportblog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != reportblog.Brid)
            {
                return BadRequest();
            }

            _context.Entry(reportblog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportblogExists(id))
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

        // POST: api/Reportblogs
        [HttpPost]
        public async Task<IActionResult> PostReportblog([FromBody] Reportblog reportblog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Reportblog.Add(reportblog);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReportblog", new { id = reportblog.Brid }, reportblog);
        }

        // DELETE: api/Reportblogs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReportblog([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reportblog = await _context.Reportblog.FindAsync(id);
            if (reportblog == null)
            {
                return NotFound();
            }

            _context.Reportblog.Remove(reportblog);
            await _context.SaveChangesAsync();

            return Ok(reportblog);
        }

        private bool ReportblogExists(int id)
        {
            return _context.Reportblog.Any(e => e.Brid == id);
        }
    }
}