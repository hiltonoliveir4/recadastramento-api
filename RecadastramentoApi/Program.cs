using System.Text.Json;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using RecadastramentoApi.Authentication;
using RecadastramentoApi.Authorization;
using RecadastramentoApi.Configuration;
using RecadastramentoApi.Database;
using RecadastramentoApi.Export;
using RecadastramentoApi.Repositories;
using RecadastramentoApi.Services;
using RecadastramentoApi.Validators;

var builder = WebApplication.CreateBuilder(args);

DefaultTypeMap.MatchNamesWithUnderscores = true;
SqlMapper.AddTypeHandler(new JsonDictionaryTypeHandler());
SqlMapper.AddTypeHandler(new DateTimeTypeHandler());
SqlMapper.AddTypeHandler(new NullableDateTimeTypeHandler());

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(DatabaseOptions.SectionName));
builder.Services.Configure<ApiSecurityOptions>(builder.Configuration.GetSection(ApiSecurityOptions.SectionName));

builder.Services
    .AddAuthentication(AuthenticationConstants.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, ClientApiKeyAuthenticationHandler>(AuthenticationConstants.SchemeName, _ =>
    {
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CityAccess", policy =>
    {
        policy.AddAuthenticationSchemes(AuthenticationConstants.SchemeName);
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new CityAccessRequirement());
    });
});

builder.Services.AddSingleton<IAuthorizationHandler, CityAccessHandler>();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.DictionaryKeyPolicy = null;
    });

builder.Services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();
builder.Services.AddScoped<IPersonaRepository, PersonaRepository>();
builder.Services.AddScoped<IPersonaService, PersonaService>();
builder.Services.AddSingleton<IPersonaExportService, PersonaExportService>();
builder.Services.AddSingleton<IPersonaValidator, PersonaUpsertValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Personas",
        Version = "v1"
    });

    options.AddSecurityDefinition(AuthenticationConstants.ClientIdHeader, new OpenApiSecurityScheme
    {
        Name = AuthenticationConstants.ClientIdHeader,
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Client identifier"
    });

    options.AddSecurityDefinition(AuthenticationConstants.ApiKeyHeader, new OpenApiSecurityScheme
    {
        Name = AuthenticationConstants.ApiKeyHeader,
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Client API key"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = AuthenticationConstants.ClientIdHeader
                }
            },
            []
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = AuthenticationConstants.ApiKeyHeader
                }
            },
            []
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
