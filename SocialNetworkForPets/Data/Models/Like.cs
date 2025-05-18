using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetworkForPets.Data.Models
{
    public class Like
    {
        //Keys
        [ForeignKey("Post")]
        public int PostId { get; set; } //Primary and Foreign Key

        [ForeignKey("User")]
        public int UserId { get; set; } //Primary and Foreign Key

        // Navigation properties
        public Post Post { get; set; }

        public User User { get; set; }
    }
}
