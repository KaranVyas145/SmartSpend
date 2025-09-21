using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using SmartSpend.Data;
using SmartSpend.Dtos;
using SmartSpend.Models;
using SmartSpend.Helper;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace SmartSpend.Services
{
    public interface IUserService
    {
        public Task<IdentityResult> RegisterUserAsync(User user, string password);
        public Task<UserLoginResponseDto> LoginUserAsync(string email, string password);
        public Task<User> GetUserByEmailAsync(string email);
        public Task<User> GetUserByUsernameAsync(string email);
        public Task<User> GetUserByIdAsync(string userId);
        public Task<IdentityResult> UpdateUserDetailsAsync(User updatedUser);
        public Task<User> GetCurrentUserAsync();
        public Task<bool> IsCurrentUserAdminAsync();
        public Task<RefreshTokenResponseDto> RefreshAccessTokenAsync(string refreshToken);
        public Task LogoutUserAsync(string userId);
        public Task AddToRoleAsync(User user, string role);
    }

    public class UserService: IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserService> _logger;
        private readonly JwtHelper _jwtHelper;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, ILogger<UserService> logger, JwtHelper jwtHelper, SignInManager<User> signInManager, ApplicationDbContext dbContext, IMapper mapper)
        {

            this._userManager = userManager;
            this._logger = logger;
            this._jwtHelper = jwtHelper;
            this._signInManager = signInManager;
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IdentityResult> RegisterUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result;

        }

        public async Task<UserLoginResponseDto> LoginUserAsync(string username, string password)
        {
            // Step 1: Check if the user exists based on the username
            var user = await _userManager.FindByNameAsync(username)
                ?? throw new InvalidOperationException("Invalid credentials"); // Prevents revealing whether username exists

            // Step 2: Attempt to sign in the user with the provided password
            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Invalid credentials"); // Keeps error messages generic for security

            // Step 3: Retrieve user roles
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User"; // Default role if none found

            // Step 4: Generate JWT token
            var token = _jwtHelper.GenerateAccessToken(user.Id, user.UserName, role);
            var refreshToken = _jwtHelper.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);  // Refresh token valid for 7 days
            await _userManager.UpdateAsync(user);

            // Step 5: Return user details along with the token
            return new UserLoginResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Token = token,
                RefreshToken = refreshToken
            };
        }

        public async Task<RefreshTokenResponseDto> RefreshAccessTokenAsync(string refreshToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid or expired refresh token");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            var newAccessToken = _jwtHelper.GenerateAccessToken(user.Id, user.UserName, role);
            var newRefreshToken = _jwtHelper.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new RefreshTokenResponseDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }


        public async Task<User> GetUserByEmailAsync(string email)
        {

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            else
            {
                return user;
            }
        }




        public async Task<User> GetUserByUsernameAsync(string username)
        {

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return null;
            }
            else
            {
                return user;
            }

        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            else
            {
                return user;
            }
        }

        public async Task<IdentityResult> UpdateUserDetailsAsync(User updatedUser)
        {
            var existingUser = await _userManager.FindByIdAsync(updatedUser.Id)
                ?? throw new Exception("User not found");

            _mapper.Map(updatedUser, existingUser);  // Auto-map fields

            return await _userManager.UpdateAsync(existingUser);
        }

        public async Task<User> GetCurrentUserAsync()
        {
            var userClaims = _httpContextAccessor.HttpContext?.User
                ?? throw new UnauthorizedAccessException("No authenticated user found.");

            // Fetch username from claims (since we're using username-based lookup)
            var username = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("Invalid user claims.");

            return await _userManager.FindByNameAsync(username)
                ?? throw new KeyNotFoundException("Authenticated user not found in the system.");
        }


        public async Task<bool> IsCurrentUserAdminAsync()
        {
            var currentUser = await GetCurrentUserAsync();
            return await _userManager.IsInRoleAsync(currentUser, Roles.Admin);
        }

        public async Task LogoutUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Remove refresh token (assuming it's stored in User table)
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;

            await _userManager.UpdateAsync(user);
        }

        public async Task AddToRoleAsync(User user, string role)
        {
            await this._userManager.AddToRoleAsync(user, role);
        }

    }
}
