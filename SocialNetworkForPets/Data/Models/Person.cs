using System.ComponentModel.DataAnnotations;

namespace TestApp.Data.Models
{
    public class Person
    {
        [Key]
        public int PerId { get; set; }

        public string PerName { get; set; }

        public string PerPassword { get; set; }

        public string? PerImgUrl { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();

        public ICollection<Pet> PetsOwned { get; set; } = new List<Pet>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
