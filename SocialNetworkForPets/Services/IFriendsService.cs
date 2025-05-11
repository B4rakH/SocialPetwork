using SocialNetworkForPets.Data.Models;

namespace SocialNetworkForPets.Services
{
    public interface IFriendsService
    {
        Task SendRequestAsync(int senderId, int receiverId);

        Task AcceptRequestAsync(int requestId);

        Task RejectRequestAsync(int requestId);

        Task RemoveFriendshipAsync(int friendshipId);

        Task<List<(User,bool)>> GetSuggestedPetsAsync(int UserId);

        Task<List<FriendshipRequest>> GetSentFriendRequestAsync(int userId);
    }
}
