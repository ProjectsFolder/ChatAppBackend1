using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Policies
{
    public class UserAuthorizedHandler : AuthorizationHandler<UserAuthorizedRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor = null;
        private readonly ChatDbContext _chatDbContext = null;

        public UserAuthorizedHandler(IHttpContextAccessor httpContextAccessor, ChatDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _chatDbContext = context;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserAuthorizedRequirement requirement)
        {
            StringValues token = String.Empty;
            var result = _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out token);
            if (result)
            {
                var t = _chatDbContext.Tokens.FirstOrDefault(e => e.Val == token);
                if (t != null)
                {
                    context.Succeed(requirement);
                }
            }
            return Task.FromResult(0);
        }
    }
}
