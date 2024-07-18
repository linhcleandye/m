using Common.Logging;
using Infrastructure.Configurations;
using Infrastructure.Extensions;
using Infrastructure.Policies;
using Payment.API.HttpRepositories;
using Payment.API.HttpRepositories.Interfaces;
using Shared.Configurations;
using Stripe;

namespace Payment.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var stripeOptions = configuration.GetSection(nameof(StripeConfig))
            .Get<StripeConfig>();
        if (stripeOptions is null)
            throw new ArgumentNullException(nameof(stripeOptions), "Stripe configurations are missing");
        
        services.AddOptions<StripeConfig>().BindConfiguration(nameof(StripeConfig));
        StripeConfiguration.ApiKey = stripeOptions.ApiKey;
        
        services.AddSingleton(x => configuration.GetSection(nameof(ServiceUrls)));

        return services;
    }
    
    internal static void ConfigureCustomerHttpClient(this IServiceCollection services)
    {
        var urls = services.GetOptions<ServiceUrls>(nameof(ServiceUrls));
        if (urls == null || string.IsNullOrEmpty(urls.Customer))
            throw new ArgumentNullException("Customer Service Url is not configured");

        services.AddHttpClient<ICustomerRepository, CustomerRepository>("CustomersAPI",
                (sp, cl) => { cl.BaseAddress = new Uri($"{urls.Customer}/api/"); })
            .AddHttpMessageHandler<LoggingDelegatingHandler>()
            ;
        services.AddScoped(sp => sp.GetService<IHttpClientFactory>()
            .CreateClient("CustomersAPI"));
    }
}