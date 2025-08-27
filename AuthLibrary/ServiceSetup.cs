using System;
using AuthLibrary.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace AuthLibrary
{
    public static class ServiceSetup
    {
        public static void AddAuthenticationService(this IServiceCollection serviceCollection, Action<AuthLibrary.Service.JwtOptions> configureOptions)
        {
            var options = new AuthLibrary.Service.JwtOptions();
            configureOptions?.Invoke(options);
            var keyProvider = new AuthLibrary.Service.LocalKeyProvider(options.KeyPaths);
            serviceCollection.AddScoped<AuthLibrary.Service.IAuthenticationService>(sp => new AuthLibrary.Service.AuthenticationService(options, keyProvider));
        }

        public static void AddAuthenticationService<TProvider>(this IServiceCollection serviceCollection, Action<AuthLibrary.Service.JwtOptions> configureOptions, Func<AuthLibrary.Service.JwtOptions, AuthLibrary.Service.IKeyProvider> keyProviderFactory)
            where TProvider : AuthLibrary.Service.IKeyProvider
        {
            var options = new AuthLibrary.Service.JwtOptions();
            configureOptions?.Invoke(options);
            var keyProvider = keyProviderFactory(options);
            serviceCollection.AddScoped<AuthLibrary.Service.IAuthenticationService>(sp => new AuthLibrary.Service.AuthenticationService(options, keyProvider));
        }

        public static AuthLibrary.Service.JwtOptions GetJwtOptionsFromConfig(IConfiguration config)
        {
            var section = config.GetSection("Jwt");
            var options = new AuthLibrary.Service.JwtOptions
            {
                Issuer = section["Issuer"],
                Audience = section["Audience"],
                ExpirationMinutes = int.TryParse(section["ExpirationMinutes"], out var exp) ? exp : 30
            };
            var keyPaths = section.GetSection("KeyPaths").GetChildren();
            foreach (var kp in keyPaths)
            {
                options.KeyPaths[kp.Key] = kp.Value;
            }
            return options;
        }
    }
}
