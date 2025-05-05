using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Data.Models
{
    public class PetOwner
    {
        //Foreign Key
        [ForeignKey("Person")]
        public int OwnerId { get; set; }

        public Person Owner { get; set; }
    }
}
