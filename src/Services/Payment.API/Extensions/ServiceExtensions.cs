using Infrastructure.Configurations;
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

        return services;
    }
}