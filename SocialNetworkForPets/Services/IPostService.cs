using SocialNetworkForPets.Data.Models;
using SocialNetworkForPets.Dtos;

namespace SocialNetworkForPets.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetAllPostsAsync(int UserId);

        Task<List<Post>> GetAllFavoritePostsAsync(int UserId);

        Task CreatePostAsync(Post post);

        Task AddPostCommentAsync(Comment comment);

        Task AddPostReportAsync(Report report);

        Task RemovePostCommentAsync(int CommentId);

        Task RemovePostAsync(int PostId);

        Task<GetNotificationDto> TogglePostLikeAsync(int PostId,int UserId);

        Task<GetNotificationDto> TogglePostFavoriteAsync(int PostId, int UserId);

        Task<Post> GetPostByIdAsync(int PostId);

    }
}
