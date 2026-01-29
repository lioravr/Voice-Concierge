using VoiceConcierge.Core.DTOs;

namespace VoiceConcierge.Core.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
    Task<UserDto?> GetCurrentUserAsync(Guid userId);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
