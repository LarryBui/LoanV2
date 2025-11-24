using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OaigLoan.Application;
using OaigLoan.Application.Common.Interfaces;
using OaigLoan.Infrastructure.Authentication;
using OaigLoan.Infrastructure.Identity;
using OaigLoan.Infrastructure.Persistence;
using OaigLoan.Infrastructure.Persistence.Repositories;

namespace OaigLoan.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
        }

        services.AddDbContext<LoanDbContext>(options => options.UseNpgsql(connectionString));
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.SigningKey), "SigningKey is required.");

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<LoanDbContext>());
        services.AddScoped<IPasswordHasher, PasswordHasherAdapter>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
