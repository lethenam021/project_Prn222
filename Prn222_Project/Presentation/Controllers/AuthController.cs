using BusinessLogic.DTOs.Request.Auth;
using BusinessLogic.Interface;
using Hangfire;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;
using Presentation.ViewModel.Data.Emails;
using Presentation.ViewModel.Params.Auth;
using System.Security.Claims;

namespace Presentation.Controllers {
    public class AuthController : Controller {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IBackgroundJobClient _jobClient;

        public AuthController(IAuthService authService, IEmailService emailService, IBackgroundJobClient jobClient) {
            _authService = authService;
            _emailService = emailService;
            _jobClient = jobClient;
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
            try {
                if (!ModelState.IsValid) {
                    return PartialView("~/Views/Auth/Login/_LoginFormPartial", request);
                }

                var user = await _authService.LoginAsync(new LoginRequest {
                    Email = request.Email,
                    Password = request.Password
                });

                var claims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, user!.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim(ClaimTypes.Role, user.Role!.ToString()),
                    new Claim(ClaimTypes.Name, user.Name!),
                    new Claim("AvatarUrl", user.AvatarUrl ?? string.Empty)
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
                var loginForm = await this.RenderViewAsync("~/Views/Auth/Login/_LoginFormPartial", request, true);

                if (loginForm == null) {
                    return StatusCode(500, "Render view error (ModelState)!");
                }
                return Json(new { success = false, html = loginForm });
            } catch (UnauthorizedAccessException ex) {
                ModelState.AddModelError("Password", ex.Message);
                var loginForm = await this.RenderViewAsync("~/Views/Auth/Login/_LoginFormPartial", request, true);

                if (loginForm == null) {
                    return StatusCode(500, "Render view error (ModelState)!");
                }
                return Json(new { success = false, html = loginForm });
            } catch (Exception ex) {
                return StatusCode(500, "Login failed. A system error occurred!");
            }
        }

        [HttpGet]
        public IActionResult Register() {
            return View("Register/Register");
        }

        [HttpPost]
        public async Task<IActionResult> ValidateRegistrationInfo([FromForm] RegisterParams request) {

            try {
                if (!ModelState.IsValid) {
                    var registerForm = await this.RenderViewAsync("~/Views/Auth/Register/_RegisterFormPartial", request, true);
                    if (registerForm == null) {
                        return StatusCode(500, "Render view error (ModelState)!");
                    }
                    return Json(new { success = false, html = registerForm });
                }

                var code = await _authService.RequestRegistrationAsync(new RegisterRequest {
                    Name = request.Name,
                    Email = request.Email,
                    Role = request.Role,
                    Password = request.Password,
                });
                var emailModel = new VerificationEmailViewModel {
                    Code = code
                };

                string htmlBody = await this.RenderViewAsync(
            "~/Views/Shared/EmailTemplates/_VerificationEmail",
            emailModel,
            true
        );

                _jobClient.Enqueue<IEmailService>(
                   service => service.SendEmailAsync(request.Email!, "Verify Your Account", htmlBody)
               );

                return Json(new { success = true });
            } catch (InvalidOperationException ex) {
                ModelState.AddModelError("Email", ex.Message);
                var registerForm = await this.RenderViewAsync("~/Views/Auth/Register/_RegisterFormPartial", request, true);
                if (registerForm == null) {
                    return StatusCode(500, "Render view error (ModelState)!");
                }
                return Json(new { success = false, html = registerForm });
            } catch (Exception ex) {
                return StatusCode(500, "Registration failed. A system error occurred!");
            }
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmailCode([FromForm] VerifyCodeParams request) {
            try {
                if (!ModelState.IsValid) {
                    return PartialView("~/Views/Auth/Register/_VerifyCodePartial", request);
                }
                await _authService.ConfirmRegistrationAsync(request.Email!, request.VerificationCode!);
                return Json(new { success = true });
            } catch (InvalidOperationException ex) {
                ModelState.AddModelError("VerificationCode", ex.Message);
                var verifyForm = await this.RenderViewAsync("~/Views/Auth/Register/_VerifyCodePartial", request, true);
                if (verifyForm == null) {
                    return StatusCode(500, "Render view error (ModelState)!");
                }
                return Json(new { success = false, html = verifyForm });
            } catch (ApplicationException ex) {
                return StatusCode(400, ex.Message);
            } catch (Exception ex) {
                return StatusCode(500, "Verification failed. A system error occurred!");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }
    }
}
