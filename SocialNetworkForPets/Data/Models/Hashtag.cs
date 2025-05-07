using System.ComponentModel.DataAnnotations;

namespace SocialNetworkForPets.Data.Models
{
    public class Hashtag
    {

        [Key]
        public int HashtagId { get; set; }

        public string TagText { get; set; }

        public int TagCount { get; set; } = 0;
    }
}
