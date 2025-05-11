using Microsoft.EntityFrameworkCore;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;

namespace SocialNetworkForPets.Services
{
    public class UsersService : IUsersService
    {
        private readonly AppDbContext _context;

        public UsersService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserAsync(int UserId)
        {
            return await _context.User.FirstOrDefaultAsync(u => u.UserId == UserId) ?? new User();
        }

        public async Task UpdateProfilePicture(int UserId, string pictureUrl)
        {
            var userDb = await _context.User.FirstOrDefaultAsync(u => u.UserId == UserId);
            if (userDb != null) 
            {
                userDb.UserImgUrl = pictureUrl;
                _context.User.Update(userDb);
                await _context.SaveChangesAsync();
            }

        }

        public async Task<List<Post>> GetUserPostsAsync(int UserId)
        {
            var allPosts = await _context.Post
                .Where(p => p.PosterId == UserId)
                .Include(p => p.Poster)
                .Include(p => p.Likes)
                .Include(p => p.Favorites)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Include(p => p.Reports)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return allPosts;

        }
    }
}
