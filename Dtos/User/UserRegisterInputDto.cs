using System.ComponentModel.DataAnnotations;

namespace Rihal_Cinema.Dtos.User
{
    public class UserRegisterInputDto
    {
        [Required(ErrorMessage = "Email is required")]
        [MaxLength(50, ErrorMessage = "Maximum length is 50 characters")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MaxLength(20, ErrorMessage = "Maximum length is 20 characters")]
        public string Password { get; set; }
    }
}
