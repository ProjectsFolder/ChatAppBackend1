using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
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
        public async Task<IActionResult> Register([FromForm] User user, IFormFile image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (UsernameExists(user.Login))
            {
                return BadRequest(new { User = new[] { $"The user with login '{user.Login}' already exists" } });
            }

            await SaveImage(user, image);

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
                    Timecreated = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds
                };

                _context.Tokens.Add(token);
                await _context.SaveChangesAsync();

                return Ok(new { token = guid, userid = u.Id, username = u.Login });
            }

            return StatusCode(403);
        }

        [HttpGet("authorized")]
        [Authorize(Policy = "UserAuthorize")]
        public IActionResult Authorized()
        {
            return Ok();
        }

        [HttpPost("logout")]
        [Authorize(Policy = "UserAuthorize")]
        public async Task<IActionResult> Logout()
        {
            StringValues token = string.Empty;
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
            StringValues strtoken = string.Empty;
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

        [HttpGet("image/{id}")]
        [Authorize(Policy = "UserAuthorize")]
        public async Task<IActionResult> GetImage([FromRoute] int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.Image == null)
            {
                return NoContent();
            }

            return File(user.Image, "image/jpeg");
        }

        [HttpPut("image")]
        [Authorize(Policy = "UserAuthorize")]
        public async Task<IActionResult> PutImage(IFormFile image)
        {
            var user = GetUserByToken();
            if (user == null)
            {
                return NotFound();
            }

            if (!await SaveImage(user, image))
            {
                user.Image = null;
            }

            if (!TryValidateModel(user))
            {
                return BadRequest(ModelState);
            }

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok();
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

        private User GetUserByToken()
        {
            StringValues token = string.Empty;
            Request.Headers.TryGetValue("Authorization", out token);
            var t = _context.Tokens.Include(e => e.User).FirstOrDefault(e => e.Val == token);
            return t?.User;
        }

        private async Task<bool> SaveImage(User user, IFormFile file)
        {
            if (file != null && IsImageFile(file) && file.Length <= 5242880)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    user.Image = memoryStream.ToArray();
                    return true;
                }
            }
            return false;
        }

        private bool IsImageFile(IFormFile file)
        {
            var cntType = file.ContentType;
            var cmpType = StringComparison.OrdinalIgnoreCase;
            return string.Equals(cntType, "image/jpeg", cmpType);
        }
    }
}
