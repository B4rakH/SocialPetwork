using SocialNetworkForPets.Data.Models;

namespace SocialNetworkForPets.Services
{
    public interface IHashtagService
    {

        Task HashtagsInNewPostAsync(string postText);

        Task HashtagsInRemovedPostAsync(string postText);

        Task<List<Hashtag>> GetTrendTopics();
    }
}
