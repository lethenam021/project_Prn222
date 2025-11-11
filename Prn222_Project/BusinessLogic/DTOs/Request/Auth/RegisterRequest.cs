using Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Request.Auth {
    public class RegisterRequest {
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

    }
}
