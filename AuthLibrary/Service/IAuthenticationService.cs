using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Claims;

namespace Cority.AuthLibrary.Service
{
    /// <summary>
    /// Interface for authentication service operations.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Generates a secure refresh token string.
        /// </summary>
        string GenerateRefreshTokenString();

        /// <summary>
        /// Gets the principal from a JWT token using the provided private key path.
        /// </summary>
        ClaimsPrincipal GetTokenPrincipal(string token, string privateKeyPath);

        /// <summary>
        /// Generates a JWT token from claims and a private key path.
        /// </summary>
        string GenerateToken(IEnumerable<Claim> claims, string privateKeyPath);

        /// <summary>
        /// Gets the RSA security key from a private key path.
        /// </summary>
        RsaSecurityKey GetSecurityKey(string privateKeyPath);
    }
}
