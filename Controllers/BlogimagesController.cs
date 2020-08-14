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
    public class BlogimagesController : ControllerBase
    {
        private readonly ModelContext _context;

        public BlogimagesController(ModelContext context)
        {
            _context = context;
        }

        // GET: api/Blogimages
        [HttpGet]
        public IEnumerable<Blogimage> GetBlogimage()
        {
            return _context.Blogimage;
        }

        // GET: api/Blogimages/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogimage([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var blogimage = await _context.Blogimage.FindAsync(id);

            if (blogimage == null)
            {
                return NotFound();
            }

            return Ok(blogimage);
        }

        // PUT: api/Blogimages/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlogimage([FromRoute] string id, [FromBody] Blogimage blogimage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != blogimage.ImgUrl)
            {
                return BadRequest();
            }

            _context.Entry(blogimage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogimageExists(id))
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

        // POST: api/Blogimages
        [HttpPost]
        public async Task<IActionResult> PostBlogimage([FromBody] Blogimage blogimage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Blogimage.Add(blogimage);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BlogimageExists(blogimage.ImgUrl))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBlogimage", new { id = blogimage.ImgUrl }, blogimage);
        }

        // DELETE: api/Blogimages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogimage([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var blogimage = await _context.Blogimage.FindAsync(id);
            if (blogimage == null)
            {
                return NotFound();
            }

            _context.Blogimage.Remove(blogimage);
            await _context.SaveChangesAsync();

            return Ok(blogimage);
        }

        private bool BlogimageExists(string id)
        {
            return _context.Blogimage.Any(e => e.ImgUrl == id);
        }
    }
}