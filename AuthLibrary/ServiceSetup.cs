using Cority.AuthLibrary.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Cority.AuthLibrary
{
    public static class ServiceSetup
    {
        public static void AddAuthenticationService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IAuthenticationService, AuthenticationService>();
        }
    }
}
