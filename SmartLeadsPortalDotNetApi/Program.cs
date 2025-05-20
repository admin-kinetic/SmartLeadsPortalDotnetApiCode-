using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Graph;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestSharp;
using Serilog;
using SmartLeadsPortalDotNetApi.Aggregates.InboundCall;
using SmartLeadsPortalDotNetApi.Aggregates.OutboundCall;
using SmartLeadsPortalDotNetApi.Configs;
using SmartLeadsPortalDotNetApi.Conventions;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Factories;
using SmartLeadsPortalDotNetApi.Helper;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;
using SmartLeadsPortalDotNetApi.Services;
using System.Security.Cryptography;
using System.Text;

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
builder.Services.Configure<KineticLeadsPortalConfig>(builder.Configuration.GetSection("KineticLeadsPortalConfig"));
builder.Services.Configure<MicrosoftGraphSettings>(graphSettings =>
{
    builder.Configuration.GetSection("MicrosoftGraph").Bind(graphSettings);

    var clientSecret = Environment.GetEnvironmentVariable("MicrosoftGraph");
    if (!string.IsNullOrEmpty(clientSecret))
    {
        graphSettings.ClientSecret = clientSecret;
    }
});

builder.Services.Configure<StorageConfig>(storageConfig =>
{
    builder.Configuration.GetSection("AzureStorage").Bind(storageConfig);

    var accountKey = Environment.GetEnvironmentVariable("AZURESTORAGE_ACCOUNTKEY");
    if (!string.IsNullOrEmpty(accountKey))
    {
        storageConfig.AccountKey = accountKey;
    }
});

builder.Services.AddScoped<DbConnectionFactory>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<SmartLeadsRepository>();
builder.Services.AddScoped<AutomatedLeadsRepository>();
builder.Services.AddScoped<WebhooksRepository>();
builder.Services.AddScoped<ExcludedKeywordsRepository>();
builder.Services.AddScoped<CallDispositionRepository>();
builder.Services.AddScoped<CallStateRepository>();
builder.Services.AddScoped<CallPurposeRepository>();
builder.Services.AddScoped<CallTagRepository>();
builder.Services.AddScoped<CallLogsRepository>();
builder.Services.AddScoped<DashboardRepository>();
builder.Services.AddScoped<VoipHttpService>();
builder.Services.AddScoped<SmartLeadsApiService>();
builder.Services.AddScoped<LeadsPortalHttpService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<SmartLeadsExportedContactsRepository>();

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
builder.Services.AddScoped<BlobService>();
builder.Services.AddScoped<LeadClicksRepository>();
builder.Services.AddScoped<VoiplineWebhookRepository>();
builder.Services.AddScoped<CallTasksTableRepository>();
builder.Services.AddScoped<CallsTableRepository>();
builder.Services.AddScoped<SavedTableViewsRepository>();
builder.Services.AddScoped<VoipPhoneNumberRepository>();
builder.Services.AddScoped<OutboundCallEventParser>();
builder.Services.AddScoped<InboundCallEventParser>();
builder.Services.AddScoped<OutboundEventStore>();
builder.Services.AddScoped<OutboundCallRepository>();
builder.Services.AddScoped<InboundCallRepository>();
builder.Services.AddScoped<ProspectRepository>();
builder.Services.AddScoped<CategorySettingsRepository>();
builder.Services.AddScoped<OutlookService>();
builder.Services.AddSingleton<MicrosoftGraphAuthProvider>();
builder.Services.AddScoped<MicrosoftGraphServiceClientFactory>();
builder.Services.AddScoped<PermissionRepository>();
builder.Services.AddScoped<RoleRepository>();
builder.Services.AddScoped<LeadsPortalWebhookRepository>();
builder.Services.AddScoped<SmartLeadsEmailStatisticsRepository>();

builder.Services.AddScoped(provider =>
    {
        var factory = provider.GetRequiredService<MicrosoftGraphServiceClientFactory>();
        return new GraphClientWrapper(factory);
    });


builder.Services.AddSingleton(provider =>
    {
    return new StorageSharedKeyCredential(
            builder.Configuration["AzureStorage:AccountName"], 
            builder.Configuration["AzureStorage:AccountKey"]);
    });

// Register BlobServiceClient as a singleton
builder.Services.AddSingleton(provider =>
    {
        // return new BlobServiceClient(azureConnectionString).GetBlobContainerClient("upload-container");
        
        var sharedKeyCredential = provider.GetRequiredService<StorageSharedKeyCredential>();
        string blobContainerUrl = string.Format(
            "https://{0}.blob.core.windows.net/{1}",
            builder.Configuration["AzureStorage:AccountName"],
            builder.Configuration["AzureStorage:Container"]);
        return new BlobContainerClient(new Uri(blobContainerUrl), sharedKeyCredential);
    });


// Add services to the container.
builder.Services.AddControllers(options =>
    {
        options.Conventions.Add(new LowerCaseControllerModelConvention());
        options.Conventions.Add(new LowerCaseActionModelConvention());
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    }); ;

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
            "https://smartleads-export.kis-systems.com",
            "https://smartleadsportal-test.kineticstaff.com",
            "https://calls-test.kineticstaff.com")
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

var jwtSecret = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET") ?? builder.Configuration["Jwt:Secret"].ToString());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var hmac = new HMACSHA256(jwtSecret);
        var securityKey = new SymmetricSecurityKey(hmac.Key);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            ClockSkew = TimeSpan.FromMinutes(5)
        };
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
