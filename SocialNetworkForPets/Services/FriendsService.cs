using Microsoft.EntityFrameworkCore;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;

namespace SocialNetworkForPets.Services
{
    public class FriendsService : IFriendsService
    {
        private readonly AppDbContext _context;

        public FriendsService(AppDbContext context)
        {
            _context = context;
        }
        public async Task SendRequestAsync(int senderId, int receiverId)
        {
            var request = new FriendshipRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId
            };
            _context.FriendRequests.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task AcceptRequestAsync(int senderId, int receiverId)
        {
            var request = await _context.FriendRequests.FirstAsync(r => r.SenderId == senderId && r.ReceiverId == receiverId);

            if (request != null)
            {
                var newFriendship = new Friendship
                {
                    User1Id = senderId,
                    User2Id = receiverId,
                };

                _context.FriendRequests.Remove(request);

                await _context.Friendship.AddAsync(newFriendship);

                await _context.SaveChangesAsync();
            }

        }

        public async Task RejectRequestAsync(int senderId, int receiverId)
        {
            var request = await _context.FriendRequests.FirstAsync(r => r.SenderId == senderId && r.ReceiverId == receiverId);

            _context.FriendRequests.Remove(request);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFriendAsync(int userId, int friendId)
        {
            var friendship = await _context.Friendship.FirstOrDefaultAsync
                (f => (f.User1Id == userId && f.User2Id == friendId) || (f.User1Id == friendId && f.User2Id == userId));

            _context.Friendship.Remove(friendship);

            await _context.SaveChangesAsync();
        }

        public async Task<List<(User,int,bool)>> GetSuggestedPetsAsync(int UserId)
        {
            var user = await _context.User.FindAsync(UserId);

            var suggestedPets = new List<(User, int ,bool)>();

            var top5Users = _context.User
                .Select(u => new
                {
                    User = u,
                    FriendCount = _context.Friendship.Count(f => f.User1Id == u.UserId || f.User2Id == u.UserId)
                })
                .OrderByDescending(u => u.FriendCount)
                .Take(5)
                .ToList();

            foreach (var element in top5Users)
                suggestedPets.Add((element.User, element.FriendCount, IsFriend(UserId, element.User.UserId)));

            return suggestedPets;
        }

        public async Task<List<FriendshipRequest>> GetSentFriendRequestAsync(int userId)
        {
            var friendRequestsSent = await _context.FriendRequests
                .Include(u => u.Receiver)
                .Include(u => u.Sender)
                .Where(f => f.SenderId == userId).ToListAsync();

            return friendRequestsSent;
        }

        public async Task<List<FriendshipRequest>> GetReceivedFriendRequestAsync(int userId)
        {
            var friendRequestsSent = await _context.FriendRequests
                .Include(u => u.Sender)
                .Include(u => u.Receiver)
                .Where(f => f.ReceiverId == userId).ToListAsync();

            return friendRequestsSent;
        }

        public async Task<List<Friendship>> GetFriendsAsync(int userId)
        {
            var friends = await _context.Friendship
                .Include(n => n.User1)
                .Include(n => n.User2)
                .Where(n => n.User1Id == userId || n.User2Id == userId)
                .ToListAsync();

            return friends;
            
        }
        private bool IsFriend(int User1Id, int User2Id)
        {
            return (User1Id == User2Id) || (_context.Friendship.Any(u => (u.User1Id == User1Id && u.User2Id == User2Id)
                                        || (u.User1Id == User2Id && u.User2Id == User1Id)))
                                        || (_context.FriendRequests
                                    .Any(u => (u.SenderId == User1Id && u.ReceiverId == User2Id)
                                        || (u.SenderId == User2Id && u.ReceiverId == User1Id)));

        }
    }
}
