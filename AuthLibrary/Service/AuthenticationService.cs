using Cority.AuthLibrary.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Cority.AuthLibrary.Service
{
    /// <summary>
    /// Options for configuring JWT token generation and validation.
    /// </summary>
    public class JwtOptions
    {
        public string Issuer { get; set; } = "CorityAuthentication";
        public int ExpirationMinutes { get; set; } = 30;
        public string PrivateKeyPath { get; set; } = string.Empty;
    }

    /// <summary>
    /// Provides authentication services for JWT tokens.
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly JwtOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
        /// </summary>
        /// <param name="options">The JWT options to configure the service.</param>
        public AuthenticationService(JwtOptions options)
        {
            _options = options;
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
        public string GenerateToken(IEnumerable<Claim> claims, string privateKeyPath = null)
        {
            if (claims == null)
                throw new TokenException("Claims are not set");

            var keyPath = privateKeyPath ?? _options.PrivateKeyPath;
            var rsaSecurityKey = GetSecurityKey(keyPath);
            var signingCred = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);
            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes),
                signingCredentials: signingCred,
                issuer: _options.Issuer
            );
            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        /// <inheritdoc/>
        public RsaSecurityKey GetSecurityKey(string keyPath = null)
        {
            var path = keyPath ?? _options.PrivateKeyPath;
            if (string.IsNullOrWhiteSpace(path))
                throw new SecurityKeyException("Private key path is not set");

            var rsaKey = RSA.Create();
            try
            {
                var xmlKey = File.ReadAllText(path);
                rsaKey.FromXmlString(xmlKey);
            }
            catch (CryptographicException ex)
            {
                throw new SecurityKeyException("Invalid XML file: " + ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                throw new SecurityKeyException("File not found: " + ex.Message);
            }
            return new RsaSecurityKey(rsaKey);
        }

        /// <inheritdoc/>
        public ClaimsPrincipal GetTokenPrincipal(string token, string privateKeyPath = null)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token is empty", nameof(token));

            var keyPath = privateKeyPath ?? _options.PrivateKeyPath;
            var securityKey = GetSecurityKey(keyPath);
            var validation = new TokenValidationParameters
            {
                IssuerSigningKey = securityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateActor = false,
                ValidateLifetime = false
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
