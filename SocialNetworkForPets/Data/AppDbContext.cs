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

        public DbSet<Report> Report { get; set; }

        public DbSet<Comment> Comment { get; set; }

        public DbSet<Friendship> Friendship { get; set; }

        public DbSet <FriendshipRequest> FriendRequests  { get; set; }

        public DbSet <Favorite> Favorite { get; set; }

        public DbSet <Like> Like { get; set; }

        public DbSet<Hashtag> Hashtag { get; set; }

        public DbSet<Notification> Notification { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            //User-Post Relation
            modelBuilder.Entity<User>()
                .HasMany(pr => pr.Posts)
                .WithOne(p => p.Poster)
                .HasForeignKey(p => p.PosterId)
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
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Like>()
                .HasOne(p => p.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(u => u.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            //Favorite Relations
            modelBuilder.Entity<Favorite>()
                .HasKey(f => new { f.PostId, f.UserId });

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

            //Report Relations
            modelBuilder.Entity<Report>()
                .HasKey(f => new { f.PostId, f.UserId });

            modelBuilder.Entity<Report>()
                .HasOne(f => f.Post)
                .WithMany(p => p.Reports)
                .HasForeignKey(f => f.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Report>()
                .HasOne(f => f.User)
                .WithMany(u => u.Reports)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //Friendship Relations
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
                .OnDelete(DeleteBehavior.Restrict);

            //FriendshipRequest Relations
            modelBuilder.Entity<FriendshipRequest>()
                 .HasKey(r => new { r.SenderId, r.ReceiverId });


            modelBuilder.Entity<FriendshipRequest>()
                .HasOne(r => r.Sender)
                .WithMany()
                .HasForeignKey(r => r.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<FriendshipRequest>()
                .HasOne(r => r.Receiver)
                .WithMany()
                .HasForeignKey(r => r.ReceiverId)
                .OnDelete(DeleteBehavior.Cascade);

            //Notification Relations
            modelBuilder.Entity<Notification>()
                .HasKey(o => o.Id);

            modelBuilder.Entity<Notification>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(p => p.Post)
                .WithMany()
                .HasForeignKey(u => u.PostId)
                .OnDelete(DeleteBehavior.Cascade);



            base.OnModelCreating(modelBuilder);
        }
    }
}
