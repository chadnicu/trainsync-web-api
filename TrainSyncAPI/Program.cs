using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Clerk.BackendAPI;
using Clerk.BackendAPI.Models.Operations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TrainSyncAPI.Data;
using TrainSyncAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", true, true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

builder.Services.AddDbContext<TrainSyncContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "TrainSync API", Version = "v1" });

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

    // Require JWT token globally (optional, can also be per-controller/action)
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

// Add CORS policy for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowFrontend",
        policy =>
            policy
                .WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
    );
});

// Use environment variable for Clerk Issuer if set, otherwise fallback to default
var clerkIssuer = builder.Configuration["Clerk:Issuer"];
var clerkBackendToken = builder.Configuration["Clerk:ApiKey"];

Console.WriteLine($"Loaded Clerk Issuer: {clerkIssuer}");
Console.WriteLine($"Loaded Clerk API Key: {clerkBackendToken}");

var jwksUrl = $"{clerkIssuer}/.well-known/jwks.json";
Console.WriteLine($"JWKS URL: {jwksUrl}");
Console.WriteLine($"Expected issuer: {clerkIssuer}");

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
                var jwksJson = new HttpClient().GetStringAsync(jwksUrl).Result;
                var keys = new JsonWebKeySet(jwksJson);
                return keys.Keys;
            }
        };
        // Add debug output for authentication events
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"JWT auth failed: {context.Exception}");
                if (context.Request.Headers.ContainsKey("Authorization"))
                {
                    var authHeader = context.Request.Headers["Authorization"].ToString();
                    Console.WriteLine($"Authorization header: {authHeader}");
                    var token = authHeader.Replace("Bearer ", "");
                    try
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var jwt = handler.ReadJwtToken(token);
                        Console.WriteLine("Decoded JWT claims:");
                        foreach (var claim in jwt.Claims) Console.WriteLine($" - {claim.Type}: {claim.Value}");
                        Console.WriteLine($"Expected issuer: {clerkIssuer}");
                        Console.WriteLine($"Actual iss claim: {jwt.Issuer}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to decode JWT: {ex.Message}");
                    }
                }

                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("JWT token validated successfully.");
                var jwt = context.SecurityToken as JwtSecurityToken;
                if (jwt != null)
                {
                    Console.WriteLine("Validated JWT claims:");
                    foreach (var claim in jwt.Claims) Console.WriteLine($" - {claim.Type}: {claim.Value}");
                    Console.WriteLine($"Issuer: {jwt.Issuer}");
                }

                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Log OPTIONS requests for CORS debugging
app.Use(async (context, next) =>
    {
        if (context.Request.Method == "OPTIONS")
        {
            Console.WriteLine($"[CORS] OPTIONS {context.Request.Path}");
            foreach (var header in context.Request.Headers)
                Console.WriteLine($"[CORS] {header.Key}: {header.Value}");
        }

        await next();
    }
);

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

// -- Call Clerk to create a test token on startup --
if (!string.IsNullOrEmpty(clerkBackendToken))
{
    var sdk = new ClerkBackendApi(clerkBackendToken);
    try
    {
        // Get the first user (for demo/testing)
        var usersResponse = await sdk.Users.ListAsync();
        var userList = usersResponse?.UserList;
        var firstUser = userList?.FirstOrDefault();
        if (firstUser != null)
        {
            var sessionRequest = new CreateSessionRequestBody
            {
                UserId = firstUser.Id
            };
            var sessionResponse = await sdk.Sessions.CreateAsync(sessionRequest);
            var sessionId = sessionResponse?.Session?.Id;
            Console.WriteLine($"sessionId: {sessionId}");
            if (!string.IsNullOrEmpty(sessionId))
                try
                {
                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(
                            "Bearer",
                            clerkBackendToken
                        );
                    var content = new StringContent(
                        "{}",
                        Encoding.UTF8,
                        "application/json"
                    );
                    var response = await httpClient.PostAsync(
                        $"https://api.clerk.com/v1/sessions/{sessionId}/tokens",
                        content
                    );
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var jwtObj = JsonDocument.Parse(json);
                            if (jwtObj.RootElement.TryGetProperty("jwt", out var jwtProp))
                                Console.WriteLine(
                                    $"\nClerk test JWT (use as Bearer token):\n{jwtProp.GetString()}\n"
                                );
                        }
                        catch
                        {
                            /* ignore JSON parse errors */
                        }
                    }
                    else
                    {
                        Console.WriteLine(
                            "Failed to get JWT via HttpClient: "
                            + response.StatusCode
                            + "\n"
                            + await response.Content.ReadAsStringAsync()
                        );
                    }
                }
                catch (Exception httpEx)
                {
                    Console.WriteLine($"HttpClient failed: {httpEx}");
                }
            // Do not return; let the API continue running
            else
                Console.WriteLine("Could not create a session for the test user.");
        }
        else
        {
            Console.WriteLine(
                "No Clerk users found. Please create a user in your Clerk dashboard."
            );
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error generating Clerk JWT for testing: {ex}");
    }
}
else
{
    Console.WriteLine("CLERK_API_KEY environment variable is not set. Cannot create test token.");
}

app.Run();