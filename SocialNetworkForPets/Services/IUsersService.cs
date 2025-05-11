using SocialNetworkForPets.Data.Models;

namespace SocialNetworkForPets.Services
{
    public interface IUsersService
    {
        Task<User> GetUserAsync(int UserId);

        Task UpdateProfilePicture(int UserId, string pictureUrl);

        Task<List<Post>> GetUserPostsAsync(int UserId);
    }
}
