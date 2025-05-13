using SocialNetworkForPets.Data.Models;

namespace SocialNetworkForPets.ViewModels.Users
{
    public class GetUserProfileVM
    {

        public User User { get; set; }
        public List<Post> Posts { get; set; }

        public bool RequestedOrAdded { get; set; }

    }
}
