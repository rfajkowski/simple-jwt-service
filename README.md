# Simple JWT Service

A lightweight .NET library for generating and validating JWT tokens using RSA keys. Designed for easy integration with ASP.NET Core applications and supports secure authentication workflows.

## Disclaimer

**This library is not production-ready.**

It is provided as a reference implementation for JWT authentication and key management. The author does not take responsibility for any use in production environments. You must review, test, and secure the code before using it in any critical or production system.

- No warranty or support is provided.
- Use at your own risk.
- Security, compliance, and reliability are your responsibility.

## Features
- Generate JWT access tokens and refresh tokens
- Validate JWT tokens and extract claims
- RSA key-based signing and validation
- Custom exception handling for token and key errors
- Easy service registration via dependency injection
- **Configurable JWT options** (issuer, audience, expiration, key rotation)
- **Enterprise security**: key size enforcement, issuer/audience validation, PEM format detection, secure vault integration hooks, key rotation

## Getting Started

### Prerequisites
- .NET 8.0 SDK or compatible
- RSA private key in XML format (legacy) or PEM format (recommended for enterprise)
- For PEM support, use .NET 5+ or integrate BouncyCastle

### Usage
#### Register the Service with Key Rotation
In your ASP.NET Core application, configure JWT options in your own `appsettings.*.json` and pass them to the library:
```csharp
services.AddAuthenticationService(options => {
    options.Issuer = "MyIssuer";
    options.Audience = "MyAudience";
    options.ExpirationMinutes = 15;
    options.KeyPaths.Add("key1", "path/to/private1.xml");
    options.KeyPaths.Add("key2", "path/to/private2.xml");
});
```

#### Generate a Token with Key ID
```csharp
var claims = new List<Claim> { new Claim(ClaimTypes.Name, "username") };
var token = authenticationService.GenerateToken(claims, "key1");
```

#### Validate a Token with Key ID
```csharp
var principal = authenticationService.GetTokenPrincipal(token, "key1");
```

#### Generate a Refresh Token
```csharp
var refreshToken = authenticationService.GenerateRefreshTokenString();
```

## Enterprise Security Notes
- **Key Storage**: Use secure vaults (Azure Key Vault, AWS KMS, HashiCorp Vault) for private keys in production. The code provides a hook for vault integration.
- **Key Format**: Prefer PEM (PKCS#8) for private keys. XML is supported for legacy only.
- **Key Size**: RSA keys must be at least 2048 bits (3072+ recommended).
- **Validation**: Issuer and audience are enforced for all tokens.
- **Key Rotation**: Use multiple keys and key IDs for rotation and migration.
- **Sensitive Data**: Never log or expose private key material or sensitive error details.

## Project Structure
- `AuthLibrary/Service/` - Core authentication logic and interfaces
- `AuthLibrary/Exceptions/` - Custom exception classes
- `AuthLibrary/ServiceSetup.cs` - Extension for DI registration
- `AuthLibrary.UnitTests/` - Unit tests for authentication logic
- `Keys/` - Example RSA key files

## Testing
Run unit tests with:
```sh
cd AuthLibrary.UnitTests
# Use your preferred test runner, e.g.:
dotnet test
```

## Security
- Always keep your private keys secure and never commit them to source control.
- Use environment variables or secure vaults for key paths in production.

## License
MIT

## Contributing
Pull requests and issues are welcome! Please open an issue to discuss your ideas or report bugs.