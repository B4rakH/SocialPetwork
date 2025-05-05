using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;

namespace SocialNetworkForPets.Helper
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context) 
        {
            if(!context.User.Any() && !context.Post.Any()) 
            {
                var newUser = new User()
                {
                    UserName = "Hugo Martin",
                    UserImgUrl = "",
                    UserPassword = ""
                };
                await context.User.AddAsync(newUser);
                await context.SaveChangesAsync();

                var newPostNoImg = new Post()
                {
                    PostText = "Which brand of cat food has the best quality?",
                    CreatedAt = DateTime.UtcNow,

                    Poster = newUser
                };

                await context.Post.AddRangeAsync(newPostNoImg);
                await context.SaveChangesAsync();
            }
        }
    }
}
