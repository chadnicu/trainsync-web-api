using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Clerk.BackendAPI;
using Clerk.BackendAPI.Models.Operations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TrainSyncAPI.Data;
using TrainSyncAPI.Mapping;
using TrainSyncAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", true, true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

builder.Services.AddDbContext<TrainSyncContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "TrainSync API", Version = "v1" });
    options.UseInlineDefinitionsForEnums();

    // Add JWT Authentication definition
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        }
    );

    // Require JWT token globally
    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        }
    );
});

// Allowed origins are stored as environment variable in comma-separated format (e.g. "AllowedOrigins": "http://localhost:3000,https://trainsync.site" "
var allowedOrigins = builder.Configuration["AllowedOrigins"]?.Split(',') ?? [];

// Add CORS policy for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowFrontend",
        policy =>
            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
    );
});

// Clerk env's
var clerkIssuer = builder.Configuration["Clerk:Issuer"];
var clerkBackendToken = builder.Configuration["Clerk:ApiKey"];
var jwksUrl = $"{clerkIssuer}/.well-known/jwks.json";

// Add HTTP Client singleton for DI
builder.Services.AddHttpClient();

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = clerkIssuer;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = clerkIssuer,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            NameClaimType = "sub",
            RoleClaimType = "role",
            IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
            {
                // var jwksJson = new HttpClient().GetStringAsync(jwksUrl).Result;
                using var client = new HttpClient();
                var jwksJson = client.GetStringAsync(jwksUrl).GetAwaiter().GetResult();
                var keys = new JsonWebKeySet(jwksJson);
                return keys.Keys;
            }
        };

        if (builder.Environment.IsDevelopment())
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogError(context.Exception, "JWT authentication failed");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("JWT token validated successfully.");
                    return Task.CompletedTask;
                }
            };
    });

builder.Services.AddAuthorization();

// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS before authentication/authorization
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

//  Generate Clerk test token in Development
if (app.Environment.IsDevelopment() && !string.IsNullOrEmpty(clerkBackendToken))
{
    try
    {
        var sdk = new ClerkBackendApi(clerkBackendToken);
        var usersResponse = await sdk.Users.ListAsync();
        var firstUser = usersResponse?.UserList?.FirstOrDefault();
        if (firstUser != null)
        {
            var sessionRequest = new CreateSessionRequestBody { UserId = firstUser.Id };
            var sessionResponse = await sdk.Sessions.CreateAsync(sessionRequest);
            var sessionId = sessionResponse?.Session?.Id;

            if (!string.IsNullOrEmpty(sessionId))
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", clerkBackendToken);
                var content = new StringContent("{}", Encoding.UTF8, "application/json");
                var response =
                    await httpClient.PostAsync($"https://api.clerk.com/v1/sessions/{sessionId}/tokens", content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var jwtObj = JsonDocument.Parse(json);
                    if (jwtObj.RootElement.TryGetProperty("jwt", out var jwtProp))
                    {
                        var logger = app.Services.GetRequiredService<ILogger<Program>>();
                        logger.LogInformation("Clerk test JWT (use as Bearer token): {Jwt}", jwtProp.GetString());
                    }
                }
                else
                {
                    var logger = app.Services.GetRequiredService<ILogger<Program>>();
                    logger.LogWarning("Failed to get JWT via HttpClient: {StatusCode} {Content}",
                        response.StatusCode, await response.Content.ReadAsStringAsync());
                }
            }
            else
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogWarning("Could not create a session for the test user.");
            }
        }
        else
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("No Clerk users found. Please create a user in your Clerk dashboard.");
        }
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error generating Clerk JWT for testing");
    }
}
else
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation(
        "CLERK_API_KEY environment variable is not set or not in development. Skipping test token creation.");
}

app.Run();