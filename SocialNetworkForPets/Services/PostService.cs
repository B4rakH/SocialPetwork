using Microsoft.EntityFrameworkCore;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;
using SocialNetworkForPets.Helper;
using SocialNetworkForPets.ViewModels.Home;

namespace SocialNetworkForPets.Services
{
    public class PostService: IPostService
    {
        private readonly AppDbContext _context;
        public PostService(AppDbContext context) 
        {
            _context = context;
        }
        public async Task<List<Post>> GetAllPostsAsync(int UserId)
        {
            var allPosts = await _context.Post
                .Include(p => p.Poster)
                .Include(p => p.Likes)
                .Include(p => p.Favorites)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return allPosts;
        }

        public async Task AddPostCommentAsync(Comment comment)
        {
            await _context.Comment.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<Post> CreatePostAsync(Post post, IFormFile image)
        {
            //Checking The file Upload if exists
            if (image != null && image.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                if (image.ContentType.Contains("image"))
                {
                    string rootFolderPathImages = Path.Combine(rootFolderPath, "images/uploaded");
                    Directory.CreateDirectory(rootFolderPathImages);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    string filePath = Path.Combine(rootFolderPathImages, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await image.CopyToAsync(stream);

                    post.PostImgUrl = "/images/uploaded/" + fileName;
                }

            }
            await _context.Post.AddAsync(post);
            await _context.SaveChangesAsync();

            return post;
        }


        public async Task<Post> RemovePostAsync(int PostId)
        {
            var postDb = await _context.Post.FirstOrDefaultAsync(p => p.PostId == PostId);

            if (postDb != null)
            {
                //If post has comments, delete one by one first
                foreach (var comment in _context.Comment.Where(c => c.PostId == postDb.PostId)) _context.Comment.Remove(comment);

                _context.Post.Remove(postDb);
                await _context.SaveChangesAsync();
            }
            return postDb;
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

        public async Task TogglePostFavoriteAsync(int PostId, int UserId)
        {
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
            }
            await _context.SaveChangesAsync();
        }

        public async Task TogglePostLikeAsync(int PostId, int UserId)
        {
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
            }
            await _context.SaveChangesAsync();
        }
    }
}
