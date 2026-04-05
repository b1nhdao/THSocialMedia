using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using THSocialMedia.Infrastructure.EfDbContext;

namespace THSocialMedia.Infrastructure
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration _configuration)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<WriteDbContext> (optionsBuilder => optionsBuilder.UseNpgsql(connectionString));
            return services;
        }
    }
}
