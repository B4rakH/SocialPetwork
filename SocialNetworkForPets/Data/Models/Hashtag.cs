using System.ComponentModel.DataAnnotations;

namespace SocialNetworkForPets.Data.Models
{
    public class Hashtag
    {

        [Key]
        public int HashtagId { get; set; }

        public string TagText { get; set; } //Foreign Key (Uses post text)

        public int TagCount { get; set; } = 0;
    }
}
