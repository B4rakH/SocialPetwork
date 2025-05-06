using Microsoft.EntityFrameworkCore;
using SocialNetworkForPets.Data.Models;

namespace SocialNetworkForPets.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<User> User { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Pet> Pet { get; set; }

        //public DbSet<PetOwner> PetOwner { get; set; }

        //public DbSet<Administrator> Admin { get; set; }
        public DbSet<Comment> Comment { get; set; }

        public DbSet<Friendship> Friendship { get; set; }

        public DbSet <Favorite> Favorite { get; set; }
        public DbSet <Like> Like { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            
            //Friendship Key Adding & Relations
            modelBuilder.Entity<Friendship>()
                .HasKey(f => new { f.User1Id, f.User2Id });

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User1)
                .WithMany()
                .HasForeignKey(f => f.User1Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User2)
                .WithMany()
                .HasForeignKey(f => f.User2Id)
                .OnDelete(DeleteBehavior.ClientNoAction);

            ////Admin Adding Key
            //modelBuilder.Entity<Administrator>()
            //    .HasKey(ad => new { ad.AdminId });

            ////PetOwner Adding Key
            //modelBuilder.Entity<PetOwner>()
            //    .HasKey(po => new { po.OwnerId });

            //User-Post Relation
            modelBuilder.Entity<User>()
                .HasMany(pr => pr.Posts)
                .WithOne(p => p.Poster)
                .HasForeignKey(p => p.PosterId)
                .OnDelete(DeleteBehavior.Cascade);

            //User-Pet Relation
            modelBuilder.Entity<User>()
                .HasMany(p => p.PetsOwned)
                .WithOne(pt => pt.Owner)
                .HasForeignKey(pt => pt.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            //User-Comment Relation
            modelBuilder.Entity<User>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //Post-Comment Relation
            modelBuilder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            

            //Like Relations
            modelBuilder.Entity<Like>()
                .HasKey(l => new { l.PostId, l.UserId });

            modelBuilder.Entity<Like>()
                .HasOne(p => p.User)
                .WithMany(p => p.Likes)
                .HasForeignKey(u => u.UserId);

            modelBuilder.Entity<Like>()
                .HasOne(p => p.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(u => u.PostId);

            //Favorite Relations
            modelBuilder.Entity<Favorite>()
                .HasKey(l => new { l.PostId, l.UserId });

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Post)
                .WithMany(p => p.Favorites)
                .HasForeignKey(f => f.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
