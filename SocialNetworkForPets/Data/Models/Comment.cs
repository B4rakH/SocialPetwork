using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetworkForPets.Data.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        public string CommentText { get; set; }


        [ForeignKey("User")]
        public int UserId { get; set; } //Foreign Key

        [ForeignKey("Post")]
        public int PostId { get; set; } //Foreign Key

        // Navigation properties

        public User User { get; set; }


        public Post Post { get; set; }
    }
}
