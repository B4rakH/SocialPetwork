using Microsoft.EntityFrameworkCore;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;

namespace SocialNetworkForPets.Services
{
    public class ModeratorService: IModeratorService
    {
         private readonly AppDbContext _context;
        private readonly IPostService _postService;
        private readonly IHashtagService _hashtagService;

        public ModeratorService(AppDbContext context
                    , IPostService postService
                        , IHashtagService hashtagService)
        {
            _context = context;
            _postService = postService;
            _hashtagService = hashtagService;
        }

        public async Task ApproveReport(int postId)
        {
            var post = await _context.Post.FirstOrDefaultAsync(p => p.PostId == postId);
            if (post != null) 
            {
                var deletedPost = await _postService.RemovePostAsync(postId);
                await _hashtagService.HashtagsInRemovedPostAsync(deletedPost.PostText);
            }
        }

        public async Task RejectReport(int postId)
        {
            var reports = await _context.Report.Where(r => r.PostId == postId).ToListAsync();

            if (reports.Any())
            {
                _context.Report.RemoveRange(reports);
                await _context.SaveChangesAsync();
            }
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
