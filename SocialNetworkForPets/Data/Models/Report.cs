namespace SocialNetworkForPets.Data.Models
{
    public class Report
    {
        public int ReportId { get; set; }

        public int PostId { get; set; }

        public int UserId { get; set; }

        public Post Post { get; set; }

        public User User { get; set; }
    }
}
