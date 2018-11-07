using Microsoft.EntityFrameworkCore;

namespace ChatApp.Models
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Token> Tokens { get; set; }
    }
}
