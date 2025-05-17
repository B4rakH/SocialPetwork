using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;
using SocialNetworkForPets.Dtos;

namespace SocialNetworkForPets.Services
{
    public class PostService: IPostService
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly IHashtagService _hashtagService;
        public PostService(AppDbContext context, INotificationService notificationService, IHashtagService hashtagService) 
        {
            _context = context;
            _notificationService = notificationService;
            _hashtagService = hashtagService;
        }
        public async Task<List<Post>> GetAllPostsAsync(int UserId)
        {
            var allPosts = await _context.Post
                .Include(p => p.Poster)
                .Include(p => p.Likes)
                .Include(p => p.Favorites)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Include(p => p.Reports)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return allPosts;
        }

        public async Task<List<Post>> GetAllFavoritePostsAsync(int UserId)
        {
            var allFavoritedPosts = await _context.Favorite
                .Include(f => f.Post.Poster)
                .Include(f => f.Post.Comments)
                    .ThenInclude(c => c.User)
                .Include(f => f.Post.Likes)
                .Include(f => f.Post.Favorites)
                .Where(n => n.UserId == UserId)
                .OrderByDescending(f => f.Post.CreatedAt)
                .Select(n => n.Post)
                .ToListAsync();

            return allFavoritedPosts;

        }

        public async Task AddPostCommentAsync(Comment comment)
        {
            await _context.Comment.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task CreatePostAsync(Post post)
        {
            await _context.Post.AddAsync(post);
            await _hashtagService.HashtagsInNewPostAsync(post.PostText);
            await _context.SaveChangesAsync();
        }


        public async Task RemovePostAsync(int PostId)
        {
            var postDb = await _context.Post.FirstOrDefaultAsync(p => p.PostId == PostId);

            if (postDb != null)
            {
                //If post has comments, delete one by one first
                foreach (var comment in _context.Comment.Where(c => c.PostId == PostId)) _context.Comment.Remove(comment);

                _context.Post.Remove(postDb);
                await _hashtagService.HashtagsInRemovedPostAsync(postDb.PostText);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemovePostCommentAsync(int CommentId)
        {
            var commentDb = await _context.Comment
                .FirstOrDefaultAsync(c => c.CommentId == CommentId);
            if (commentDb != null)
            {
                _context.Comment.Remove(commentDb);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<GetNotificationDto> TogglePostFavoriteAsync(int PostId, int UserId)
        {
            var response = new GetNotificationDto()
            {
                IsSuccess = true,
                SendNotification = false
            };

            var favorite = await _context.Favorite
                .Where(l => l.PostId == PostId && l.UserId == UserId)
                .FirstOrDefaultAsync();

            if (favorite != null)
            {
                _context.Favorite.Remove(favorite);
            }
            else
            {
                var newFavorite = new Favorite()
                {
                    PostId = PostId,
                    UserId = UserId
                };

                await _context.Favorite.AddAsync(newFavorite);
                response.SendNotification = true;
            }

            await _context.SaveChangesAsync();

            return response;
        }

        public async Task<GetNotificationDto> TogglePostLikeAsync(int PostId, int UserId)
        {
            var response = new GetNotificationDto()
            {
                IsSuccess = true,
                SendNotification = false
            };

            var like = await _context.Like
                .Where(l => l.PostId == PostId && l.UserId == UserId)
                .FirstOrDefaultAsync();

            if (like != null)
            {
                _context.Like.Remove(like);
            }
            else
            {
                var newLike = new Like()
                {
                    PostId = PostId,
                    UserId = UserId
                };

                await _context.Like.AddAsync(newLike);

                response.SendNotification = true;
            }

            await _context.SaveChangesAsync();

            return response;
        }

        public async Task AddPostReportAsync(Report report)
        {
            await _context.Report.AddAsync(report);
            await _context.SaveChangesAsync();
        }

        public async Task<Post> GetPostByIdAsync(int postId)
        {
            var postDb = await _context.Post
               .Include(n => n.Poster)
               .Include(n => n.Likes)
               .Include(n => n.Favorites)
               .Include(n => n.Comments).ThenInclude(n => n.User)
               .FirstOrDefaultAsync(n => n.PostId == postId);

            return postDb;
        }
    }
}
