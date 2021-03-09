using System.IO.Compression;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.ElasticApm.WebApi.Core.Extensions;
using Sample.ElasticApm.WebApi.Core.Middleware;
using Serilog;

namespace Sample.ElasticApm.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddElasticsearch(Configuration);
            services.AddServices();
            services.AddSwagger(Configuration);
            services.AddHealthCheckApi(Configuration);

            services.AddResponseCompression();
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = LogEnricherExtensions.EnrichFromRequest);

            app.UseMiddleware<RequestSerilLogMiddleware>();
            app.UseMiddleware<ErrorHandlingMiddleware>();
                
            app.UseSwaggerDoc();

            app.UseElasticApm(Configuration);
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseHealthCheckApi();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/hc",
                    new HealthCheckOptions
                    {
                        Predicate = _ => true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                    });
            });
        }
    }
}
