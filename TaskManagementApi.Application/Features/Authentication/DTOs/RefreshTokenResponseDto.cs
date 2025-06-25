namespace TaskManagementApi.Application.Features.Authentication.DTOs
{
    /// <summary>
    /// Represents the response for Refresh token Response
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="Token"></param>
    /// <param name="Expires"></param>
    /// <param name="IsExpired"></param>
    /// <param name="Created"></param>
    /// <param name="CreatedByIp"></param>
    /// <param name="Revoked"></param>
    /// <param name="RevokedByIp"></param>
    /// <param name="IsActive"></param>
    public record RefreshTokenResponseDto(
    string Id,
    string Token, //this includes token
    DateTime Expires,
    bool IsExpired,
    DateTime Created,
    string CreatedByIp,
    DateTime? Revoked,
    string? RevokedByIp,
    bool IsActive
    )
    {
        public RefreshTokenResponseDto(string id, string token, DateTime expires, DateTime created, string createdByIp, DateTime? revoked, string? revokedByIp, bool isActive)
        {
            Id = id;
            Token = token;
            Expires = expires;
            Created = created;
            CreatedByIp = createdByIp;
            Revoked = revoked;
            RevokedByIp = revokedByIp;
            IsActive = isActive;
        }
    }

    /// <summary>
    /// Represents a response for Refresh tokens but with no value token with it
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="Expires"></param>
    /// <param name="IsExpired"></param>
    /// <param name="Created"></param>
    /// <param name="CreatedByIp"></param>
    /// <param name="Revoked"></param>
    /// <param name="RevokedByIp"></param>
    /// <param name="IsActive"></param>
    public record RefreshTokenResponseNoTokenDto( 
    string Id,
    DateTime Expires,
    bool IsExpired,
    DateTime Created,
    string CreatedByIp,
    DateTime? Revoked,
    string? RevokedByIp,
    bool IsActive
    );
}
