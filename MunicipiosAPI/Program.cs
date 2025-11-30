using Microsoft.OpenApi;
using MunicipiosAPI.Middlewares;
using MunicipiosAPI.Domain.Enums;
using MunicipiosAPI.Domain.Interfaces;
using MunicipiosAPI.Service.Interfaces;
using MunicipiosAPI.Service.Services;
using MunicipiosAPI.Providers.Providers;

var builder = WebApplication.CreateBuilder(args);

// Controllers + JSON
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Municipios API",
        Version = "v1",
        Description = "API para listar municípios por UF usando diferentes providers."
    });
});

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Cache
builder.Services.AddMemoryCache();

// Service
builder.Services.AddScoped<IMunicipiosService, MunicipiosService>();

// Environment variable 
var providerEnv = Environment.GetEnvironmentVariable("PROVIDER_MUNICIPIOS");
if (string.IsNullOrWhiteSpace(providerEnv))
    throw new Exception("A variável de ambiente PROVIDER_MUNICIPIOS não foi configurada.");

var providerType = providerEnv.ToUpper() switch
{
    "BRASIL_API" => MunicipiosProviderType.BrasilApi,
    "IBGE_API" => MunicipiosProviderType.IBGEApi,
    _ => throw new Exception("Valor inválido para PROVIDER_MUNICIPIOS. Use BRASIL_API ou IBGE_API.")
};

// Register selected provider
switch (providerType)
{
    case MunicipiosProviderType.BrasilApi:
        builder.Services.AddHttpClient<IProviderMunicipios, BrasilApiMunicipiosProvider>();
        break;

    case MunicipiosProviderType.IBGEApi:
        builder.Services.AddHttpClient<IProviderMunicipios, IbgeMunicipiosProvider>();
        break;
}

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware de exceções
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
