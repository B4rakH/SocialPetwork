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
                    UserName = "Hugo Martin"
                };
                await context.User.AddAsync(newUser);
                await context.SaveChangesAsync();

                var newPostNoImg = new Post()
                {
                    PostText = "Which brand of cat food has the best quality?",
                    PostImgUrl = "",
                    CreatedAt = DateTime.UtcNow,

                    PostId = newUser.UserId
                };
                var newPost = new Post()
                {
                    PostText = "Give to Niyazi another tea!",
                    PostImgUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTsUTK3mAGMy2Y8trSIO45p4NV21YVHQBnrD5A2qrgeGI5fLeiD89jgiPALxsCuw0KU1Ig&usqp=CAU",
                    CreatedAt = DateTime.UtcNow,

                    PostId = newUser.UserId
                };
                await context.Post.AddRangeAsync(newPostNoImg,newPost);
                await context.SaveChangesAsync();
            }
        }
    }
}
