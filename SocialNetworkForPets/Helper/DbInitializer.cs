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
                    UserFullName = "Garfield",
                    UserName = "garfield_cat",
                    UserImgUrl = "",
                    UserPassword = "",
                };
                await context.User.AddAsync(newUser);
                await context.SaveChangesAsync();

                var newPostNoImg = new Post()
                {
                    PostText = "Where is my lasagna, John?",
                    CreatedAt = DateTime.UtcNow,

                    Poster = newUser
                };

                await context.Post.AddRangeAsync(newPostNoImg);
                await context.SaveChangesAsync();
            }
        }
    }
}
