using Microsoft.Extensions.DependencyInjection;
using THSocialMedia.Application.Services;

namespace THSocialMedia.Application
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IProposalService, ProposalService>();
            return services;
        }
    }
}
