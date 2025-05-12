using SocialNetworkForPets.Data.Models;

namespace SocialNetworkForPets.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetAllPostsAsync(int UserId);

        Task<List<Post>> GetAllFavoritePostsAsync(int UserId);

        Task<Post> CreatePostAsync(Post post);

        Task AddPostCommentAsync(Comment comment);

        Task AddPostReportAsync(Report report);

        Task RemovePostCommentAsync(int CommentId);

        Task<Post> RemovePostAsync(int PostId);

        Task TogglePostLikeAsync(int PostId,int UserId);

        Task TogglePostFavoriteAsync(int PostId, int UserId);

        Task<Post> GetPostByIdAsync(int PostId);

    }
}
