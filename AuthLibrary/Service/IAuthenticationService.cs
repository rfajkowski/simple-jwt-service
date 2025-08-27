using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Claims;

namespace Cority.AuthLibrary.Service
{
    public interface IAuthenticationService
    {
        string GenerateRefreshTokenString();
        ClaimsPrincipal GetTokenPrincipal(string token, string privateKeyPath);
        ClaimsPrincipal GetClaims(string token, string privateKeyPath);
        string GenerateToken(IEnumerable<Claim> claims, string privateKeyPath);
        RsaSecurityKey GetSecurityKey(string privateKeyPath);

    }
}
