using TestApp.Data;
using TestApp.Data.Models;

namespace SocialNetworkForPets.Helper
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context) 
        {
            if(!context.User.Any() && !context.Post.Any()) 
            {
                var person = new User();
            }
        }
    }
}
