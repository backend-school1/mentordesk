namespace MentorDesk.Application.Common;

public class Result
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }

    public static Result Success()
    {
        return new Result
        {
            IsSuccess = true
        };
    }

    public static Result Failure(string errorMessage)
    {
        return new Result
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

public class Result<T> : Result
{
    public T? Value { get; set; }
    
    public static Result<T> Success(T value)
    {
        return new Result<T>
        {
            Value = value,
            IsSuccess = true
        };
    }

    public static new Result<T> Failure(string errorMessage)
    {
        return new Result<T>()
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}
