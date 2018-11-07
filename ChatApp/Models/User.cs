using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [Column("login")]
        public string Login { get; set; }

        [Required]
        [MinLength(3)]
        [Column("password")]
        public string Password { get; set; }

        public List<Message> Messages { get; set; }

        public List<Token> Tokens { get; set; }
    }
}
