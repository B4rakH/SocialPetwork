using SocialNetworkForPets.Data.Models;

namespace SocialNetworkForPets.Services
{
    public interface IFriendsService
    {
        Task SendRequestAsync(int senderId, int receiverId);

        Task AcceptRequestAsync(int requestId);

        Task RejectRequestAsync(int requestId);

        Task RemoveFriendAsync(int friendshipId);

        Task<List<(User,int,bool)>> GetSuggestedPetsAsync(int UserId);

        Task<List<FriendshipRequest>> GetSentFriendRequestAsync(int userId);

        Task<List<FriendshipRequest>> GetReceivedFriendRequestAsync(int userId);

        Task<List<Friendship>> GetFriendsAsync(int userId);
    }
}
