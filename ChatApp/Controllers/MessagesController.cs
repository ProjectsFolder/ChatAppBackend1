using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ChatDbContext _context;

        public MessagesController(ChatDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Policy = "UserAuthorize")]
        public JsonResult GetMessages()
        {
            var messages = _context.Messages.Include(m => m.User);
            var result = new List<object>();
            foreach (var message in messages)
            {
                result.Add(
                    new
                    {
                        id = message.Id,
                        text = message.Text,
                        userid = message.UserId,
                        username = message.User.Login,
                        timecreated = message.Timecreated
                    }
                );
            }
            return new JsonResult(result);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "UserAuthorize")]
        public async Task<IActionResult> GetMessage([FromRoute] int id)
        {
            var message = await _context.Messages.Include(m => m.User).Where(m => m.Id == id).FirstOrDefaultAsync();

            if (message == null)
            {
                return NotFound();
            }

            return Ok(
                new
                {
                    id = message.Id,
                    text = message.Text,
                    userid = message.UserId,
                    username = message.User.Login,
                    timecreated = message.Timecreated
                }
            );
        }

        [HttpPost]
        [Authorize(Policy = "UserAuthorize")]
        public async Task<IActionResult> PostMessage([FromForm] string text)
        {
            var user = GetUserByToken();
            if (user == null)
            {
                return StatusCode(403);
            }

            var message = new Message()
            {
                UserId = user.Id,
                Text = text,
                Timecreated = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds
            };

            if (!TryValidateModel(message))
            {
                return BadRequest(ModelState);
            }

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UserAuthorize")]
        public async Task<IActionResult> PutMessage([FromRoute] int id, [FromForm] string text)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            if (!CheckUserPermissions(message))
            {
                return StatusCode(403);
            }

            message.Text = text;
            if (!TryValidateModel(message))
            {
                return BadRequest(ModelState);
            }

            _context.Entry(message).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "UserAuthorize")]
        public async Task<IActionResult> DeleteMessage([FromRoute] int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            if (!CheckUserPermissions(message))
            {
                return StatusCode(403);
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return Ok(message);
        }

        private User GetUserByToken()
        {
            StringValues token = String.Empty;
            Request.Headers.TryGetValue("Authorization", out token);
            var t = _context.Tokens.Include(e => e.User).FirstOrDefault(e => e.Val == token);
            return t != null ? t.User : null;
        }

        private bool CheckUserPermissions(Message message)
        {
            var user = GetUserByToken();
            return user != null && user.Id == message.UserId;
        }
    }
}
