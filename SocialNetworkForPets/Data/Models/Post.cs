using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Data.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        public string PostText { get; set; }

        public string PostImgUrl { get; set; }

        public int PostLikes { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //Foreign Key
        [ForeignKey("User")]
        public int PosterId { get; set; }

        public User Poster { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
