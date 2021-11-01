using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.ElasticApm.Domain.Application;
using Sample.ElasticApm.Domain.Interface;
using Sample.ElasticApm.WebApi.Core.Middleware;

namespace Sample.ElasticApm.WebApi.Core.Extensions;

public static class ApiConfigurationExtensions
{
    public static void AddApiConfiguration(this IServiceCollection services)
    {
        services.AddRouting(options => options.LowercaseUrls = true);

        services.AddTransient<ISampleApplication, SampleApplication>();

        services.AddHttpClient();
        services.AddControllers();
    }

    public static void UseApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseMiddleware<RequestSerilLogMiddleware>();
        app.UseMiddleware<ErrorHandlingMiddleware>();

        app.UseHttpsRedirection();
        app.UseRouting();
    }
}