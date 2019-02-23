using System;
using System.Linq;
using ChatApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace ChatApp.Services
{
    public class Utils : IUtils
    {
        private readonly ChatDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContext;

        public Utils(ChatDbContext dbContext, IHttpContextAccessor httpContext)
        {
            _dbContext = dbContext;
            _httpContext = httpContext;
        }

        public Guid GetGiudByUser()
        {
            Guid guid;
            Token token = null;
            do
            {
                guid = Guid.NewGuid();
                token = _dbContext.Tokens.FirstOrDefault(e => e.Val == guid.ToString());
            } while (token != null);
            return guid;
        }

        public User GetUserByToken()
        {
            StringValues token = string.Empty;
            _httpContext.HttpContext.Request.Headers.TryGetValue("Authorization", out token);
            var t = _dbContext.Tokens.Include(e => e.User).FirstOrDefault(e => e.Val == token);
            return t?.User;
        }
    }
}
