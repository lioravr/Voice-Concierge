using System.ComponentModel.DataAnnotations;

namespace VoiceConcierge.Core.DTOs;

/// <summary>
/// Request model for generating LiveKit access tokens
/// </summary>
public record TokenRequest
{
    /// <summary>
    /// User identity for the LiveKit session (auto-generated if not provided)
    /// </summary>
    [StringLength(100, ErrorMessage = "Identity cannot exceed 100 characters")]
    public string? Identity { get; init; }
    
    /// <summary>
    /// Room name to join (uses default if not provided)
    /// </summary>
    [StringLength(100, ErrorMessage = "Room name cannot exceed 100 characters")]
    public string? RoomName { get; init; }
    
    /// <summary>
    /// Voice configuration preference ID (1-4)
    /// </summary>
    [Range(1, 4, ErrorMessage = "Voice preference must be between 1 and 4")]
    public int? VoicePreference { get; init; }
}
