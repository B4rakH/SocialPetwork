using Microsoft.EntityFrameworkCore;

namespace SocialNetworkForPets.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) 
        {
        
        }
        public DbSet<Models.Post> Posts { get; set; }
    }
}
