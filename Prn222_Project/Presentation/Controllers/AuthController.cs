using BusinessLogic.DTOs.Request.Auth;
using BusinessLogic.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;
using Presentation.ViewModel.Params.Auth;
using System.Security.Claims;
using System.Text.Json;

namespace Presentation.Controllers {
    public class AuthController : Controller {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login() {
            if (User.Identity != null && User.Identity.IsAuthenticated) {
                return RedirectToAction("Index", "Home");
            }

            return View("Login/Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginParams request) {

            if (!ModelState.IsValid) {
                return PartialView("Login/_LoginFormPartial", request);
            }

            try {
                var user = await _authService.LoginAsync(new LoginRequest {
                    Email = request.Email,
                    Password = request.Password
                });

                var claims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, user!.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim(ClaimTypes.Role, user.Role!.ToString()),
                    new Claim(ClaimTypes.Name, user.Name!),
                    new Claim("AvatarUrl", user.AvatarUrl!)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);


                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties {
                        IsPersistent = request.RememberMe,
                        ExpiresUtc = request.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : null
                    }
                );

                return Json(new { success = true, redirectUrl = "/" });
            } catch (InvalidOperationException ex) {
                ModelState.AddModelError("Email", ex.Message);
                var loginForm = await this.RenderViewAsync("Login/_LoginFormPartial", request, true);
                return Json(new { success = false, html = loginForm });
            } catch (UnauthorizedAccessException ex) {
                ModelState.AddModelError("Password", ex.Message);
                var loginForm = await this.RenderViewAsync("Login/_LoginFormPartial", request, true);
                return Json(new { success = false, html = loginForm });
            } catch (Exception ex) {
                return StatusCode(500, "Authentication failed. A system error occurred!");
            }
        }

        [HttpGet]
        public IActionResult Register() {
            return View("Register/Register");
        }

        [HttpGet]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }
    }
}
