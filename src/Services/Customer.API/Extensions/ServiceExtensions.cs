using Contracts.Domains.Interfaces;
using Customer.API.Persistence;
using Customer.API.Repositories;
using Customer.API.Repositories.Interfaces;
using Customer.API.Services;
using Customer.API.Services.Interfaces;
using Infrastructure.Common;
using Infrastructure.Configurations;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shared.Configurations;
using Stripe;
using CustomerService = Customer.API.Services.CustomerService;

namespace Customer.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings))
            .Get<DatabaseSettings>();
        if (databaseSettings is null)
            throw new ArgumentNullException(nameof(databaseSettings), "Database settings are missing");
        services.AddSingleton(databaseSettings);
        
        var stripeOptions = configuration.GetSection(nameof(StripeConfig))
            .Get<StripeConfig>();
        if (stripeOptions is null)
            throw new ArgumentNullException(nameof(stripeOptions), "Stripe configurations are missing");
        
        services.AddOptions<StripeConfig>().BindConfiguration(nameof(StripeConfig));
        StripeConfiguration.ApiKey = stripeOptions.ApiKey;

        return services;
    }

    public static void ConfigureCustomerContext(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
        if (databaseSettings == null || string.IsNullOrEmpty(databaseSettings.ConnectionString))
            throw new ArgumentNullException("Connection string is not configured.");

        services.AddDbContext<CustomerContext>(
            options => options.UseNpgsql(databaseSettings.ConnectionString));
    }

    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services
            .AddScoped(typeof(IRepositoryBase<,,>), typeof(RepositoryBase<,,>))
            .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<ICustomerStripeService, CustomerStripeService>()
            .AddScoped<Stripe.CustomerService>()
            ;
    }

    public static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
        services.AddHealthChecks()
            .AddNpgSql(databaseSettings.ConnectionString,
                name: "PostgresQL Health",
                failureStatus: HealthStatus.Degraded);
    }

    public static void ConfigureApiVersion(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new(1.0);
            options.AssumeDefaultVersionWhenUnspecified = true;
        });
    }
    
    public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration["AllowOrigins"] ?? throw new InvalidOperationException("AllowedOrigin is not set");;
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(buider =>
            {
                buider.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }
}