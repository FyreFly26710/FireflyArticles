using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF.Articles.Backend.Common.Exceptions;

/// <summary>
/// Centralized error codes for the application
/// </summary>
public class ErrorCode
{
    // 4xx Client Errors
    // 400: Bad Request
    public static readonly ErrorCode PARAMS_ERROR = new ErrorCode(40000, "Request parameter error");
    public static readonly ErrorCode VALIDATION_ERROR = new ErrorCode(40001, "Request validation failed");
    public static readonly ErrorCode INVALID_FORMAT = new ErrorCode(40002, "Invalid data format");

    // 401: Unauthorized
    public static readonly ErrorCode NOT_LOGIN_ERROR = new ErrorCode(40100, "Not logged in");
    public static readonly ErrorCode INVALID_TOKEN = new ErrorCode(40101, "Invalid token");
    public static readonly ErrorCode TOKEN_EXPIRED = new ErrorCode(40102, "Token expired");

    // 403: Forbidden
    public static readonly ErrorCode NO_AUTH_ERROR = new ErrorCode(40301, "No permission");
    public static readonly ErrorCode FORBIDDEN_ERROR = new ErrorCode(40300, "Access forbidden");

    // 404: Not Found
    public static readonly ErrorCode NOT_FOUND_ERROR = new ErrorCode(40400, "Requested data not found");
    public static readonly ErrorCode RESOURCE_NOT_FOUND = new ErrorCode(40401, "Resource not found");
    public static readonly ErrorCode ENDPOINT_NOT_FOUND = new ErrorCode(40404, "Endpoint not found");

    // 409: Conflict
    public static readonly ErrorCode CONCURRENCY_ERROR = new ErrorCode(40900, "The record was modified");
    public static readonly ErrorCode DUPLICATE_RESOURCE = new ErrorCode(40901, "Resource already exists");

    // 429: Too Many Requests
    public static readonly ErrorCode RATE_LIMIT_EXCEEDED = new ErrorCode(42900, "Rate limit exceeded");

    // 5xx Server Errors
    // 500: Internal Server Error
    public static readonly ErrorCode SYSTEM_ERROR = new ErrorCode(50000, "Internal system exception");
    public static readonly ErrorCode OPERATION_ERROR = new ErrorCode(50001, "Operation failure");
    public static readonly ErrorCode DATABASE_ERROR = new ErrorCode(50002, "Database operation error");

    // 502: Bad Gateway
    public static readonly ErrorCode SERVICE_UNAVAILABLE = new ErrorCode(50200, "External service unavailable");

    // 503: Service Unavailable
    public static readonly ErrorCode MAINTENANCE_MODE = new ErrorCode(50300, "System in maintenance mode");

    // 504: Gateway Timeout
    public static readonly ErrorCode TIMEOUT_ERROR = new ErrorCode(50400, "Operation timed out");

    /// <summary>
    /// Numeric error code
    /// </summary>
    public int Code { get; }

    /// <summary>
    /// Human-readable error message
    /// </summary>
    public string Message { get; }

    private ErrorCode(int code, string message)
    {
        Code = code;
        Message = message;
    }
}
