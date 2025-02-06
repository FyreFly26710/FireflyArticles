using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF.Articles.Backend.Common.Utils;
public static class ResultUtils
{
    public static ApiResponse<T> Success<T>(T data) => new(0, data, "ok");
    //public static ApiResponse<T> Error<T>(int code, string message) => new(code, default, message);
    public static ApiResponse<T> Error<T>(ErrorCode errorCode) => new(errorCode.Code, default, errorCode.Message);
    public static ApiResponse<T> Error<T>(ErrorCode errorCode, string message) => new(errorCode.Code, default, message);

}