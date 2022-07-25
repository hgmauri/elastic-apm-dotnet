using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sample.ElasticApm.WebApi.Core.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.AddSerilog(builder.Configuration, "API Elastic APM");
Log.Information("Starting API");

builder.Services.AddApiConfiguration();

builder.Services.AddElasticsearch(builder.Configuration);
builder.Services.AddSqlDatabase(builder.Configuration);
builder.Services.AddSwagger(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseApiConfiguration(app.Environment);

app.UseSwaggerDoc();
app.UseElasticApm(builder.Configuration);

app.MapControllers();

app.Run();