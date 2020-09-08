using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CatjiApi.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

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

        public class USID
        {
            public int usid;
        }

        [HttpPost("block")]
        public async Task<IActionResult> BlockSO(USID FU)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }
            var auth = await HttpContext.AuthenticateAsync();
            if (!auth.Succeeded)
            {
                return NotFound(new { status = "not login" });
            }

            var claim = User.FindFirstValue("User");

            if (!Int32.TryParse(claim, out var loginUsid))
            {
                return BadRequest(new { status = "validation failed" });
            }

            var user = await _context.Users.FindAsync(loginUsid);

            if (user == null)
            {
                return BadRequest(new { status = "No user!" });
            }

            var usid = user.Usid;
            var fusid = FU.usid;

            user = await _context.Users.FindAsync(fusid);

            if (user == null)
            {
                return BadRequest(new { status = "拉黑的人不存在" });
            }

            var FO = await _context.Block.FindAsync(fusid, usid);

            if (FO != null)
                return BadRequest(new { status = "已经拉黑此人" });

            FO = new Block();
            FO.Usid = usid;
            FO.BlockUsid = fusid;

            try
            {
                await _context.Block.AddAsync(FO);

                var FOLL = await _context.Follow.FindAsync(usid, fusid);

                if (FOLL != null)
                    _context.Follow.Remove(FOLL);

                await _context.SaveChangesAsync();
            }
            catch
            {
                return BadRequest(new { status = "拉黑失败" });
            }

            return Ok(new { status = "ok" });
        }

        [HttpPost("unblock")]
        public async Task<IActionResult> UnblockSO(USID FU)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }
            var auth = await HttpContext.AuthenticateAsync();
            if (!auth.Succeeded)
            {
                return NotFound(new { status = "not login" });
            }

            var claim = User.FindFirstValue("User");

            if (!Int32.TryParse(claim, out var loginUsid))
            {
                return BadRequest(new { status = "validation failed" });
            }

            var user = await _context.Users.FindAsync(loginUsid);

            if (user == null)
            {
                return BadRequest(new { status = "No user!" });
            }

            var usid = user.Usid;
            var fusid = FU.usid;

            user = await _context.Users.FindAsync(fusid);

            if (user == null)
            {
                return BadRequest(new { status = "拉黑的人不存在" });
            }

            var FO = await _context.Block.FindAsync(fusid, usid);

            if (FO == null)
                return BadRequest(new { status = "未拉黑此人" });

            try
            {
                _context.Block.Remove(FO);
                await _context.SaveChangesAsync();
            }
            catch
            {
                return BadRequest(new { status = "取消拉黑失败" });
            }

            return Ok(new { status = "ok" });
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