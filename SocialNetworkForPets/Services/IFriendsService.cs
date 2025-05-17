using SocialNetworkForPets.Data.Models;

namespace SocialNetworkForPets.Services
{
    public interface IFriendsService
    {
        Task SendRequestAsync(int senderId, int receiverId);

        Task AcceptRequestAsync(int senderId, int receiverId);

        Task RejectRequestAsync(int senderId, int receiverId);

        Task RemoveFriendAsync(int userId,int friendId);

        Task<List<(User,int,bool)>> GetSuggestedPetsAsync(int UserId);

        Task<List<FriendshipRequest>> GetSentFriendRequestAsync(int userId);

        Task<List<FriendshipRequest>> GetReceivedFriendRequestAsync(int userId);

        Task<List<Friendship>> GetFriendsAsync(int userId);
    }
}
