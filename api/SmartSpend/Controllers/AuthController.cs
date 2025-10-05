using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSpend.Dtos;
using SmartSpend.Helper;
using SmartSpend.Models;
using SmartSpend.Services;

namespace SmartSpend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        // ✅ Constructor for DI
        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                UserName = model.UserName == null ? model.UserName : model.Email,
                Email = model.Email
            };

            var result = await _userService.RegisterUserAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return ApiResponse.Success(result);
        }

        /// <summary>
        /// Logs in a user
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto model)
        {
            try
            {
                var response = await _userService.LoginUserAsync(model.Email, model.Password);
                return ApiResponse.Success(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized login attempt");
                return ApiResponse.Unauthorized("Invalid username or password");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login");
                return ApiResponse.BadRequest("An error occurred while logging in");
            }
        }

        /// <summary>
        /// Refreshes the access token
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto model)
        {
            try
            {
                var response = await _userService.RefreshAccessTokenAsync(model.RefreshToken);
                return ApiResponse.Success(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Invalid refresh token");
                return ApiResponse.Unauthorized("Invalid refresh token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while refreshing token");
                return ApiResponse.BadRequest("An error occurred while refreshing token");
            }
        }

        /// <summary>
        /// Logs out a user
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await _userService.LogoutUserAsync(userId);
            return ApiResponse.Success("User logged out successfully.");
        }
    }
}
