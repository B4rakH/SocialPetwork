using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Data.Models
{
    public class Pet
    {
        [Key]
        public int PetId { get; set; }
        public string PetName { get; set; }
        public string PetBreed { get; set; }

        //Foreign Key

        [ForeignKey("User")]
        public int OwnerId { get; set; }
        public User Owner { get; set; }
    }
}
