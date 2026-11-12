
using System;
using System.IO;
using System.Linq;
using System.Text;

using Azure.Identity;
using Azure.Extensions.AspNetCore.DataProtection.Blobs;
using Azure.Extensions.AspNetCore.DataProtection.Keys;

using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using RaithuNestham.Data;
using RaithuNestham.Repositories;
using RaithuNestham.Repositories.Interfaces;
using RaithuNestham.Services;
using RaithuNestham.Services.Interfaces;
using RaithuNestham.Services.RAG;

var builder = WebApplication.CreateBuilder(args);

// =============================================
// 1. FIREBASE ADMIN SDK INITIALIZATION
// =============================================

var firebaseCredentialsPath =
    builder.Configuration["Firebase:CredentialsPath"];

if (!string.IsNullOrWhiteSpace(firebaseCredentialsPath))
{
    if (!File.Exists(firebaseCredentialsPath))
    {
        throw new FileNotFoundException(
            "Firebase service account credentials file was not found.",
            firebaseCredentialsPath);
    }

    FirebaseApp.Create(new AppOptions
    {
        Credential = CredentialFactory
            .FromFile<ServiceAccountCredential>(
                firebaseCredentialsPath)
            .ToGoogleCredential()
    });

    Console.WriteLine(
        "Firebase Admin SDK initialized successfully.");
}
else if (builder.Environment.IsProduction())
{
    throw new InvalidOperationException(
        "Firebase:CredentialsPath is required in production.");
}

// =============================================
// 2. CONTROLLERS
// =============================================

builder.Services.AddControllers();

// =============================================
// 3. AZURE DATA PROTECTION
// PRODUCTION: AZURE BLOB STORAGE + KEY VAULT
// DEVELOPMENT: LOCAL DATA PROTECTION KEYS
// =============================================

if (builder.Environment.IsProduction())
{
    // Azure Blob Storage SAS URL
    var dataProtectionBlobUri =
        builder.Configuration["DataProtection:BlobUri"];

    // Azure Key Vault configuration
    var tenantId =
        builder.Configuration["AzureKeyVault:TenantId"];

    var clientId =
        builder.Configuration["AzureKeyVault:ClientId"];

    var clientSecret =
        builder.Configuration["AzureKeyVault:ClientSecret"];

    var keyIdentifier =
        builder.Configuration["AzureKeyVault:KeyIdentifier"];

    // Validate configuration
    if (string.IsNullOrWhiteSpace(dataProtectionBlobUri) ||
        string.IsNullOrWhiteSpace(tenantId) ||
        string.IsNullOrWhiteSpace(clientId) ||
        string.IsNullOrWhiteSpace(clientSecret) ||
        string.IsNullOrWhiteSpace(keyIdentifier))
    {
        throw new InvalidOperationException(
            "Azure Data Protection configuration is incomplete. " +
            "Check Render environment variables.");
    }

    // Validate Blob SAS URL
    if (!Uri.TryCreate(
            dataProtectionBlobUri,
            UriKind.Absolute,
            out var blobUri) ||
        blobUri.Scheme != Uri.UriSchemeHttps)
    {
        throw new InvalidOperationException(
            "DataProtection:BlobUri must be a valid HTTPS URL.");
    }

    // Validate Key Vault key identifier
    if (!Uri.TryCreate(
            keyIdentifier,
            UriKind.Absolute,
            out var keyVaultUri) ||
        keyVaultUri.Scheme != Uri.UriSchemeHttps ||
        !string.Equals(
            keyVaultUri.Host,
            "raithunestham-kv-2026.vault.azure.net",
            StringComparison.OrdinalIgnoreCase))
    {
        throw new InvalidOperationException(
            "AzureKeyVault:KeyIdentifier must be a valid HTTPS " +
            "identifier for the RaithuNestham Key Vault.");
    }

    // Authenticate with Azure Key Vault
    var credential = new ClientSecretCredential(
        tenantId,
        clientId,
        clientSecret);

    // Persist Data Protection keys in Azure Blob Storage
    // Encrypt those keys using Azure Key Vault
    builder.Services
        .AddDataProtection()
        .SetApplicationName("RaithuNestham")
        .PersistKeysToAzureBlobStorage(blobUri)
        .ProtectKeysWithAzureKeyVault(
            keyVaultUri,
            credential);

    Console.WriteLine(
        "Production Azure Data Protection configured.");
}
else
{
    // Local development uses local Data Protection keys.
    // No Azure credentials are required locally.
    builder.Services
        .AddDataProtection()
        .SetApplicationName("RaithuNestham");

    Console.WriteLine(
        "Development Data Protection configured locally.");
}

// =============================================
// 4. SQL SERVER DATABASE
// =============================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        }));

// =============================================
// 5. HTTP CLIENTS AND EXTERNAL SERVICES
// =============================================

builder.Services.AddHttpClient();
builder.Services.AddHttpClient<WeatherForecastService>();

builder.Services.AddScoped<WeatherAdvisoryService>();
builder.Services.AddScoped<MarketPriceService>();
builder.Services.AddScoped<FirebasePushNotificationService>();

// =============================================
// 6. AUTHENTICATION SERVICES
// =============================================

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();

// =============================================
// 7. AI SERVICE
// =============================================

builder.Services.AddScoped<IAIService, AIService>();

// =============================================
// 8. RAG SERVICES
// =============================================

builder.Services.AddScoped<KnowledgeDocumentService>();
builder.Services.AddScoped<EmbeddingService>();
builder.Services.AddScoped<RagRetrievalService>();

// =============================================
// 9. JWT CONFIGURATION
// =============================================

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey) ||
    string.IsNullOrWhiteSpace(jwtIssuer) ||
    string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "JWT configuration is missing. " +
        "Check Jwt:Key, Jwt:Issuer, and Jwt:Audience.");
}

// =============================================
// 10. JWT AUTHENTICATION
// =============================================

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)),

                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,

                ValidateAudience = true,
                ValidAudience = jwtAudience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
    });

builder.Services.AddAuthorization();

// =============================================
// 11. CORS CONFIGURATION
// =============================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "http://localhost:8080",
                "https://localhost",
                "capacitor://localhost",
                "https://raithunestham.com",
                "https://www.raithunestham.com",
                "https://raithunestham-1.onrender.com")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// =============================================
// 12. SWAGGER CONFIGURATION
// =============================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RaithuNestham API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token."
        });

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
        });
});

// =============================================
// 13. BUILD APPLICATION
// =============================================

var app = builder.Build();

// =============================================
// 14. SWAGGER IN DEVELOPMENT ONLY
// =============================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// =============================================
// 15. MIDDLEWARE PIPELINE
// =============================================

app.UseCors("AngularPolicy");

app.UseAuthentication();
app.UseAuthorization();

// =============================================
// 16. MAP CONTROLLER ROUTES
// =============================================

app.MapControllers();

// =============================================
// 17. RAG DEVELOPMENT TEST ENDPOINTS
// =============================================

if (app.Environment.IsDevelopment())
{
    app.MapGet("/api/rag/test-chunks",
        async (KnowledgeDocumentService service) =>
        {
            var chunks = await service.LoadChunksAsync();

            return Results.Ok(new
            {
                Count = chunks.Count,
                Chunks = chunks
            });
        })
        .WithTags("RAG Development Tests");

    app.MapGet("/api/rag/test-embedding",
        async (string text, EmbeddingService service) =>
        {
            var vector = await service.CreateEmbeddingAsync(text);

            return Results.Ok(new
            {
                Success = vector.Count > 0,
                VectorLength = vector.Count,
                SampleValues = vector.Take(5)
            });
        })
        .WithTags("RAG Development Tests");

    app.MapGet("/api/rag/test-retrieval",
        async (string question, RagRetrievalService service) =>
        {
            var chunks = await service.RetrieveRelevantChunksAsync(
                question, 3);

            return Results.Ok(new
            {
                Question = question,
                RetrievedCount = chunks.Count,
                Chunks = chunks
            });
        })
        .WithTags("RAG Development Tests");

    app.MapGet("/api/rag/test-retrieval-scores",
        async (string question, RagRetrievalService service) =>
        {
            var results = await service.RetrieveScoredChunksAsync(
                question, 6);

            return Results.Ok(new
            {
                Question = question,
                RetrievedCount = results.Count,
                Results = results.Select(result => new
                {
                    result.Score,
                    result.Chunk
                })
            });
        })
        .WithTags("RAG Development Tests");

    app.MapGet("/api/rag/test-retrieval-threshold",
        async (
            string question,
            double minimumScore,
            RagRetrievalService service) =>
        {
            if (minimumScore < -1 || minimumScore > 1)
            {
                return Results.BadRequest(new
                {
                    Message = "minimumScore must be between -1 and 1."
                });
            }

            var chunks =
                await service.RetrieveRelevantChunksWithThresholdAsync(
                    question,
                    topK: 3,
                    minimumScore: minimumScore);

            return Results.Ok(new
            {
                Question = question,
                MinimumScore = minimumScore,
                RetrievedCount = chunks.Count,
                Chunks = chunks
            });
        })
        .WithTags("RAG Development Tests");
}

// =============================================
// 18. START APPLICATION
// =============================================

app.Run();
