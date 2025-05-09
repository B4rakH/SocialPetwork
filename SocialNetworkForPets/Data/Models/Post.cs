using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetworkForPets.Data.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        public string PostText { get; set; }

        public string PostImgUrl { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //Foreign Key
        [ForeignKey("User")]
        public int PosterId { get; set; }

        public User Poster { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<Like> Likes { get; set; } = new List<Like>();

        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
