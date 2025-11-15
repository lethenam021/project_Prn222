using Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Presentation.ViewModel.Params.Auth {
    public class RegisterParams : IValidatableObject {
        [Required(ErrorMessage = "Name is required!")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Invalid email. Please try again!")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Role is required!")]
        public UserRole Role { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_])(?!.*\s).{6,}$",
    ErrorMessage = "Password must be at least 6 characters long, contain uppercase, lowercase, a special character, and no spaces!")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required!")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_])(?!.*\s).{6,}$",
    ErrorMessage = "Password must be at least 6 characters long, contain uppercase, lowercase, a special character, and no spaces!")]
        public string? ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrWhiteSpace(Password) && Password != ConfirmPassword) {
                yield return new ValidationResult(
                    "Confirm password does not match new password!",
                    new[] { nameof(ConfirmPassword) }
                );
            }
        }
    }
}
