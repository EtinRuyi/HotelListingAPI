using Hotel_Listing.api.Configurations;
using Hotel_Listing.api.Data;
using Hotel_Listing.api.Middlewares;
using Hotel_Listing.api.Models;
using Hotel_Listing.api.Services.Contracts;
using Hotel_Listing.api.Services.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.OData;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connection = builder.Configuration.GetConnectionString("HotelListingDB");
builder.Services.AddDbContext<HotelListingDbContext>(options =>
{
    options.UseSqlServer(connection);
});

//Configuring Identity Core
builder.Services.AddIdentityCore<ApiUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<ApiUser>>("HotelListingApi")
    .AddEntityFrameworkStores<HotelListingDbContext>()
    .AddDefaultTokenProviders();



//Configuring API Security, Authorization
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Hotel Lising Api", Version = "v1" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
       Description = @"JWT Authorization header using the Bearer scheme. 
                     Enter 'Bearer' [space] and then your token in the text below.
                     Example: 'Bearer 123456767ef'",
       Name = "Authorization",
       In = ParameterLocation.Header,
       Type = SecuritySchemeType.ApiKey,
       Scheme = JwtBearerDefaults.AuthenticationScheme
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "0auth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
}); 

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", 
        b => b.AllowAnyHeader()
            .AllowAnyOrigin()
            .AllowAnyMethod());
});

//Registering Api Versioning
builder.Services.AddApiVersioning(options => 
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("X-Version"),
        new MediaTypeApiVersionReader("ver")
        );
});

builder.Services.AddVersionedApiExplorer(
    options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    }
);

//SeriLog Configuration.
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

//AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(MapperConfig));
 
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICountryRepo, CountryRepo>();
builder.Services.AddScoped<IHotelRepo, HotelRepo>();
builder.Services.AddScoped<IAuthManager, AuthManager>();

//Configuring JWT Auth
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
    };
});

//Configure Response Cacheing
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024;
    options.UseCaseSensitivePaths = true;
});

//Adding Health Checks to the Controllers  -- AspNetCore.HealthChecks.SqlServer 
//EFCore - Microsoft.Extensions.Diagnostics.HealthChecks.EntityFramworkCore
builder.Services.AddHealthChecks()
    .AddCheck<CustomHealthChecks>("CustomHealthCheck", 
        failureStatus: HealthStatus.Degraded, 
        tags: new[]{ "custom" }
        ).AddSqlServer(connection, tags: new[] { "database" }) //Database is healthy
        .AddDbContextCheck<HotelListingDbContext>(tags: new[] { "database" }); //Checking that the we can connect to the Db

//Configuring OData to #AddControllers
builder.Services.AddControllers().AddOData(options =>
{
    options.Select().Filter().OrderBy();
});


var app = builder.Build();
 
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Adding the middleware for Exceptoin in the pipeline
app.UseMiddleware<ExceptionMiddleware>();

//Middleware for health checks
app.MapHealthChecks("/healthcheck", new HealthCheckOptions
{
    Predicate = healthcheck => healthcheck.Tags.Contains("custom"),    //Run checks with custom tags
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
    },
    ResponseWriter = WriteResponse
});

//Database Health Checks
app.MapHealthChecks("/databasehealthcheck", new HealthCheckOptions
{
    Predicate = healthcheck => healthcheck.Tags.Contains("database"),    //Run checks for database
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
    },
    ResponseWriter = WriteResponse
});

//A Base Health Checker
app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
    },
    ResponseWriter = WriteResponse
});
//--Extention of creating a custom health checks.. Handdy when you have multiple health checks
static Task WriteResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json; charset=utf-8";

    var options = new JsonWriterOptions {Indented = true};

    using var memoryStream = new MemoryStream();
    using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
    {
        jsonWriter.WriteStartObject();
        jsonWriter.WriteString("status", report.Status.ToString());
        jsonWriter.WriteStartObject("results");

        foreach(var healthRepositoryEntry in report.Entries)
        {
            jsonWriter.WriteStartObject(healthRepositoryEntry.Key);
            jsonWriter.WriteString("status", healthRepositoryEntry.Value.Status.ToString());
            jsonWriter.WriteString("description", healthRepositoryEntry.Value.Description);
            jsonWriter.WriteStartObject("data");

            foreach(var item in healthRepositoryEntry.Value.Data)
            {
                jsonWriter.WritePropertyName(item.Key);

                System.Text.Json.JsonSerializer.Serialize(jsonWriter, item.Value, 
                    item.Value?.GetType() ?? typeof(object));
            }

            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
        }

        jsonWriter.WriteEndObject();
        jsonWriter.WriteEndObject();
    }
    return context.Response.WriteAsync(
        Encoding.UTF8.GetString(memoryStream.ToArray()));
}

app.MapHealthChecks("/health"); //Run all default check


//Adding Serilog to the pipeline
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

//Adding Cors to the pipeline
app.UseCors("AllowAll");

//Adding Cacheing
app.UseResponseCaching();

app.Use(async (context, next) =>
{
    context.Response.GetTypedHeaders().CacheControl =
    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
    {
        Public = true,
        MaxAge = TimeSpan.FromSeconds(10)
    };
    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
    new string[] { "Accept-Encoding" };

    await next();
});

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();

class CustomHealthChecks : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isHealthy = true;
        /*customs checks... Logic.... etc */

        if (isHealthy)
        {
            return Task.FromResult(HealthCheckResult.Healthy("All Systems are looking good"));
        }
        return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, "System Unhealthy"));
    }
}