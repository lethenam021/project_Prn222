using BusinessLogic.DTOs.Request.Auth;
using Common.Enums;
using Common.Helpers;
using DataAccess.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Validation {
    public class AuthValidator {
        private IUserRepo _userRepo;

        public AuthValidator(IUserRepo userRepo) {
            _userRepo = userRepo;
        }

        public async Task ValidateForLoggingIn(LoginRequest loginRequest) {
            await this.CheckUserAsync(loginRequest.Email, CheckMode.MustExist);

            var user = await _userRepo.GetUserByEmailAsync(loginRequest.Email);
            if (!PasswordHelper.VerifyPassword(loginRequest.Password, user!.Password!))
                throw new UnauthorizedAccessException("Password is incorrect!");
        }

        public async Task ValidateForRegistering(RegisterRequest registerRequest) {
            await this.CheckUserAsync(registerRequest.Email, CheckMode.MustNotExist);
        }

        private async Task CheckUserAsync(string email, CheckMode mode) {
            var user = await _userRepo.GetUserByEmailAsync(email);

            switch (mode) {
                case CheckMode.MustExist:
                if (user == null)
                    throw new InvalidOperationException("User doesn't exist!");
                break;

                case CheckMode.MustNotExist:
                if (user != null)
                    throw new InvalidOperationException("User has already existed!");
                break;
            }
        }
    }
}
