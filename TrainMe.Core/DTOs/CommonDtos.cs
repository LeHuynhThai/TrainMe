using System.ComponentModel.DataAnnotations;

namespace TrainMe.Core.DTOs;

/// <summary>
/// DTO phản hồi API chung với dữ liệu kiểu generic
/// Cung cấp cấu trúc thống nhất cho tất cả API responses
/// </summary>
/// <typeparam name="T">Kiểu dữ liệu trả về</typeparam>
/// <param name="Success">Trạng thái thành công/thất bại của request</param>
/// <param name="Message">Thông báo mô tả kết quả</param>
/// <param name="Data">Dữ liệu trả về (nếu có)</param>
/// <param name="Errors">Danh sách lỗi chi tiết (nếu có)</param>
public record ApiResponse<T>(bool Success, string Message, T? Data = default, List<string>? Errors = null)
{
    /// <summary>
    /// Tạo response thành công với dữ liệu
    /// </summary>
    /// <param name="data">Dữ liệu cần trả về</param>
    /// <param name="message">Thông báo thành công</param>
    /// <returns>ApiResponse với trạng thái thành công</returns>
    public static ApiResponse<T> SuccessResult(T data, string message = "Thành công") =>
        new(true, message, data);

    /// <summary>
    /// Tạo response lỗi với thông báo
    /// </summary>
    /// <param name="message">Thông báo lỗi</param>
    /// <param name="errors">Danh sách lỗi chi tiết</param>
    /// <returns>ApiResponse với trạng thái lỗi</returns>
    public static ApiResponse<T> ErrorResult(string message, List<string>? errors = null) =>
        new(false, message, default, errors);
}

/// <summary>
/// DTO phản hồi API chung không có dữ liệu trả về
/// Sử dụng cho các operations chỉ cần biết thành công/thất bại
/// </summary>
/// <param name="Success">Trạng thái thành công/thất bại của request</param>
/// <param name="Message">Thông báo mô tả kết quả</param>
/// <param name="Errors">Danh sách lỗi chi tiết (nếu có)</param>
public record ApiResponse(bool Success, string Message, List<string>? Errors = null) : ApiResponse<object>(Success, Message, null, Errors)
{
    /// <summary>
    /// Tạo response thành công không có dữ liệu
    /// </summary>
    /// <param name="message">Thông báo thành công</param>
    /// <returns>ApiResponse với trạng thái thành công</returns>
    public static ApiResponse SuccessResult(string message = "Thành công") =>
        new(true, message);

    /// <summary>
    /// Tạo response lỗi không có dữ liệu
    /// </summary>
    /// <param name="message">Thông báo lỗi</param>
    /// <param name="errors">Danh sách lỗi chi tiết</param>
    /// <returns>ApiResponse với trạng thái lỗi</returns>
    public static new ApiResponse ErrorResult(string message, List<string>? errors = null) =>
        new(false, message, errors);
}

// DTOs cho tính năng BMI Calculator

/// <summary>
/// DTO yêu cầu tính toán chỉ số BMI
/// Chứa thông tin chiều cao và cân nặng để tính BMI
/// </summary>
/// <param name="Height">Chiều cao tính bằng mét (0.5-3.0m)</param>
/// <param name="Weight">Cân nặng tính bằng kg (10-500kg)</param>
public record BmiCalculationRequest(
    [Required, Range(0.5, 3.0, ErrorMessage = "Chiều cao phải từ 0.5m đến 3.0m")]
    double Height,

    [Required, Range(10, 500, ErrorMessage = "Cân nặng phải từ 10kg đến 500kg")]
    double Weight);

/// <summary>
/// DTO phản hồi kết quả tính toán BMI
/// Chứa đầy đủ thông tin về chỉ số BMI và lời khuyên sức khỏe
/// </summary>
/// <param name="Height">Chiều cao đã nhập (mét)</param>
/// <param name="Weight">Cân nặng đã nhập (kg)</param>
/// <param name="BmiValue">Chỉ số BMI được tính toán</param>
/// <param name="Category">Phân loại BMI (Thiếu cân, Bình thường, Thừa cân, etc.)</param>
/// <param name="Description">Mô tả chi tiết về tình trạng sức khỏe</param>
/// <param name="HealthAdvice">Lời khuyên sức khỏe dựa trên BMI</param>
/// <param name="CalculatedAt">Thời gian thực hiện tính toán</param>
public record BmiCalculationResponse(
    double Height,
    double Weight,
    double BmiValue,
    string Category,
    string Description,
    string HealthAdvice,
    DateTime CalculatedAt);

/// <summary>
/// DTO thông tin về các phân loại BMI
/// Sử dụng để hiển thị bảng tham khảo các mức BMI
/// </summary>
/// <param name="Category">Tên phân loại (Thiếu cân nghiêm trọng, Thiếu cân, etc.)</param>
/// <param name="Description">Mô tả chi tiết về phân loại</param>
/// <param name="MinBmi">Giá trị BMI tối thiểu của phân loại</param>
/// <param name="MaxBmi">Giá trị BMI tối đa của phân loại (null nếu không giới hạn)</param>
/// <param name="HealthAdvice">Lời khuyên sức khỏe cho phân loại này</param>
public record BmiCategoryInfo(
    string Category,
    string Description,
    double MinBmi,
    double? MaxBmi,
    string HealthAdvice);
