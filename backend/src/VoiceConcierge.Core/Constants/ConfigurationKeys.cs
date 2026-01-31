namespace VoiceConcierge.Core.Constants;

/// <summary>
/// User role constants for authorization
/// </summary>
public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Guest = "Guest";
}

/// <summary>
/// Centralized configuration keys used throughout the application.
/// Prevents magic strings and provides compile-time safety for configuration access.
/// </summary>
public static class ConfigurationKeys
{
    /// <summary>
    /// LiveKit service configuration keys
    /// </summary>
    public static class LiveKit
    {
        public const string Url = "LiveKit:Url";
        public const string ApiKey = "LiveKit:ApiKey";
        public const string ApiSecret = "LiveKit:ApiSecret";
        public const string DefaultRoomName = "LiveKit:DefaultRoomName";
        public const string TokenExpirationHours = "LiveKit:TokenExpirationHours";
    }

    /// <summary>
    /// JWT authentication configuration keys
    /// </summary>
    public static class Jwt
    {
        public const string Key = "Jwt:Key";
        public const string Issuer = "Jwt:Issuer";
        public const string Audience = "Jwt:Audience";
    }

    /// <summary>
    /// OpenAI service configuration keys
    /// </summary>
    public static class OpenAI
    {
        public const string ApiKey = "OpenAI:ApiKey";
    }

    /// <summary>
    /// Database connection string keys (use with configuration.GetConnectionString())
    /// </summary>
    public static class ConnectionStrings
    {
        /// <summary>
        /// Use with configuration.GetConnectionString(ConfigurationKeys.ConnectionStrings.DefaultConnection)
        /// The GetConnectionString method automatically prepends "ConnectionStrings:" prefix
        /// </summary>
        public const string DefaultConnection = "DefaultConnection";
    }
}
