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
    public class CatsController : ControllerBase
    {
        private readonly ModelContext _context;

        public CatsController(ModelContext context)
        {
            _context = context;
        }

        // GET: api/Cats
        [HttpGet]
        public IEnumerable<Cat> GetCat()
        {
            return _context.Cat;
        }

        // GET: api/Cats/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCat([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cat = await _context.Cat.FindAsync(id);

            if (cat == null)
            {
                return NotFound();
            }

            return Ok(cat);
        }
        // GET: api/Cats/top
        [HttpGet("top")]
        public async Task<IActionResult> GetCTop()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cat_top = _context.Cat.Take(10);

            var result = cat_top.Select(x => new
            {
                c_id=x.CatId,
                c_p=x.Banner,
                c_t=x.Name
            });

            return Ok(result);
        }
        // PUT: api/Cats/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCat([FromRoute] int id, [FromBody] Cat cat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cat.CatId)
            {
                return BadRequest();
            }

            _context.Entry(cat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CatExists(id))
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

        // POST: api/Cats
        [HttpPost]
        public async Task<IActionResult> PostCat([FromBody] Cat cat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Cat.Add(cat);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CatExists(cat.CatId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCat", new { id = cat.CatId }, cat);
        }

        // DELETE: api/Cats/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCat([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cat = await _context.Cat.FindAsync(id);
            if (cat == null)
            {
                return NotFound();
            }

            _context.Cat.Remove(cat);
            await _context.SaveChangesAsync();

            return Ok(cat);
        }

        private bool CatExists(int id)
        {
            return _context.Cat.Any(e => e.CatId == id);
        }
    }
}