using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetworkForPets.Data.Models
{
    public class Administrator
    {
        //Foreign Key
        [ForeignKey("User")]
        public int AdminId { get; set; }

        public User Admin { get; set; }
    }
}
