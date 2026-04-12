using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using StackExchange.Redis;
using THSocialMedia.Domain.Abstractions;
using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Infrastructure.EfDbContext;
using THSocialMedia.Infrastructure.EfDbContext.WriteRepositoies;
using THSocialMedia.Infrastructure.EventBus;
using THSocialMedia.Infrastructure.Services.Jwt;
using THSocialMedia.Infrastructure.Services.RedisCache;
using THSocialMedia.Infrastructure.MongoDb.Repositories;
using THSocialMedia.Infrastructure.MongoDb.Abstractions;

namespace THSocialMedia.Infrastructure
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration _configuration)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<WriteDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseNpgsql(connectionString);
            });

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var config = ConfigurationOptions.Parse(
                    _configuration.GetConnectionString("Redis")
                );

                config.AbortOnConnectFail = false;
                config.ConnectTimeout = 500;
                config.SyncTimeout = 500;
                return ConnectionMultiplexer.Connect(config);
            });

            // MongoDB configuration
            services.AddSingleton<IMongoClient>(sp =>
            {
                var mongoConnectionString = _configuration.GetConnectionString("MongoDB");
                return new MongoClient(mongoConnectionString);
            });

            services.AddSingleton(sp =>
            {
                var mongoClient = sp.GetRequiredService<IMongoClient>();
                var mongoDbName = _configuration.GetSection("MongoDB:DatabaseName").Value ?? "THSocialMediaRead";
                return mongoClient.GetDatabase(mongoDbName);
            });

            // Event Bus
            services.AddScoped<IEventBus, InMemoryEventBus>();

            // Read repositories
            services.AddScoped<IPostReadRepository, PostReadRepository>();

            // Write repositories
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IRelationshipRepository, RelationshipRepository>();
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<WriteDbContext>());
            services.AddScoped<ICacheService, CacheService>();

            services.AddSingleton(sp =>
            {
                var opts = new JwtOptions();
                _configuration.GetSection(JwtOptions.SectionName).Bind(opts);
                return Microsoft.Extensions.Options.Options.Create(opts);
            });
            services.AddSingleton<IJwtTokenProvider, JwtTokenProvider>();

            return services;
        }
    }
}
