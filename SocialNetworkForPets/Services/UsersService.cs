using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;
using SocialNetworkForPets.Helper.Constants;
using System.Security.Claims;
using System.Xml.Linq;

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

        public async Task UpdateProfilePicture(int UserId, string newPictureUrl)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.UserId == UserId);
            //checking user exists
            if (user != null) 
            {
                user.UserImgUrl = newPictureUrl;
                _context.User.Update(user);
                await _context.SaveChangesAsync();
            }

        }

        public async Task<List<Post>> GetUserPostsAsync(int UserId)
        {
            //SQL Query code:

            //SELECT * FROM Post p

            //LEFT JOIN[User] u_poster ON p.PosterId = u_poster.Id

            //LEFT JOIN[Like] l ON l.PostId = p.Id

            //LEFT JOIN Favorite f ON f.PostId = p.Id

            //LEFT JOIN Comment c ON c.PostId = p.Id

            //LEFT JOIN[User] u_comment ON c.UserId = u_comment.Id

            //LEFT JOIN Report r ON r.PostId = p.Id

            //WHERE p.PosterId = @UserId

            //ORDER BY p.CreatedAt DESC;

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
