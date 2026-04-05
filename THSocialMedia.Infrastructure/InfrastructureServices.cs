using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using THSocialMedia.Domain.Abstractions;
using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Infrastructure.EfDbContext;
using THSocialMedia.Infrastructure.EfDbContext.WriteRepositoies;

namespace THSocialMedia.Infrastructure
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration _configuration)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<WriteDbContext> (optionsBuilder =>
            {
                optionsBuilder.UseNpgsql(connectionString);
            });

            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<WriteDbContext>());

            return services;
        }
    }
}
