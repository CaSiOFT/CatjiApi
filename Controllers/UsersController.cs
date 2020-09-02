using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CatjiApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatjiApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ModelContext _context;

        public UsersController(ModelContext context)
        {
            _context = context;
        }

        [Serializable]
        public class UserDTO
        {
            public int usid;
            public string nickname;
            public string password;
            public string email;
            public string phone;
        }

        // POST: api/users/register 注册
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            if (user.email is null && user.phone is null)
            {
                return BadRequest(new { status = "bad request" });
            }

            var findUser = _context.Users.Where(x =>
                user.phone != null && x.Tel == user.phone ||
                user.email != null && x.Email == user.email ||
                x.Nickname == user.nickname
            );

            if (findUser.Count() != 0)
            {
                return BadRequest(new { status = "replicated" });
            }

            var user0 = new Users();
            user0.Nickname = user.nickname;
            user0.Tel = user.phone;
            user0.Email = user.email;
            user0.Password = user.password;
            user0.CreateTime = DateTime.Now;
            user0.ChangedTime = DateTime.Now;

            try
            {
                _context.Users.Add(user0);
                //_context.SaveChanges();
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return NotFound(new { status = "Create failed.", data = e.ToString() });
            }

            await RealLogin(user, user0);

            return Ok(new { status = "ok", data = new { usid = user0.Usid } });
        }

        // POST: /api/users/login 登录
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var auth = await HttpContext.AuthenticateAsync();
            if (auth.Succeeded)
            {
                return Ok(new { status = "already login" });
            }

            if (user.email == null && user.phone == null && user.nickname == null)
            {
                return BadRequest(new { status = "bad request" });
            }

            var findUser = _context.Users.Where(x =>
                (user.phone != null && x.Tel == user.phone ||
                user.email != null && x.Email == user.email ||
                user.nickname != null && x.Nickname == user.nickname) &&
                user.password != null && x.Password == user.password
            ).FirstOrDefault();

            if (findUser == null)
            {
                return NotFound(new { status = "not found" });
            }

            await RealLogin(user, findUser);

            return Ok(new { status = "ok" });
        }

        // 这里必须返回Task不然调用的时候没法await
        // 参数分别为传输用的user对象
        // 和数据库存储的user对象
        private async Task RealLogin(UserDTO user, Users userDAO)
        {
            var claims = new List<Claim> {
                new Claim ("User", userDAO.Usid.ToString ()),
                new Claim ("LastChanged", userDAO.ChangedTime.ToString ())
            };

            //使用证件单元创建一张身份证
            var identity = new ClaimsIdentity(claims);

            //使用身份证创建一个证件当事人
            var identityPrincipal = new ClaimsPrincipal(identity);

            //登录
            await HttpContext.SignInAsync(identityPrincipal, new AuthenticationProperties
            {
                //ExpiresUtc = DateTime.UtcNow.AddDays(1),
                IsPersistent = true,
                AllowRefresh = true
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var auth = await HttpContext.AuthenticateAsync();
            if (!auth.Succeeded)
            {
                return NotFound(new { status = "not login" });
            }
            await HttpContext.SignOutAsync();
            return Ok(new { status = "ok" });
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var auth = await HttpContext.AuthenticateAsync();
            if (!auth.Succeeded)
            {
                return NotFound(new { status = "not login" });
            }

            var claim = User.FindFirstValue("User");
            int usid;

            if (!Int32.TryParse(claim, out usid))
            {
                return BadRequest(new { status = "validation failed" });
            }

            var user = await _context.Users.FindAsync(usid);

            return Ok(new
            {
                status = "ok",
                data = new
                {
                    usid = user.Usid,
                    nickname = user.Nickname,
                    password = user.Password,
                    email = user.Email,
                    phone = user.Tel
                }
            });
        }

        // 获取用户排行榜
        [HttpGet("top")]
        public IActionResult GetUTop()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var user_top = _context.Users.OrderByDescending(x => x.FollowerNum + x.Video.Count()).Take(10);

            var result = user_top.Select(x => new
            {
                u_id = x.Usid,
                u_pic = x.Avatar,
                u_name = x.Nickname
            });

            return Ok(new { status = "ok", data = result });
        }

        // 查询用户信息
        [HttpGet("info")]
        public async Task<IActionResult> GetUserInfo()
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

            int usid;
            bool bo1 = int.TryParse(auth.Principal.Identity.Name, out usid);

            var users = await _context.Users.FindAsync(usid);

            if (users == null)
            {
                return NotFound(new { status = "not found" });
            }

            return Ok(new { status = "ok", data = users });
        }

        // GET: api/Users?usid=5 按usid查询用户信息
        // [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int usid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var users = await _context.Users.FindAsync(usid);

            if (users == null)
            {
                return NotFound(new { status = "not found" });
            }

            return Ok(new { status = "ok", data = users });
        }

        // PUT: api/Users/{id:int} ???
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUsers([FromRoute] int id, [FromBody] Users users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            if (id != users.Usid)
            {
                return BadRequest(new { status = "usid not match" });
            }

            if (!UsersExists(id))
            {
                return NotFound(new { status = "not found" });
            }

            _context.Entry(users).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { status = "db exception" });
            }

            return Ok(new { status = "ok" });
        }

        // POST: api/Users [需要登录] 更新个人信息
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostUsers([FromBody] Users users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            _context.Users.Add(users);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return NotFound(new { status = "db exception", data = e.ToString() });
            }
            return CreatedAtAction("GetUsers", new { id = users.Usid }, users);
        }

        // DELETE: api/Users/5 [需要登录] 更新个人信息
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUsers([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "invalid", data = ModelState });
            }

            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound(new { status = "not found" });
            }

            _context.Users.Remove(users);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return NotFound(new { status = "db exception", data = e.ToString() });
            }
            return Ok(new { status = "ok", data = users });
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.Usid == id);
        }
    }
}