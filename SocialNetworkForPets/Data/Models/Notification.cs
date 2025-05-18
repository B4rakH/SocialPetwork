using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetworkForPets.Data.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; } //Foreign Key

        public DateTime DateCreated { get; set; } = DateTime.Now;

        [ForeignKey("Post")]
        public int? PostId { get; set; } //Foreign Key

        public string Message { get; set; }

        public string Type { get; set; }

        // Navigation properties

        public User? User { get; set; }

        public Post? Post { get; set; }
    }
}
