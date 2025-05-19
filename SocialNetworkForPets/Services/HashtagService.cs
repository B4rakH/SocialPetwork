
using Microsoft.EntityFrameworkCore;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;
using SocialNetworkForPets.Helper;

namespace SocialNetworkForPets.Services
{
    public class HashtagService : IHashtagService
    {
        private readonly AppDbContext _context;
        public HashtagService(AppDbContext context) 
        {
            _context = context;
        }
        public async Task HashtagsInNewPostAsync(string PostText)
        {
            //Finding and storing tags
            var postHashTags = HashtagHelper.GetHashtags(PostText);
            foreach (var tag in postHashTags)
            {
                var hashtagDb = await _context.Hashtag.FirstOrDefaultAsync(t => t.TagText == tag);
                if (hashtagDb != null)
                {
                    hashtagDb.TagCount++;
                }
                else
                {
                    var newHashtag = new Hashtag()
                    {
                        TagText = tag,
                        TagCount = 1
                    };
                    await _context.Hashtag.AddAsync(newHashtag);
                }
            }
                await _context.SaveChangesAsync();
        }

        public async Task HashtagsInRemovedPostAsync(string postText)
        {
            //If post have hashtags, decrease count or delete
            var postHashtags = HashtagHelper.GetHashtags(postText);

            foreach (var hashtag in postHashtags)
            {
                var hashtagDb = await _context.Hashtag
                    .FirstOrDefaultAsync(h => h.TagText == hashtag);

                if (hashtagDb != null)
                {
                    if (hashtagDb.TagCount <= 1)
                    {
                        _context.Hashtag.Remove(hashtagDb);
                    }
                    else
                    {
                        hashtagDb.TagCount--;
                        _context.Hashtag.Update(hashtagDb);
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
        public async Task<List<Hashtag>> GetTrendTopics()
        {
            var topTopics = await _context.Hashtag
                .OrderByDescending(tag => tag.TagCount)
                .Take(3)
                .ToListAsync();
            return topTopics;
        }
    }
}
