namespace VoiceConcierge.Core.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Guest"; // Admin or Guest
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Guest = "Guest";
}
