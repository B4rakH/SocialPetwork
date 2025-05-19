using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;
using SocialNetworkForPets.Helper.Constants;

namespace SocialNetworkForPets.Helper
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context) 
        {
            //Creates post and user in the beginning of empty dataset
            if(!context.User.Any() && !context.Post.Any()) 
            {
                var newUser = new User()
                {
                    UserFullName = "Garfield",
                    UserName = "garfield@admin",
                    UserPassword = "monday",
                    UserRank = UserRank.Admin
                };

                var newPost = new Post()
                {
                    PostText = "Get me a lasagna John, with tea.",
                    CreatedAt = DateTime.UtcNow,
                    PostImgUrl = "/images/uploaded/post/2c26b4ee-1784-4b89-93f1-8e77b4f88177.jpg",
                    Poster = newUser
                };

                await context.User.AddAsync(newUser);

                await context.Post.AddRangeAsync(newPost);

                await context.SaveChangesAsync();
            }
        }
    }
}
