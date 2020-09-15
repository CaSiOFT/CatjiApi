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
using System.Text.RegularExpressions;

namespace CatjiApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly ModelContext _context;

        public BlogsController(ModelContext context)
        {
            _context = context;
        }

        [HttpPost("release"), DisableRequestSizeLimit]
        public async Task<IActionResult> Upload(IFormCollection paras)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var auth = await HttpContext.AuthenticateAsync();
            if (!auth.Succeeded)
            {
                return BadRequest(new { status = "not login" });
            }

            var claim = User.FindFirstValue("User");
            int usid;

            if (!int.TryParse(claim, out usid))
            {
                return BadRequest(new { status = "validation failed" });
            }

            var blogPO = new Blog();

            blogPO.IsPublic = Convert.ToDecimal(paras["is_public"]);
            blogPO.Content = paras["content"];
            blogPO.CreateTime = DateTime.Now;
            blogPO.Usid = usid;

            try
            {
                await _context.Blog.AddAsync(blogPO);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    status = "Create failed.",
                    data = e.ToString()
                });
            }

            MatchCollection mc = Regex.Matches(blogPO.Content, @"(?<=\#)[\s\S]*?(?=\#)");
            try
            {
                for (int i = 0; i < mc.Count; i += 2)
                {
                    var tag = await _context.Tag.FirstOrDefaultAsync(x => x.Name == mc[i].ToString());
                    if (tag == null)
                    {
                        tag = new Tag();
                        tag.Name = mc[i].ToString();
                        await _context.Tag.AddAsync(tag);
                    }
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    status = "Create tag failed.",
                    data = e.ToString()
                });
            }

            try
            {
                foreach (var v in paras.Files.GetFiles("images"))
                {
                    string FileName = Guid.NewGuid().ToString() + '.' + v.FileName.Split('.').Last();
                    string pathToSave = "wwwroot/images" + "/" + FileName;
                    using (var stream = System.IO.File.Create(pathToSave))
                    {
                        await v.CopyToAsync(stream);
                    }
                    var BG = new Blogimage();
                    BG.Bid = blogPO.Bid;
                    BG.ImgUrl = FileName;
                    await _context.Blogimage.AddAsync(BG);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    status = "Create image failed.",
                    data = e.ToString()
                });
            }

            return Ok(new { status = "ok" });
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetBlogInfo(int offset, int usid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var blogs = _context.Blog.Where(x => x.Usid == usid).OrderByDescending(x => x.CreateTime).Skip(offset).Take(10);

            foreach (var blog in blogs)
            {
                blog.Blogimage = await _context.Blogimage.Where(x => x.Bid == blog.Bid).ToListAsync();
            }

            bool isLogin = false;
            int myid = -1;
            List<int> LikeList = new List<int>();

            var auth = await HttpContext.AuthenticateAsync();
            if (auth.Succeeded)
            {
                var claim = User.FindFirstValue("User");
                if (int.TryParse(claim, out myid))
                    isLogin = true;
            }

            if (isLogin)
            {
                LikeList = await _context.Likeblog.Where(x => x.Usid == myid).Select(x => x.Bid).ToListAsync();
            }

            string baseUrl = Request.Scheme + "://" + Request.Host + "/";

            var result = blogs.Select(x => new
            {
                bid = x.Bid,
                create_time = x.CreateTime.ToTimestamp(),
                content = x.Content,
                transmit_num = x.TransmitNum,
                comment_num = x.CommentNum,
                like_num = x.LikeNum,
                images = x.Blogimage.Select(y => baseUrl + "images/" + y.ImgUrl),
                ilike = LikeList.Contains(x.Bid) ? 1 : 0
            });

            return Ok(new { status = "ok", data = result });
        }

        [HttpGet("content")]
        public async Task<IActionResult> GetBlog(int offset, bool only_cat)
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
            int usid;

            if (!int.TryParse(claim, out usid))
            {
                return BadRequest(new { status = "validation failed" });
            }

            IQueryable<Blog> blogs;
            //var followedUsid = await _context.Follow.Where(x => x.Usid == usid && (!only_cat || _context.Users.Find(x.FollowUsid).CatId != null)).Select(x => x.FollowUsid).ToListAsync();

            if (!only_cat)
            {
                var followedUsid = await _context.Follow.Where(x => x.Usid == usid).Select(x => x.FollowUsid).ToListAsync();
                followedUsid.Add(usid);
                blogs = _context.Blog.Where(x => followedUsid.Contains(x.Usid)).OrderByDescending(x => x.CreateTime).Skip(offset).Take(10);
            }
            else
            {
                blogs = _context.Blog.Where(x => x.IsPublic != 0).OrderByDescending(x => x.CreateTime).Skip(offset).Take(10);
            }

            foreach (var blog in blogs)
            {
                blog.Us = await _context.Users.FindAsync(blog.Usid);
                blog.Blogimage = await _context.Blogimage.Where(x => x.Bid == blog.Bid).ToListAsync();
            }

            var LikeList = await _context.Likeblog.Where(x => x.Usid == usid).Select(x => x.Bid).ToListAsync();

            string baseUrl = Request.Scheme + "://" + Request.Host + "/";

            var result = blogs.Select(x => new
            {
                bid = x.Bid,
                create_time = x.CreateTime.ToTimestamp(),
                content = x.Content,
                up = new
                {
                    usid = x.Us.Usid,
                    name = x.Us.Nickname,
                    avatar = baseUrl + "images/" + x.Us.Avatar
                },
                transmit_num = x.TransmitNum,
                comment_num = x.CommentNum,
                like_num = x.LikeNum,
                images = x.Blogimage.Select(y => baseUrl + "images/" + y.ImgUrl),
                ilike = LikeList.Contains(x.Bid) ? 1 : 0
            });

            return Ok(new { status = "ok", data = result });
        }

        // GET: api/Blogs
        [HttpGet]
        public IEnumerable<Blog> GetBlog()
        {
            return _context.Blog;
        }

        // GET: api/Blogs/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlog([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var blog = await _context.Blog.FindAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            return Ok(blog);
        }

        // PUT: api/Blogs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlog([FromRoute] int id, [FromBody] Blog blog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != blog.Bid)
            {
                return BadRequest();
            }

            _context.Entry(blog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogExists(id))
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

        // POST: api/Blogs
        [HttpPost]
        public async Task<IActionResult> PostBlog([FromBody] Blog blog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Blog.Add(blog);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBlog", new { id = blog.Bid }, blog);
        }

        // DELETE: api/Blogs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var blog = await _context.Blog.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            _context.Blog.Remove(blog);
            await _context.SaveChangesAsync();

            return Ok(blog);
        }

        private bool BlogExists(int id)
        {
            return _context.Blog.Any(e => e.Bid == id);
        }
    }
}