using System.Net;
using Ardalis.Result;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace THSocialMedia.Api.Middlawares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            string errorMessage = ex.Message;
            var httpCode = HttpStatusCode.InternalServerError;
            // Set the status code to reflect the error
            httpContext.Response.StatusCode = (int)httpCode;

            await HandleExceptionAsync(
                httpContext,
                httpCode,
                errorMessage
            );
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, HttpStatusCode statusCode, string message)
    {
        var response = httpContext.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)statusCode;

        var result = Result.Error(message);

        await response.WriteAsync(JsonConvert.SerializeObject(result, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        }));
    }

}
