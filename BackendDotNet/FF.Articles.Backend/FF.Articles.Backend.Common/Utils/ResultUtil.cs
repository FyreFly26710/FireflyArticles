namespace FF.Articles.Backend.Common.Utils;

/// <summary>
/// Return result util class
/// </summary>
public static class ResultUtil
{
    public static ApiResponse<T> Success<T>(T data) => new(200, data, "ok");
    //public static ApiResponse<T> Error<T>(int code, string message) => new(code, default, message);
    // public static ApiResponse<T> Error<T>(ErrorCode errorCode) => new(errorCode.Code, default, errorCode.Message);
    // public static ApiResponse<T> Error<T>(ErrorCode errorCode, string message) => new(errorCode.Code, default, message);

}