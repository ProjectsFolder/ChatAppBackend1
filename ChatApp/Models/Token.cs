using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Models
{
    [Table("tokens")]
    public class Token
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("text")]
        public string Val { get; set; }

        [Required]
        [Column("timecreated")]
        public int Timecreated { get; set; }

        [Column("userid")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
