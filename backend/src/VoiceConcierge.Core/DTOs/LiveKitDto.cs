namespace VoiceConcierge.Core.DTOs;

/// <summary>
/// Request model for generating LiveKit access tokens
/// </summary>
public record TokenRequest
{
    /// <summary>
    /// User identity for the LiveKit session (auto-generated if not provided)
    /// </summary>
    public string? Identity { get; init; }
    
    /// <summary>
    /// Room name to join (uses default if not provided)
    /// </summary>
    public string? RoomName { get; init; }
    
    /// <summary>
    /// Voice configuration preference ID
    /// </summary>
    public int? VoicePreference { get; init; }
}
