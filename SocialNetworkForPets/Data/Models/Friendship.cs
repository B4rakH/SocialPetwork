using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetworkForPets.Data.Models
{
    public class Friendship
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int User1Id { get; set; }

        public User User1 { get; set; }

        [ForeignKey("User")]
        public int User2Id { get; set; }

        public User User2 { get; set; }
    }
}
