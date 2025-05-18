using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetworkForPets.Data.Models
{
    public class Friendship
    {
        [ForeignKey("User")]
        public int User1Id { get; set; } //Primary and Foreign Key

        [ForeignKey("User")]
        public int User2Id { get; set; } //Primary and Foreign Key

        // Navigation properties
        public User User1 { get; set; }


        public User User2 { get; set; }
    }
}
