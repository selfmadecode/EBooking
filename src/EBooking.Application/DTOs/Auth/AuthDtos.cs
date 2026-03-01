namespace EBooking.Application.DTOs.Auth;

public record RegisterDto
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
}

public record AuthResponseDto
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = null!;
    public List<string> Roles { get; set; } = [];
}

public record UserDto
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
}

public record LoginDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
