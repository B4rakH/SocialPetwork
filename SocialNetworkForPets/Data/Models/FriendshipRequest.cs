using System.ComponentModel.DataAnnotations;

namespace SocialNetworkForPets.Data.Models
{
    public class FriendshipRequest
    {
        [Key]
        public int RequestId { get; set; }

        public int User1Id { get; set; }

        public User User1 { get; set; }

        public int User2Id { get; set; }

        public User User2 { get; set; }
    }
}
