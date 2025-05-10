using System.ComponentModel.DataAnnotations;

namespace SocialNetworkForPets.ViewModels.Authentication
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Username is between 2-50 characters")]
        [RegularExpression(@"^[a-z0-9@._\-]+$", ErrorMessage = "Username contains only numbers, lowercase letters and ('@' '.' '_' '-') symbols")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
