using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CatjiApi.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CatjiApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ModelContext _context;

        public UsersController(ModelContext context)
        {
            _context = context;
        }

        public class User
        {
            public int usid;
            public string nickname;
            public string password;
            public string email;
            public string phone;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var users = _context.Users.Where(x => user.phone != null && x.Tel == user.phone || user.email != null && x.Email == user.email || x.Nickname == user.nickname);

            if (users.Count() != 0)
                return BadRequest();

            var user0 = new Users();
            user0.Nickname = user.nickname;
            user0.Tel = user.phone;
            user0.Email = user.email;
            user0.Password = user.password;
            user0.CreateTime = DateTime.Now;
            user0.ChangedTime = user0.CreateTime;

            _context.Users.Add(user0);
            await _context.SaveChangesAsync();

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user0.Usid.ToString()),
                new Claim("LastChanged", user0.ChangedTime.ToString())
            };

            //使用证件单元创建一张身份证
            var identity = new ClaimsIdentity(claims, "Cookies");

            //使用身份证创建一个证件当事人
            var identityPrincipal = new ClaimsPrincipal(identity);

            //登录
            await HttpContext.SignInAsync("Cookies", identityPrincipal
                , new AuthenticationProperties
                {
                    //ExpiresUtc = DateTime.UtcNow.AddDays(1),
                    IsPersistent = true,
                    AllowRefresh = true
                }
                );
            return Ok(new { });

        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user0 = await _context.Users.FindAsync(user.usid);

            if (user0 == null)
            {
                return NotFound();
            }

            if (user0.Password != user.password)
                return BadRequest();

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.usid.ToString()),
                new Claim("LastChanged", user0.ChangedTime.ToString())
            };

            //使用证件单元创建一张身份证
            var identity = new ClaimsIdentity(claims, "Cookies");

            //使用身份证创建一个证件当事人
            var identityPrincipal = new ClaimsPrincipal(identity);

            //登录
            await HttpContext.SignInAsync("Cookies", identityPrincipal
                , new AuthenticationProperties
                {
                    //ExpiresUtc = DateTime.UtcNow.AddDays(1),
                    IsPersistent = true,
                    AllowRefresh = true
                }
                );
            return Ok(new { });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok(new { });
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetUserInfo()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var auth = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!auth.Succeeded)
            {
                return BadRequest();
            }

            int usid;
            bool bo1 = int.TryParse(auth.Principal.Identity.Name, out usid);

            var users = await _context.Users.FindAsync(usid);

            if (users == null)
            {
                return NotFound();
            }

            return Ok(users);
        }

        // GET: api/Users
        [HttpGet]
        public IEnumerable<Users> GetUsers()
        {
            return _context.Users;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsers([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var users = await _context.Users.FindAsync(id);

            if (users == null)
            {
                return NotFound();
            }

            return Ok(users);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsers([FromRoute] int id, [FromBody] Users users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != users.Usid)
            {
                return BadRequest();
            }

            _context.Entry(users).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(id))
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

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> PostUsers([FromBody] Users users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Users.Add(users);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsers", new { id = users.Usid }, users);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsers([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }

            _context.Users.Remove(users);
            await _context.SaveChangesAsync();

            return Ok(users);
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.Usid == id);
        }
    }
}