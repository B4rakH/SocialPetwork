using System.ComponentModel.DataAnnotations;

namespace SocialNetworkForPets.Data.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string UserFullName { get; set; }

        public string UserName { get; set; }

        public string UserPassword { get; set; }

        public string? UserImgUrl { get; set; }

        public string UserRank { get; set; } = "Default";

        public ICollection<Post> Posts { get; set; } = new List<Post>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<Like> Likes { get; set; } = new List<Like>();

        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
