using System.Text.RegularExpressions;

namespace SocialNetworkForPets.Helper
{
    public static class HashtagHelper
    {
        public static List<string> GetHashtags(string postText)
        {
            //Pattern for finding valid tags
            var hashTagPattern = new Regex(@"#\w+");

            //Getting tag values and prevent count multiple times (one per post)
            var hashTags = hashTagPattern.Matches(postText)
                .Select(tag => tag.Value.TrimEnd('.', ',', '!', '?').ToLower())
                .Distinct().ToList();

            return hashTags;
        }
    }
}
