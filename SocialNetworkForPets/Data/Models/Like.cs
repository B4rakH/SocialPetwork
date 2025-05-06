using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetworkForPets.Data.Models
{
    public class Like
    {
        [Key]
        public int LikeId { get; set; }

        //Foreign Keys

        [ForeignKey("Post")]
        public int PostId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        // Navigation properties
        public Post Post { get; set; }

        public User User { get; set; }
    }
}
