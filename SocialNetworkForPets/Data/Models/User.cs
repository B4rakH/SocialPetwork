using System.ComponentModel.DataAnnotations;

namespace SocialNetworkForPets.Data.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string UserFullName { get; set; }

        public string UserMail { get; set; }

        public string UserPassword { get; set; }

        public string? UserImgUrl { get; set; }

        public string UserRole { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();

        public ICollection<Pet> PetsOwned { get; set; } = new List<Pet>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<Like> Likes { get; set; } = new List<Like>();

        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
