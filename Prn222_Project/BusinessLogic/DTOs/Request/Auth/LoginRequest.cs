using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Request.Auth {
    public class LoginRequest {
        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Email is invalid!")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
