using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using ProdManagement.Domain.Entities.User;
using Microsoft.Extensions.DependencyInjection;
using ProdManagement.Infrastructure.Persistence;
using ProdManagement.Application.Common.Settings;
using ProdManagement.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ProdManagement.Application.Features.Users.Login;
using ProdManagement.Application.Features.Users.Register;
using ProdManagement.Application.Abstractions.Persistence;
using ProdManagement.Infrastructure.Persistence.Repository;
using ProdManagement.Application.Abstractions.Authentication;

namespace ProdManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        #region DbContext

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        #endregion DbContext - End

        #region Identity Builder

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        #endregion Identity Builder - End

        #region Persistence

        services.AddScoped<IProductReadRepository, ProductReadRepository>();
        services.AddScoped<IProductWriteRepository, ProductWriteRepository>();

        #endregion Persistence - End

        #region MediatR

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(LoginCommandHandler).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(RegisterCommandHandler).Assembly);
        });

        #endregion MediatR - End

        #region Authentication & JWT

        // Configure strongly typed settings object
        services.Configure<TokenSettings>(configuration.GetSection("JwtSettings"));

        // Inject token generator
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        // Read JWT settings
        var jwtSettings = configuration.GetSection("JwtSettings").Get<TokenSettings>();

        // Configure JWT authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
            };
        });

        #endregion

        return services;
    }
}
