using Housemaid.api.Data;
using Housemaid.api.Services;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.EntityFrameworkCore;

namespace Housemaid.api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(5),
                LocalCacheExpiration = TimeSpan.FromMinutes(1)
            };
        });

        var redisConnectionString = config.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
            });
        }

        services.AddDbContext<AppDbContext>(options =>
        {
            var conn = config.GetConnectionString("DummyDB") ?? throw new InvalidOperationException("Bad Connection string");
            options.UseNpgsql(conn, sql => sql.EnableRetryOnFailure(maxRetryCount: 3));
        });
        services.AddScoped<IPropertyAccessService, PropertyAccessService>();

        return services;
    }
}