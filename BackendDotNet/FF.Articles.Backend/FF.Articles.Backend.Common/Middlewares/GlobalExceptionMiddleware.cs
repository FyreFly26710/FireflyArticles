using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using FF.Articles.Backend.Common.Exceptions;
using Azure;
using FF.Articles.Backend.Common.Responses;

namespace FF.Articles.Backend.Common.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestId = Activity.Current?.Id ?? context.TraceIdentifier;

            try
            {
                context.Response.Headers.Add("X-Request-Id", requestId);
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, requestId);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, string requestId)
        {
            context.Response.ContentType = "application/json";
            var response = new ApiResponse<bool>();
            var log = $"Exception during request {requestId} - {context.Request.Path}: {exception.Message}" + Environment.NewLine;
            // Determine the error code based on exception type
            if (exception is ApiException apiEx)
            {
                response = new ApiResponse<bool>(apiEx);
                log += $"API Exception: {apiEx.Code} - {apiEx.Message}";
                _logger.LogError(log);
            }
            else
            {
                // Default to system error for all non-API exceptions
                response = new ApiResponse<bool>(ErrorCode.SYSTEM_ERROR);
                log += $"Unhandled exception: {exception.Message}";
                _logger.LogError(log);
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var result = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(result);
        }

    }

    public class ErrorResponse
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string RequestId { get; set; } = string.Empty;
    }
}