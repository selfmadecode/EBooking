using EBooking.Application.Common;
using EBooking.Application.DTOs.Auth;
using EBooking.Application.Interfaces;
using EBooking.Domain.Entities;
using EBooking.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EBooking.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public AuthService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork,
        IConfiguration configuration, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _roleManager = roleManager;
    }

    public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto)
    {
        if (dto.Password != dto.ConfirmPassword)
            return ApiResponse<AuthResponseDto>.FailureResponse("Passwords do not match.");

        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return ApiResponse<AuthResponseDto>.FailureResponse("Email already in use.");

        var user = new ApplicationUser
        {
            FullName = dto.FullName,
            Email = dto.Email,
            UserName = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return ApiResponse<AuthResponseDto>.FailureResponse("Registration failed.", result.Errors.Select(e => e.Description).ToList());

        if (!await _roleManager.RoleExistsAsync(RoleHelper.User))
            await _roleManager.CreateAsync(new ApplicationRole { Name = RoleHelper.User });

        await _userManager.AddToRoleAsync(user, RoleHelper.User);

        // Create wallet for the user
        var wallet = new Wallet { UserId = user.Id };
        await _unitOfWork.Wallets.AddAsync(wallet);
        await _unitOfWork.SaveChangesAsync();

        var authResponse = GenerateJwtToken(user, [RoleHelper.User]);
        return ApiResponse<AuthResponseDto>.SuccessResponse(authResponse, "Registration successful.");
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return ApiResponse<AuthResponseDto>.FailureResponse("Invalid email or password.");

        var validPassword = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!validPassword)
            return ApiResponse<AuthResponseDto>.FailureResponse("Invalid email or password.");

        var userRoles = await _userManager.GetRolesAsync(user);
        var authResponse = GenerateJwtToken(user, userRoles);
        authResponse.Roles = userRoles.ToList();

        return ApiResponse<AuthResponseDto>.SuccessResponse(authResponse, "Login successful.");
    }

    private AuthResponseDto GenerateJwtToken(ApplicationUser user, IList<string> userRoles)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpiryHours"] ?? "24"));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.GivenName, user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = Guid.NewGuid().ToString(),
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!
            }
        };
    }
}
