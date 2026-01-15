namespace LeaveRequestAPI.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public string ErrorCode { get; private set; } = string.Empty;

    private Result(bool isSuccess, T? data, string errorMessage, string errorCode)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }

    public static Result<T> Success(T data)
    {
        return new Result<T>(true, data, string.Empty, string.Empty);
    }

    public static Result<T> Failure(string errorMessage, string errorCode)
    {
        return new Result<T>(false, default, errorMessage, errorCode);
    }
}

// Versión sin tipo genérico para operaciones que no retornan datos
public class Result
{
    public bool IsSuccess { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public string ErrorCode { get; private set; } = string.Empty;

    private Result(bool isSuccess, string errorMessage, string errorCode)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }

    public static Result Success()
    {
        return new Result(true, string.Empty, string.Empty);
    }

    public static Result Failure(string errorMessage, string errorCode)
    {
        return new Result(false, errorMessage, errorCode);
    }
}
