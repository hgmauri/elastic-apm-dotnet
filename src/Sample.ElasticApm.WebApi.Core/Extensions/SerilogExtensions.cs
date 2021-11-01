using Elastic.Apm.SerilogEnricher;
using Elastic.CommonSchema.Serilog;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using Serilog.Exceptions;

namespace Sample.ElasticApm.WebApi.Core.Extensions;

public static class SerilogExtensions
{
    public static void AddSerilog(IConfiguration configuration)
    {
        //https://www.elastic.co/guide/en/apm/agent/dotnet/master/serilog.html
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithExceptionDetails()
            .Enrich.WithElasticApmCorrelationInfo()
            .Enrich.WithProperty("ApplicationName", $"API Elastic APM - {configuration.GetSection("DOTNET_ENVIRONMENT")?.Value}")
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticsearchSettings:uri"]))
            {
                CustomFormatter = new EcsTextFormatter(),
                AutoRegisterTemplate = true,
                IndexFormat = "indexlogs",
                ModifyConnectionSettings = x => x.BasicAuthentication(configuration["ElasticsearchSettings:username"], configuration["ElasticsearchSettings:password"])
            })
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .CreateLogger();
    }
}