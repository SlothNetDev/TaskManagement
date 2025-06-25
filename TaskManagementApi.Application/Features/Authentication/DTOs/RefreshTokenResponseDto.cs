﻿namespace TaskManagementApi.Application.Features.Authentication.DTOs
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
    );
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
