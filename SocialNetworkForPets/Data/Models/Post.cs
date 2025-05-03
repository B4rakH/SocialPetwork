using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetworkForPets.Data.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        public string PostText { get; set; }

        public string? PostImgUrl { get; set; }

        public int PostLikes { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
