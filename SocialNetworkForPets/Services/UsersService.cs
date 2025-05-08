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
    }
}
