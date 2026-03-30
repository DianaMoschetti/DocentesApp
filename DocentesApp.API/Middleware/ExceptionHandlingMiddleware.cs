using System.Diagnostics;
using System.Net;
using System.Text.Json;
using DocentesApp.Application.Common.Constants;
using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.Common.Responses;
using Microsoft.AspNetCore.Http;

namespace DocentesApp.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
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
                await HandleExceptionAsync(context, ex, _logger);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context,Exception exception, ILogger logger)
        {
            context.Response.ContentType = "application/json";

            var traceId = context.TraceIdentifier;

            ApiErrorResponse response;
            int statusCode;

            switch (exception)
            {
                case BadRequestException badRequestException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    logger.LogWarning(exception,
                        "{Message} TraceId: {TraceId}",
                        UserMessages.BadRequest,
                        traceId);

                    response = new ApiErrorResponse
                    {
                        StatusCode = statusCode,
                        Message = badRequestException.Message,
                        TraceId = traceId
                    };
                    break;

                case NotFoundException notFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    logger.LogWarning(exception,
                        "{Message} TraceId: {TraceId}",
                        UserMessages.NotFound,
                        traceId);

                    response = new ApiErrorResponse
                    {
                        StatusCode = statusCode,
                        Message = notFoundException.Message,
                        TraceId = traceId
                    };
                    break;

                case ConflictException conflictException:
                    statusCode = (int)HttpStatusCode.Conflict;
                    logger.LogWarning(exception,
                        "{Message} TraceId: {TraceId}",
                        UserMessages.Conflict,
                        traceId);

                    response = new ApiErrorResponse
                    {
                        StatusCode = statusCode,
                        Message = conflictException.Message,
                        TraceId = traceId
                    };
                    break;

                case UnauthorizedAppException unauthorizedException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    logger.LogWarning(exception,
                        "{Message} TraceId: {TraceId}",
                        UserMessages.Unauthorized,
                        traceId);

                    response = new ApiErrorResponse
                    {
                        StatusCode = statusCode,
                        Message = unauthorizedException.Message,
                        TraceId = traceId
                    };
                    break;

                case ForbiddenException forbiddenException:
                    statusCode = (int)HttpStatusCode.Forbidden;
                    logger.LogWarning(exception,
                        "{Message} TraceId: {TraceId}",
                        UserMessages.Forbidden,
                        traceId);

                    response = new ApiErrorResponse
                    {
                        StatusCode = statusCode,
                        Message = forbiddenException.Message,
                        TraceId = traceId
                    };
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    logger.LogError(exception,
                        "{Message} TraceId: {TraceId}",
                        UserMessages.Unhandled,
                        traceId);

                    response = new ApiErrorResponse
                    {
                        StatusCode = statusCode,
                        Message = UserMessages.Error500,
                        Details = exception.Message,
                        TraceId = traceId
                    };
                    break;
            }

            context.Response.StatusCode = statusCode;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
