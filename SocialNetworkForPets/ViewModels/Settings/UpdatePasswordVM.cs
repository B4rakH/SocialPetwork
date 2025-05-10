namespace SocialNetworkForPets.ViewModels.Settings
{
    public class UpdatePasswordVM
    {

        public int UserId { get; set; }

        public string currentPassword { get; set; }

        public string newPassword { get; set; }

        public string confirmPassword { get; set; }

    }
}
