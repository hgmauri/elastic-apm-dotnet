using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.ElasticApm.Persistence.Context;

namespace Sample.ElasticApm.WebApi.Core.Extensions;

public static class SqlExtensions
{
    public static void AddSqlDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SampleDataContext>(o => o
            .UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
    }
}