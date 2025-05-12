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
                User1Id = senderId,
                User2Id = receiverId
            };
            _context.FriendRequests.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task AcceptRequestAsync(int requestId)
        {
            var request = await _context.FriendRequests.FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (request != null)
            {
                var newFriendship = new Friendship
                {
                    User1Id = request.User1Id,
                    User2Id = request.User2Id,
                };

                _context.FriendRequests.Remove(request);

                var user1 = await _context.User.FindAsync(request.User1Id);

                var user2 = await _context.User.FindAsync(request.User2Id);

                user1.Friends.Add(user2);

                user2.Friends.Add(user1);

                _context.User.Update(user1);

                _context.User.Update(user2);

                await _context.Friendship.AddAsync(newFriendship);

                await _context.SaveChangesAsync();
            }

        }

        public async Task RejectRequestAsync(int requestId)
        {
            var request = await _context.FriendRequests.FirstOrDefaultAsync(r => r.RequestId == requestId);

            _context.FriendRequests.Remove(request);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFriendAsync(int friendshipId)
        {
            var friendship = await _context.Friendship.FirstOrDefaultAsync(f => f.Id == friendshipId);

            var user1 = await _context.User.FindAsync(friendship.User1Id);

            var user2 = await _context.User.FindAsync(friendship.User2Id);

            user1.Friends.Remove(user2);

            user2.Friends.Remove(user1);

            _context.Friendship.Remove(friendship);

            await _context.SaveChangesAsync();
        }

        public async Task<List<(User, bool)>> GetSuggestedPetsAsync(int UserId)
        {
            var user = await _context.User.FindAsync(UserId);

            var suggestedPets = new List<(User, bool)>();

            foreach (var pet in _context.User.OrderBy(u => u.Friends.Count).Take(5).ToList())

                suggestedPets.Add((pet, (pet.UserId != UserId)
                            && (!user.Friends.Contains(pet))
                                && (_context.FriendRequests
                                    .FirstOrDefault(u => (u.User1Id == pet.UserId && u.User2Id == UserId)
                                        || (u.User1Id == UserId && u.User2Id == pet.UserId)) == null)));

            return suggestedPets;
        }

        public async Task<List<FriendshipRequest>> GetSentFriendRequestAsync(int userId)
        {
            var friendRequestsSent = await _context.FriendRequests
                .Include(u => u.User1)
                .Include(u => u.User2)
                .Where(f => f.User1Id == userId).ToListAsync();

            return friendRequestsSent;
        }

        public async Task<List<FriendshipRequest>> GetReceivedFriendRequestAsync(int userId)
        {
            var friendRequestsSent = await _context.FriendRequests
                .Include(u => u.User1)
                .Include(u => u.User2)
                .Where(f => f.User2Id == userId).ToListAsync();

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
    }
}
