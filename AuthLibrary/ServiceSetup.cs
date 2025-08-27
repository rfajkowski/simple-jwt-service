using System;
using Cority.AuthLibrary.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Cority.AuthLibrary
{
    public static class ServiceSetup
    {
        public static void AddAuthenticationService(this IServiceCollection serviceCollection, Action<JwtOptions> configureOptions)
        {
            var options = new JwtOptions();
            configureOptions?.Invoke(options);
            serviceCollection.AddScoped<IAuthenticationService>(sp => new AuthenticationService(options));
        }
    }
}
