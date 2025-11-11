using System.ComponentModel.DataAnnotations;

namespace Presentation.ViewModel.Params.Auth {
    public class VerifyCodeParams {
        [Required]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Verification code is required!")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Invalid verification code!")]
        [MaxLength(6, ErrorMessage = "Invalid verification code!")]
        public required string VerificationCode { get; set; }
    }
}
