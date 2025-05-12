using SocialNetworkForPets.Data.Models;

namespace SocialNetworkForPets.ViewModels.Friends
{
    public class FriendshipVM
    {
        public List<Friendship> Friends { get; set; } = new List<Friendship>();
        public List<FriendshipRequest> FriendRequestsSent = new List<FriendshipRequest>();
        public List<FriendshipRequest> FriendRequestsReceived = new List<FriendshipRequest>();
    }
}
