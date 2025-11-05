using BusinessLogic.DTOs.Request.Auth;
using BusinessLogic.DTOs.Response.Auth;
using BusinessLogic.Interface;
using BusinessLogic.Validation;
using Common.Enums;
using DataAccess.IRepo;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services {
    public class AuthService : IAuthService {
        private readonly IUserRepo _repo;
        private readonly AuthValidator _validator;

        public AuthService(IUserRepo repo, AuthValidator validator) {
            _repo = repo;
            _validator = validator;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest) {
            await _validator.ValidateForLoggingIn(loginRequest);

            var account = await _repo.GetUserByEmailAsync(loginRequest.Email);

            return new LoginResponse() {
                Id = account!.Id,
                Email = account.Email,
                Name = account.Username,
                Role = Enum.Parse<UserRole>(account.Role!, ignoreCase: true),
                AvatarUrl = account.AvatarUrl
            };
        }
    }
}
