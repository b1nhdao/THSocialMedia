using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using THSocialMedia.Application;
using THSocialMedia.Application.UsecaseHandlers.Users.Commands;
using THSocialMedia.Infrastructure;
using THSocialMedia.Infrastructure.EfDbContext;

namespace THSocialMedia.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddSwaggerGen();

            builder.Services
                .AddHttpContextAccessor()
                .AddApplicationServices()
                .AddInfrastructureServices(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<WriteDbContext>();

                    await context.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    // log nếu cần
                    Console.WriteLine($"Migration error: {ex.Message}");
                }
            }

            app.Run();
        }
    }
}
