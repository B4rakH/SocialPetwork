using System.ComponentModel.DataAnnotations;

namespace SocialNetworkForPets.Data.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string UserPassword { get; set; }

        public string? UserImgUrl { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();

        public ICollection<Pet> PetsOwned { get; set; } = new List<Pet>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
