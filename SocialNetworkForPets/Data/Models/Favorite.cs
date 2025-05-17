using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocialNetworkForPets.Data.Models
{
    public class Favorite
    {
        //Keys

        [ForeignKey("Post")]
        public int PostId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        // Navigation properties
        public Post Post { get; set; }

        public User User { get; set; }
    }
}
