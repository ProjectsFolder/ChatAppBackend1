using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ChatDbContext _context;

        public UsersController(ChatDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public string Index()
        {
            return "API is started...";
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (UsernameExists(user.Login))
            {
                return BadRequest(new { User = new[] { $"The user with login '{user.Login}' already exists" } });
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var u = await _context.Users.FirstOrDefaultAsync(
                e => e.Login.ToLower() == user.Login.ToLower() && e.Password == user.Password);

            if (u != null)
            {
                var guid = GetGuid();

                var token = new Token()
                {
                    Val = guid.ToString(),
                    UserId = u.Id,
                    Timecreated = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds
                };

                _context.Tokens.Add(token);
                await _context.SaveChangesAsync();

                return Ok(new{ token = guid, userid = u.Id });
            }

            return StatusCode(403);
        }

        [HttpPost("logout")]
        [Authorize(Policy = "UserAuthorize")]
        public async Task<IActionResult> Logout()
        {
            StringValues token = String.Empty;
            var result = Request.Headers.TryGetValue("Authorization", out token);
            
            if (result)
            {
                var t = await _context.Tokens.FirstOrDefaultAsync(e => e.Val == token);
                if (t != null)
                {
                    _context.Tokens.Remove(t);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
            }

            return BadRequest();
        }

        [HttpPost("logout_all")]
        [Authorize(Policy = "UserAuthorize")]
        public async Task<IActionResult> LogoutAll()
        {
            StringValues strtoken = String.Empty;
            var result = Request.Headers.TryGetValue("Authorization", out strtoken);

            if (result)
            {
                var t = await _context.Tokens.FirstOrDefaultAsync(e => e.Val == strtoken);
                if (t != null)
                {
                    var tokens = _context.Tokens.Where(e => e.UserId == t.UserId);
                    foreach (var token in tokens)
                    {
                        _context.Tokens.Remove(token);
                    }
                    await _context.SaveChangesAsync();
                    return Ok();
                }
            }

            return BadRequest();
        }

        private bool UsernameExists(string login)
        {
            return _context.Users.Any(e => e.Login == login);
        }

        private Guid GetGuid()
        {
            Guid guid;
            Token token = null;
            do
            {
                guid = Guid.NewGuid();
                token = _context.Tokens.FirstOrDefault(e => e.Val == guid.ToString());
            } while (token != null);
            return guid;
        }
    }
}
