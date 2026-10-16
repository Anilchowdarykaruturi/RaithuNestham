
using System;
using System.Linq;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
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

// 1. Controllers
builder.Services.AddControllers();

// 2. SQL Server database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. HTTP client for external AI APIs
builder.Services.AddHttpClient();

// 4. Authentication services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();

// 5. AI service
builder.Services.AddScoped<IAIService, AIService>();

// 6. RAG services
builder.Services.AddScoped<KnowledgeDocumentService>();
builder.Services.AddScoped<EmbeddingService>();
builder.Services.AddScoped<RagRetrievalService>();

// 7. JWT configuration
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey) ||
    string.IsNullOrWhiteSpace(jwtIssuer) ||
    string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "JWT configuration is missing. Check Jwt:Key, Jwt:Issuer, and Jwt:Audience.");
}

// 8. JWT authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
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

// 9. CORS for local and deployed Angular frontends
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
    "https://raithunestham-1.onrender.com"
)

            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// 10. Swagger configuration
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RaithuNestham API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
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

// 11. Build application
var app = builder.Build();

// 12. Swagger in Development only
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 13. Middleware pipeline
// CORS must run before authentication and authorization.
app.UseCors("AngularPolicy");

app.UseAuthentication();
app.UseAuthorization();

// 14. Map controller routes
app.MapControllers();

// 15. RAG development test endpoints
// These endpoints are not exposed in Production.
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

// 16. Start application
app.Run();
