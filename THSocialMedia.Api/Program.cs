using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using THSocialMedia.Application;
using THSocialMedia.Application.Commons;
using THSocialMedia.Application.Commons.Jwt;
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

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }});
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyHeader()
                                      .AllowAnyMethod());
            });

            builder.Services
                .AddHttpContextAccessor()
                .AddApplicationServices()
                .AddInfrastructureServices(builder.Configuration);

            builder.Services.AddSingleton(sp =>
            {
                var account = new Account
                {
                    Cloud = builder.Configuration.GetSection("Cloudinary:CloudName").Value ?? string.Empty,
                    ApiKey = builder.Configuration.GetSection("Cloudinary:ApiKey").Value ?? string.Empty,
                    ApiSecret = builder.Configuration.GetSection("Cloudinary:ApiSecret").Value ?? string.Empty,
                };
                var cloudinary = new Cloudinary(account);
                cloudinary.Api.Secure = true;

                return cloudinary;
            });

            // JWT auth
            var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
                options.InstanceName = "ThSocial";
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthentication();



            app.UseCors("AllowAllOrigins");
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