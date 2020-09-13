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
    public class BlogcommentsController : ControllerBase
    {
        private readonly ModelContext _context;

        public BlogcommentsController(ModelContext context)
        {
            _context = context;
        }

        [HttpPost("addBC")]
        public async Task<IActionResult> addBC(Blogcomment bc0)
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

            var BC = new Blogcomment();
            BC.Usid = user.Usid;
            BC.Bid = bc0.Bid;
            BC.Content = bc0.Content;
            BC.CreateTime = DateTime.Now;

            try
            {
                _context.Blogcomment.Add(BC);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return NotFound(new { status = "Create failed.", data = e.ToString() });
            }

            return Ok(new { status = "ok" });
        }

        // GET: api/Blogcomments
        [HttpGet]
        public IEnumerable<Blogcomment> GetBlogcomment()
        {
            return _context.Blogcomment;
        }

        // GET: api/Blogcomments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogcomment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var blogcomment = await _context.Blogcomment.FindAsync(id);

            if (blogcomment == null)
            {
                return NotFound();
            }

            return Ok(blogcomment);
        }

        // PUT: api/Blogcomments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlogcomment([FromRoute] int id, [FromBody] Blogcomment blogcomment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != blogcomment.Bcid)
            {
                return BadRequest();
            }

            _context.Entry(blogcomment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogcommentExists(id))
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

        // POST: api/Blogcomments
        [HttpPost]
        public async Task<IActionResult> PostBlogcomment([FromBody] Blogcomment blogcomment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Blogcomment.Add(blogcomment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBlogcomment", new { id = blogcomment.Bcid }, blogcomment);
        }

        // DELETE: api/Blogcomments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogcomment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var blogcomment = await _context.Blogcomment.FindAsync(id);
            if (blogcomment == null)
            {
                return NotFound();
            }

            _context.Blogcomment.Remove(blogcomment);
            await _context.SaveChangesAsync();

            return Ok(blogcomment);
        }

        private bool BlogcommentExists(int id)
        {
            return _context.Blogcomment.Any(e => e.Bcid == id);
        }
    }
}