using System.ComponentModel.DataAnnotations;

namespace Presentation.ViewModel.Params.Auth {
    public class LoginParams {
        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Invalid email. Please try again!")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        public bool RememberMe { get; set; } = false;
    }
}
