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
    public class FavoritesController : ControllerBase
    {
        private readonly ModelContext _context;

        public FavoritesController(ModelContext context)
        {
            _context = context;
        }

        // GET: api/Favorites
        [HttpGet]
        public IEnumerable<Favorite> GetFavorite()
        {
            return _context.Favorite;
        }

        // GET: api/Favorites/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFavorite([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var favorite = await _context.Favorite.FindAsync(id);

            if (favorite == null)
            {
                return NotFound();
            }

            return Ok(favorite);
        }

        // PUT: api/Favorites/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFavorite([FromRoute] int id, [FromBody] Favorite favorite)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != favorite.Usid)
            {
                return BadRequest();
            }

            _context.Entry(favorite).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FavoriteExists(id))
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

        // POST: api/Favorites
        [HttpPost]
        public async Task<IActionResult> PostFavorite([FromBody] Favorite favorite)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Favorite.Add(favorite);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FavoriteExists(favorite.Usid))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFavorite", new { id = favorite.Usid }, favorite);
        }

        // DELETE: api/Favorites/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFavorite([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var favorite = await _context.Favorite.FindAsync(id);
            if (favorite == null)
            {
                return NotFound();
            }

            _context.Favorite.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok(favorite);
        }

        private bool FavoriteExists(int id)
        {
            return _context.Favorite.Any(e => e.Usid == id);
        }
    }
}