using AuthLibrary.Exceptions;
using AuthLibrary.Service;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Claims;

namespace AuthLibrary.UnitTests;

public class AuthenticationServiceTests
{
    private IAuthenticationService _authService;
    private readonly string _privateKeyPath;
    private readonly string _invalidKeyPath;
    private readonly string _invalidPath;
    private JwtOptions _jwtOptions;
    private IKeyProvider _keyProvider;

    public AuthenticationServiceTests()
    {
        _jwtOptions = new JwtOptions
        {
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpirationMinutes = 30,
            KeyPaths = new Dictionary<string, string>
            {
                { "Keys/private.xml", "Keys/private.xml" },
                { "Keys/emptyKey.xml", "Keys/emptyKey.xml" },
                { "Keys/notExistingKey.xml", "Keys/notExistingKey.xml" }
            }
        };
        _keyProvider = new LocalKeyProvider(_jwtOptions.KeyPaths);
        _authService = new AuthenticationService(_jwtOptions, _keyProvider);
        _privateKeyPath = "Keys/private.xml";
        _invalidKeyPath = "Keys/emptyKey.xml";
        _invalidPath = "Keys/notExistingKey.xml";
    }

    [Fact]
    public void GenerateRefreshTokenString_ReturnsString()
    {
        var token = _authService.GenerateRefreshTokenString();
        token.Should().NotBeNullOrEmpty();
        token.Should().BeOfType<string>();
    }

    [Fact]
    public void GenerateRefreshTokenString_ShouldReturnBase64String()
    {
        var token = _authService.GenerateRefreshTokenString();
        Action act = () => Convert.FromBase64String(token);
        act.Should().NotThrow<FormatException>();
    }

    [Fact]
    public void GenerateRefreshTokenString_ShouldReturnStringOfExpectedLength()
    {
        var token = _authService.GenerateRefreshTokenString();
        var decodedBytes = Convert.FromBase64String(token);
        decodedBytes.Length.Should().Be(64);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidToken()
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
        var token = _authService.GenerateToken(claims, _privateKeyPath);
        token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenerateToken_ShouldThrowArgumentNullException_WhenClaimsAreNull()
    {
        Action act = () => _authService.GenerateToken(null, _privateKeyPath);
        act.Should().Throw<TokenException>().WithMessage("Claims are not set");
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidToken_WhenClaimsAreEmpty()
    {
        var claims = new List<Claim>();
        var token = _authService.GenerateToken(claims, _privateKeyPath);
        token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenerateToken_ShouldThrowSecurityKeyException_WhenPrivateKeyPathIsNull()
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
        Action act = () => _authService.GenerateToken(claims, null);
        act.Should().Throw<SecurityKeyException>().WithMessage("Private key path is not set");
    }

    [Fact]
    public void GenerateToken_ShouldThrowSecurityKeyException_WhenPrivateKeyPathIsInvalid()
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
        Action act = () => _authService.GenerateToken(claims, _invalidPath);
        act.Should().Throw<SecurityKeyException>().WithMessage("File not found");
    }

    [Fact]
    public void GenerateToken_ShouldThrowSecurityKeyException_WhenXmlContentIsInvalid()
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
        Action act = () => _authService.GenerateToken(claims, _invalidKeyPath);
        act.Should().Throw<SecurityKeyException>().WithMessage("Invalid XML file");
    }

    [Fact]
    public void GetTokenPrincipal_ShouldReturnClaimsPrincipal()
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
        var token = _authService.GenerateToken(claims, _privateKeyPath);
        var principal = _authService.GetTokenPrincipal(token, _privateKeyPath);
        principal.Should().NotBeNull();
        principal.Identity?.Name.Should().Be("testuser");
    }

    [Fact]
    public void GetTokenPrincipal_ShouldThrowArgumentNullException_WhenTokenIsNull()
    {
        Action act = () => _authService.GetTokenPrincipal(null, _privateKeyPath);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetTokenPrincipal_ShouldThrowArgumentException_WhenTokenIsEmpty()
    {
        Action act = () => _authService.GetTokenPrincipal(string.Empty, _privateKeyPath);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GetTokenPrincipal_ShouldThrowSecurityTokenException_WhenTokenIsInvalid()
    {
        var invalidToken = "invalidToken";
        Action act = () => _authService.GetTokenPrincipal(invalidToken, _privateKeyPath);
        act.Should().Throw<SecurityTokenException>().WithMessage("Invalid token");
    }

    [Fact]
    public void GetSecurityKey_ShouldReturnRsaSecurityKey()
    {
        var rsaSecurityKey = _authService.GetSecurityKey(_privateKeyPath);
        rsaSecurityKey.Should().NotBeNull();
        rsaSecurityKey.Should().BeOfType<RsaSecurityKey>();
    }

    [Fact]
    public void GetSecurityKey_ShouldThrowException_WhenPrivateKeyPathIsNull()
    {
        Action act = () => _authService.GetSecurityKey(null);
        act.Should().Throw<SecurityKeyException>().WithMessage("Private key path is not set");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void GetSecurityKey_ShouldThrowSecurityKeyException_WhenPrivateKeyPathIsEmptyOrWhitespace(string path)
    {
        Action act = () => _authService.GetSecurityKey(path);
        act.Should().Throw<Exception>().WithMessage("Private key path is not set");
    }

    [Fact]
    public void GetSecurityKey_ShouldThrowSecurityKeyException_WhenPrivateKeyPathIsInvalid()
    {
        Action act = () => _authService.GetSecurityKey(_invalidKeyPath);
        act.Should().Throw<SecurityKeyException>().WithMessage("Invalid XML file");
    }

    [Fact]
    public void GetSecurityKey_ShouldThrowSecurityKeyException_WhenInvalidPathProvided()
    {
        Action act = () => _authService.GetSecurityKey(_invalidPath);
        act.Should().Throw<SecurityKeyException>().WithMessage("File not found");
    }
}