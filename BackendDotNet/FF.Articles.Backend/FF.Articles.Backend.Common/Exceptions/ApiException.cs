namespace FF.Articles.Backend.Common.Exceptions;

public class ApiException : Exception
{
    public int Code { get; }

    public ApiException(ErrorCode errorCode) : base(errorCode.Message)
    {
        Code = errorCode.Code;
    }

    public ApiException(ErrorCode errorCode, string message) : base(message)
    {
        Code = errorCode.Code;
    }
}