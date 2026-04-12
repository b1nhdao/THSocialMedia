using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using THSocialMedia.Application.Services.AuthService;
using THSocialMedia.Infrastructure;

namespace THSocialMedia.Application
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IIdentityService, IdentityService>();

            // Register MediatR from both Application and Infrastructure assemblies
            // Application assembly: Commands, Queries, and their handlers
            // Infrastructure assembly: Event handlers (INotificationHandler implementations)
            services.AddMediatR(cfg => 
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(ApplicationServices))!);
                cfg.RegisterServicesFromAssembly(typeof(InfrastructureServices).Assembly);
            });

            return services;
        }
    }
}
