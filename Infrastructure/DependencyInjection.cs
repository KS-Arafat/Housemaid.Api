using Housemaid.api.Data;
using Microsoft.EntityFrameworkCore;

namespace Housemaid.api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            var conn = config.GetConnectionString("DummyDB") ?? throw new InvalidOperationException("Bad Connection string");
            options.UseNpgsql(conn, sql => sql.EnableRetryOnFailure(maxRetryCount: 3));
        });

        return services;
    }
}