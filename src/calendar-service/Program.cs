using System;
using Calendar.General.Persistence;
using Calendar.Model.Compact;
using Calendar.Service.Configuration;
using Calendar.Service.Filters;
using Calendar.Service.Middlewares;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using NLog.Web;
using Vialto.WebApi.Fundamental.Extensions;
using Vialto.WebApi.Fundamental.Middlewares;
using ConfigurationManager = System.Configuration.ConfigurationManager;

var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
var builder = WebApplication.CreateBuilder(args);
ConfigurationHelper.Configuration = builder.Configuration;
if (!builder.Environment.IsDevelopment())
{
    // For non-dev environments, get all configuration ONLY from env vars.
    // This is to avoid falling back to appsettings.json and potentially using incorrect configuration values.
    ((IConfigurationBuilder)builder.Configuration).Sources.Clear();
    builder.Configuration.AddEnvironmentVariables();
}
try
{
    if (builder.Environment.IsDevelopment()) { 
        builder.Configuration
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appSettings.json", true);
    }

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(LogLevel.Trace);
    builder.Host.UseNLog();

    builder.Services.AddApplicationInsightsTelemetry(
        new ApplicationInsightsServiceOptions
        {
            ConnectionString = AppSettings.Config.ApplicationInsightsConnectionString
        });
    builder.Services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) =>
    {
        module.EnableSqlCommandTextInstrumentation =
            AppSettings.Config.EnableTrackSqlFullQueryTextInApplicationInsights == "true";
    });
    builder.Services.AddControllers(options =>
    {
        options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
    }).AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new DefaultNamingStrategy()
        };
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(config =>
    {
        config.CustomSchemaIds(x => x.FullName);

        // add Basic Authentication
        var basicSecurityScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "basic",
            Reference = new OpenApiReference { Id = "BasicAuth", Type = ReferenceType.SecurityScheme }
        };
        config.AddSecurityDefinition(basicSecurityScheme.Reference.Id, basicSecurityScheme);
        config.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { basicSecurityScheme, Array.Empty<string>() }
        });
    });

    builder.Services
        .AddAuthentication("BasicAuthentication")
        .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

    builder.Services.AddAuthorization();
    builder.Services.AddVialtoHealthChecks(
        ConfigurationManager.AppSettings["ApplicationName"],
        ConfigurationManager.AppSettings["ApplicationVersion"]
        )
        .AddSqlServer(AppSettings.Config.DBConnectionString);
    builder.Services.AddSingleton<RemoveDoubleSlashMiddleware>();
    builder.Services.AddSingleton<LogRequestIdWhenResponseFailedMiddleware>();

    var app = builder.Build();
    app.UseAccessLogging();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<RemoveDoubleSlashMiddleware>();
    app.UseMiddleware<LogRequestIdWhenResponseFailedMiddleware>();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseHttpException();

    app.MapVialtoHealthChecks();
    app.MapControllers();

    DbInitializerFactory.CreateSqlServer(AppSettings.Config.DBConnectionString);
    DbConfigurationFactory.CreateSqlServer(AppSettings.Config.DBConnectionString);

    app.Run();
}
catch (Exception e)
{
    logger.Error(e, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}