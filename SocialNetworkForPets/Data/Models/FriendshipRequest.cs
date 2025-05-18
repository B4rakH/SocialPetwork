using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetworkForPets.Data.Models
{
    public class FriendshipRequest
    {
        [ForeignKey("User")]
        public int SenderId { get; set; } //Primary and Foreign Key

        [ForeignKey("User")]
        public int ReceiverId { get; set; } //Primary and Foreign Key

        // Navigation properties
        public User Sender { get; set; }

        public User Receiver { get; set; }
    }
}
