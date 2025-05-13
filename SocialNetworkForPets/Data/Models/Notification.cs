using System.ComponentModel.DataAnnotations;

namespace SocialNetworkForPets.Data.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public int? PostId { get; set; }

        public string Message { get; set; }

        public string Type { get; set; }
    }
}
