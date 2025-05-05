using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Data.Models
{
    public class PetOwner
    {
        //Foreign Key
        [ForeignKey("User")]
        public int OwnerId { get; set; }

        public User Owner { get; set; }
    }
}
