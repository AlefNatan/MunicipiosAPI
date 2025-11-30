using System.Net;
using System.Text.Json;

namespace MunicipiosAPI.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception caught by middleware.");

            var (status, message) = MapExceptionToResponse(ex);

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(
                new
                {
                    error = message,
                    status = status
                },
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            await context.Response.WriteAsync(result);
        }
    }

    private static (int statusCode, string message) MapExceptionToResponse(Exception ex)
    {
        return ex switch
        {
            KeyNotFoundException => ((int)HttpStatusCode.NotFound,
                ex.Message ?? "Recurso não encontrado."),

            ArgumentException => ((int)HttpStatusCode.BadRequest,
                ex.Message ?? "Parâmetros inválidos."),

            UnauthorizedAccessException => ((int)HttpStatusCode.Unauthorized,
                ex.Message ?? "Acesso não autorizado."),

            InvalidOperationException => ((int)HttpStatusCode.Conflict,
                ex.Message ?? "Operação inválida."),

            _ => ((int)HttpStatusCode.InternalServerError,
                "Erro interno no servidor.")
        };
    }
}
