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
    public class AuthenticationService : IAuthenticationService
    {
        public string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[64];

            using (var numberGenerator = RandomNumberGenerator.Create())
            {
                numberGenerator.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }

        public string GenerateToken(IEnumerable<Claim> claims, string privateKeyPath)
        {
            if(claims == null)
                throw new TokenException("Claims are not set");

            var rsaSecurityKey = GetSecurityKey(privateKeyPath);

            var signingCred = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signingCred,
                issuer: "CorityAuthentication"
                );

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;
        }

        public ClaimsPrincipal GetClaims(string token, string privateKeyPath)
        {
            return GetTokenPrincipal(token, privateKeyPath);
        }

        public RsaSecurityKey GetSecurityKey(string keyPath)
        {
            var rsaKey = RSA.Create();

            if (string.IsNullOrWhiteSpace(keyPath))
                throw new SecurityKeyException("Private key path is not set");

            try
            {
                var xmlKey = File.ReadAllText(keyPath);
                rsaKey.FromXmlString(xmlKey);
            } catch (CryptographicException ex)
            {
                throw new SecurityKeyException("Invalid XML file", ex);
            } catch (FileNotFoundException ex)
            {
                throw new SecurityKeyException("File not found", ex);
            }


            var rsaSecurityKey = new RsaSecurityKey(rsaKey);
            return rsaSecurityKey;
        }

        public ClaimsPrincipal GetTokenPrincipal(string token, string privateKeyPath)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException(nameof(token));

            var securityKey = GetSecurityKey(privateKeyPath);

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
