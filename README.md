# Simple JWT Service

A lightweight .NET library for generating and validating JWT tokens using RSA keys. Designed for easy integration with ASP.NET Core applications and supports secure authentication workflows.

## Features
- Generate JWT access tokens and refresh tokens
- Validate JWT tokens and extract claims
- RSA key-based signing and validation
- Custom exception handling for token and key errors
- Easy service registration via dependency injection
- **Configurable JWT options** (issuer, expiration, key path)

## Getting Started

### Prerequisites
- .NET 8.0 SDK or compatible
- RSA private key in XML format

### Installation
1. Clone the repository:
   ```sh
   git clone <your-repo-url>
   ```
2. Add the `AuthLibrary` project to your solution or reference the DLL.

### Usage
#### Register the Service with Options
In your ASP.NET Core `Startup.cs` or Program setup:
```csharp
services.AddAuthenticationService(options => {
    options.Issuer = "MyIssuer";
    options.ExpirationMinutes = 60;
    options.PrivateKeyPath = "path/to/private.xml";
});
```

#### Generate a Token
```csharp
var claims = new List<Claim> { new Claim(ClaimTypes.Name, "username") };
var token = authenticationService.GenerateToken(claims);
```

#### Validate a Token
```csharp
var principal = authenticationService.GetTokenPrincipal(token);
```

#### Generate a Refresh Token
```csharp
var refreshToken = authenticationService.GenerateRefreshTokenString();
```

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