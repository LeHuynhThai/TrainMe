using System.ComponentModel.DataAnnotations;

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

// BMI DTOs
public record BmiCalculationRequest(
    [Required, Range(0.5, 3.0, ErrorMessage = "Chiều cao phải từ 0.5m đến 3.0m")]
    double Height,

    [Required, Range(10, 500, ErrorMessage = "Cân nặng phải từ 10kg đến 500kg")]
    double Weight);

public record BmiCalculationResponse(
    double Height,
    double Weight,
    double BmiValue,
    string Category,
    string Description,
    string HealthAdvice,
    DateTime CalculatedAt);

public record BmiCategoryInfo(
    string Category,
    string Description,
    double MinBmi,
    double? MaxBmi,
    string HealthAdvice);
