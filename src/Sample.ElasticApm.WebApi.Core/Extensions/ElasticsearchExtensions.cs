using System;
using Elastic.Apm.NetCoreAll;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace Sample.ElasticApm.WebApi.Core.Extensions
{
    public static class ElasticsearchExtensions
    {
        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = new ConnectionSettings(new Uri(configuration["ElasticsearchSettings:uri"]));

            var defaultIndex = configuration["ElasticsearchSettings:defaultIndex"];

            if (!string.IsNullOrEmpty(defaultIndex))
                settings = settings.DefaultIndex(defaultIndex);

            var basicAuthUser = configuration["ElasticsearchSettings:username"];
            var basicAuthPassword = configuration["ElasticsearchSettings:password"];

            if (!string.IsNullOrEmpty(basicAuthUser) && !string.IsNullOrEmpty(basicAuthPassword))
                settings = settings.BasicAuthentication(basicAuthUser, basicAuthPassword);

            var client = new ElasticClient(settings);

            //https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/lifetimes.html
            services.AddSingleton<IElasticClient>(client);

            //https://www.elastic.co/guide/en/apm/agent/dotnet/current/config-http.html#config-capture-body
            services.Configure<KestrelServerOptions>(options => options.AllowSynchronousIO = true);
            services.Configure<IISServerOptions>(options => options.AllowSynchronousIO = true);
        }

        public static void UseElasticApm(this IApplicationBuilder app, IConfiguration configuration)
        {
            //https://www.elastic.co/guide/en/apm/agent/dotnet/current/configuration-on-asp-net-core.html
            app.UseAllElasticApm(configuration);
        }
    }
}
