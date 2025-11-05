using BusinessLogic.DTOs.Request.Auth;
using BusinessLogic.DTOs.Response.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interface {
    public interface IAuthService {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        //Task RegisterAsync(RegisterRequest request);
    }
}
