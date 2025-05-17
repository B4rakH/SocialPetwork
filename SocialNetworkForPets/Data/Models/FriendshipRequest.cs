using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetworkForPets.Data.Models
{
    public class FriendshipRequest
    {
        //Keys
        [ForeignKey("User")]
        public int SenderId { get; set; }

        public User Sender { get; set; }

        [ForeignKey("User")]
        public int ReceiverId { get; set; }

        public User Receiver { get; set; }
    }
}
