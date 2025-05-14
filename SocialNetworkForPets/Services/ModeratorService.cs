using Microsoft.EntityFrameworkCore;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;

namespace SocialNetworkForPets.Services
{
    public class ModeratorService: IModeratorService
    {
         private readonly AppDbContext _context;

        public ModeratorService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Post>> GetReportedPostsAsync()
        {
            var repLimit = 0;

            //SELECT p.*
            //FROM Posts p
            //JOIN(
            //    SELECT PostId
            //    FROM Reported
            //    GROUP BY PostId
            //    HAVING COUNT(*) > replimit
            //) r ON p.Id = r.PostId;

            var posts = await _context.Report
                               .GroupBy(r => r.PostId)
                               .Where(g => g.Count() > repLimit)
                               .Select(g => g.Key)
                               .Join(_context.Post,
                                     postId => postId,
                                     p => p.PostId,
                                     (postId, p) => p)
                               .Include(p => p.Poster)
                               .ToListAsync();

            return posts;
        }
    }
}
