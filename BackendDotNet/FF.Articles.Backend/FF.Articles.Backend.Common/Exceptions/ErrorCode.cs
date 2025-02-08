using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF.Articles.Backend.Common.Exceptions;

public class ErrorCode
{
    public static readonly ErrorCode PARAMS_ERROR = new ErrorCode(40000, "Request parameter error");
    public static readonly ErrorCode NOT_LOGIN_ERROR = new ErrorCode(40100, "Not logged in");
    public static readonly ErrorCode NO_AUTH_ERROR = new ErrorCode(40101, "No permission");
    public static readonly ErrorCode NOT_FOUND_ERROR = new ErrorCode(40400, "Requested data not found");
    public static readonly ErrorCode FORBIDDEN_ERROR = new ErrorCode(40300, "Access forbidden");
    public static readonly ErrorCode SYSTEM_ERROR = new ErrorCode(50000, "Internal system exception");
    public static readonly ErrorCode OPERATION_ERROR = new ErrorCode(50001, "Operation failure");
    public static readonly ErrorCode CONCURRENCY_ERROR = new ErrorCode(50001, "The record was modified");

    public int Code { get; }
    public string Message { get; }

    private ErrorCode(int code, string message)
    {
        Code = code;
        Message = message;
    }
}
