using SocialNetworkForPets.Data.Models;

namespace SocialNetworkForPets.Services
{
    public interface IModeratorService
    {
        public Task<List<Post>> GetReportedPostsAsync();
    }
}
