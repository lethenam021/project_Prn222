using Azure.Core;
using BusinessLogic.DTOs.Request.Auth;
using BusinessLogic.DTOs.Response.Auth;
using BusinessLogic.Interface;
using BusinessLogic.Validation;
using Common.Enums;
using Common.Helpers;
using DataAccess.IRepo;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogic.Services {
    public class AuthService : IAuthService {
        private readonly IUserRepo _repo;
        private readonly IDistributedCache _cache;
        private readonly AuthValidator _validator;

        public AuthService(IUserRepo repo, AuthValidator validator, IDistributedCache cache) {
            _repo = repo;
            _cache = cache;
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

        public async Task<string> RequestRegistrationAsync(RegisterRequest registerRequest) {
            await _validator.ValidateForRegistering(registerRequest);

            var passwordHash = PasswordHelper.HashPassword(registerRequest.Password!);

            string code = new Random().Next(100000, 999999).ToString();

            var pendingData = new {
                Email = registerRequest.Email,
                Name = registerRequest.Name,
                Role = registerRequest.Role.ToString(),
                PasswordHash = passwordHash,
                VerificationCode = code,
                CodeExpiresAtUtc = DateTime.UtcNow.AddMinutes(10)
            };

            string cacheKey = $"PendingReg_{registerRequest.Email}";

            var options = new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3)
            };

            string jsonData = JsonSerializer.Serialize(pendingData);
            await _cache.SetStringAsync(cacheKey, jsonData, options);

            return code;
        }

        public async Task ConfirmRegistrationAsync(string email, string providedCode) {
            string cacheKey = $"PendingReg_{email}";
            var jsonData = await _cache.GetStringAsync(cacheKey);

            if (string.IsNullOrEmpty(jsonData)) {
                throw new ApplicationException("Registration session expired. Please register again!");
            }

            var pendingData = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonData);

            if (pendingData == null) {
                throw new ApplicationException("Temporary data error. Please register again!");
            }

            Console.WriteLine(jsonData);

            if (DateTime.UtcNow > DateTime.Parse(pendingData["CodeExpiresAtUtc"]).ToUniversalTime()) {
                throw new InvalidOperationException("Verification code has expired. Please resend code!");
            }

            string verifyCode = pendingData["VerificationCode"];

            if (verifyCode != providedCode) {
                throw new InvalidOperationException("The verification code is incorrect. Please try again!");
            }

            var newUser = new User {
                Email = pendingData["Email"],
                Password = pendingData["PasswordHash"],
                Username = pendingData["Name"],
                Role = pendingData["Role"]
            };

            await _repo.AddUserAsync(newUser);
            await _cache.RemoveAsync(cacheKey);
        }


    }
}
