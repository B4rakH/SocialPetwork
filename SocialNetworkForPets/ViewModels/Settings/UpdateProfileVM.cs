using System.ComponentModel.DataAnnotations;

namespace SocialNetworkForPets.ViewModels.Settings
{
    public class UpdateProfileVM
    {
        public int UserId { get; set; }

        public string UserFullName { get; set; }

        public string UserName { get; set; }
    }
}
