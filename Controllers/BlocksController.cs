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
    public class BlocksController : ControllerBase
    {
        private readonly ModelContext _context;

        public BlocksController(ModelContext context)
        {
            _context = context;
        }

        // GET: api/Blocks
        [HttpGet]
        public IEnumerable<Block> GetBlock()
        {
            return _context.Block;
        }

        // GET: api/Blocks/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlock([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var block = await _context.Block.FindAsync(id);

            if (block == null)
            {
                return NotFound();
            }

            return Ok(block);
        }

        // PUT: api/Blocks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlock([FromRoute] int id, [FromBody] Block block)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != block.BlockUsid)
            {
                return BadRequest();
            }

            _context.Entry(block).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlockExists(id))
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

        // POST: api/Blocks
        [HttpPost]
        public async Task<IActionResult> PostBlock([FromBody] Block block)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Block.Add(block);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BlockExists(block.BlockUsid))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBlock", new { id = block.BlockUsid }, block);
        }

        // DELETE: api/Blocks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlock([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var block = await _context.Block.FindAsync(id);
            if (block == null)
            {
                return NotFound();
            }

            _context.Block.Remove(block);
            await _context.SaveChangesAsync();

            return Ok(block);
        }

        private bool BlockExists(int id)
        {
            return _context.Block.Any(e => e.BlockUsid == id);
        }
    }
}