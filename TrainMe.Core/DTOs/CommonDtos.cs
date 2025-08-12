namespace TrainMe.Core.DTOs;

public record ApiResponse<T>(bool Success, string Message, T? Data = default, List<string>? Errors = null)
{
    public static ApiResponse<T> SuccessResult(T data, string message = "Thành công") =>
        new(true, message, data);

    public static ApiResponse<T> ErrorResult(string message, List<string>? errors = null) =>
        new(false, message, default, errors);
}

public record ApiResponse(bool Success, string Message, List<string>? Errors = null) : ApiResponse<object>(Success, Message, null, Errors)
{
    public static ApiResponse SuccessResult(string message = "Thành công") =>
        new(true, message);

    public static new ApiResponse ErrorResult(string message, List<string>? errors = null) =>
        new(false, message, errors);
}
