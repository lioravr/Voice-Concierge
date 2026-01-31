namespace VoiceConcierge.Core.Domain.Entities;

/// <summary>
/// Represents a user in the system
/// </summary>
public class User
{
    public Guid Id { get; set; }
    
    public string Username { get; set; } = string.Empty;
    
    public string PasswordHash { get; set; } = string.Empty;
    
    public string Role { get; set; } = "Guest";
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? LastLoginAt { get; set; }
}
