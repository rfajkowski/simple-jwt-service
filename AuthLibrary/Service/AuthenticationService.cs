using System;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using AuthLibrary.Exceptions;
using AuthLibrary.Service;

namespace AuthLibrary.Service
{
    public interface IAuthenticationService
    {
        string GenerateRefreshTokenString();
        string GenerateToken(IEnumerable<Claim> claims, string keyId);
        RsaSecurityKey GetSecurityKey(string keyId);
        ClaimsPrincipal GetTokenPrincipal(string token, string keyId);
    }

    /// <summary>
    /// Options for configuring JWT token generation and validation.
    /// </summary>
    public class JwtOptions
    {
        public string Issuer { get; set; } = "";
        public string Audience { get; set; } = "";
        public int ExpirationMinutes { get; set; } = 30;
        public Dictionary<string, string> KeyPaths { get; set; } = new Dictionary<string, string>(); // keyId -> path
    }

    /// <summary>
    /// Provides authentication services for JWT tokens.
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly JwtOptions _options;
        private readonly IKeyProvider _keyProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
        /// </summary>
        /// <param name="options">The JWT options to configure the service.</param>
        /// <param name="keyProvider">The key provider to retrieve RSA keys.</param>
        public AuthenticationService(JwtOptions options, IKeyProvider keyProvider)
        {
            _options = options;
            _keyProvider = keyProvider;
        }

        /// <inheritdoc/>
        public string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[64];
            using (var numberGenerator = RandomNumberGenerator.Create())
            {
                numberGenerator.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }

        /// <inheritdoc/>
        public string GenerateToken(IEnumerable<Claim> claims, string keyId)
        {
            if (claims == null)
                throw new TokenException("Claims are not set");

            var rsaSecurityKey = GetSecurityKey(keyId);
            var signingCred = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);
            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes),
                signingCredentials: signingCred,
                issuer: _options.Issuer,
                audience: _options.Audience
            );
            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        /// <inheritdoc/>
        public RsaSecurityKey GetSecurityKey(string keyId)
        {
            var rsaKey = _keyProvider.GetRsaKey(keyId);
            return new RsaSecurityKey(rsaKey);
        }

        /// <inheritdoc/>
        public ClaimsPrincipal GetTokenPrincipal(string token, string keyId)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token is empty", nameof(token));

            var securityKey = GetSecurityKey(keyId);
            var validation = new TokenValidationParameters
            {
                IssuerSigningKey = securityKey,
                ValidateIssuer = true,
                ValidIssuer = _options.Issuer,
                ValidateAudience = true,
                ValidAudience = _options.Audience,
                ValidateActor = false,
                ValidateLifetime = true
            };
            try
            {
                var claims = new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
                return claims;
            }
            catch (SecurityTokenMalformedException ex)
            {
                throw new SecurityTokenException("Invalid token", ex);
            }
        }
    }
}
