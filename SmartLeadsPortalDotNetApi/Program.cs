using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi.Models;
using RestSharp;
using Serilog;
using SmartLeadsPortalDotNetApi.Configs;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Helper;
using SmartLeadsPortalDotNetApi.Repositories;
using SmartLeadsPortalDotNetApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Create the logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/smartleadsportal.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<VoIpConfig>(builder.Configuration.GetSection("VoIpConfig"));
builder.Services.Configure<SmartLeadConfig>(builder.Configuration.GetSection("SmartLeadsConfig"));
builder.Services.AddScoped<DbConnectionFactory>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<AutomatedLeadsRepository>();
builder.Services.AddScoped<WebhooksRepository>();
builder.Services.AddScoped<ExcludedKeywordsRepository>();
builder.Services.AddScoped<VoipHttpService>();
builder.Services.AddScoped<SmartLeadsApiService>();
builder.Services.AddScoped<RestClient>(provider =>
{
    var options = new RestClientOptions
    {

        ThrowOnAnyError = true,
        Timeout = TimeSpan.FromMilliseconds(5000)
    };

    return new RestClient(options);
});

builder.Services.AddHttpClient();

builder.Services.AddHttpClient("VoipClient", client =>
{
    client.BaseAddress = new Uri("https://au.voipcloud.online/api/integration/v2");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = long.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});

builder.Services.AddScoped<WebhookService>();
builder.Services.AddScoped<LeadClicksRepository>();
builder.Services.AddScoped<VoiplineWebhookRepository>();


// Add services to the container.
// builder.Services.AddControllers(options =>
//     {
//         options.Conventions.Add(new KebabCaseControllerModelConvention());
//         options.Conventions.Add(new KebabCaseActionModelConvention());
//     })
//     .AddJsonOptions(options =>
//     {
//         options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
//     }); ;

builder.Services.AddCors(options =>
{

    // Test comment only

    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    var localDev = env != null && env.Equals("Development", StringComparison.OrdinalIgnoreCase)
        ? "http://localhost:4200" : string.Empty;
    var localDevhttps = env != null && env.Equals("Development", StringComparison.OrdinalIgnoreCase)
        ? "https://localhost:4200" : string.Empty;

    options.AddPolicy("CorsApi", builder =>
        builder.WithOrigins(
            localDev,
            localDevhttps,
            "smartleads-export.kis-systems.com",
            "https://smartleads-export.kis-systems.com")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .WithExposedHeaders("Content-Disposition"));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartLeadsPortal", Version = "v1" });
});

var app = builder.Build();

// Trigger database connection validation on startup
using (var scope = app.Services.CreateScope())
{
   var dbConnectionFactory = scope.ServiceProvider.GetRequiredService<DbConnectionFactory>();
   dbConnectionFactory.ValidateConnections();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartLeadsPortal API V1");
});

if (app.Environment.IsProduction())
{
    app.UseExceptionHandler("/error"); // Custom error handler, if any
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage(); // Shows detailed errors
}

app.Map("/error", (HttpContext context) =>
{
    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
    return Results.Problem(detail: exceptionHandlerPathFeature?.Error?.Message, title: "An error occurred");
});

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("CorsApi");

app.UseAuthorization();

app.MapControllers();

app.Run();
