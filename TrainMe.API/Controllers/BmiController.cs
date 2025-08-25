using Microsoft.AspNetCore.Mvc;
using TrainMe.Core.DTOs;
using TrainMe.Core.Interfaces.Services;

namespace TrainMe.API.Controllers;

/// <summary>
/// Controller xử lý các API tính toán chỉ số BMI và đánh giá sức khỏe
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BmiController : ControllerBase
{
    // Service xử lý logic tính toán BMI
    private readonly IBmiService _bmiService;

    /// <summary>
    /// Hàm khởi tạo controller, inject IBmiService
    /// </summary>
    /// <param name="bmiService">Service tính toán BMI</param>
    public BmiController(IBmiService bmiService)
    {
        // Kiểm tra null và gán giá trị cho _bmiService
        _bmiService = bmiService ?? throw new ArgumentNullException(nameof(bmiService));
    }

    /// <summary>
    /// API tính toán chỉ số BMI và đánh giá sức khỏe
    /// </summary>
    /// <param name="request">Yêu cầu tính BMI với chiều cao (m) và cân nặng (kg)</param>
    /// <returns>Kết quả tính BMI kèm phân loại và lời khuyên sức khỏe</returns>
    [HttpPost("calculate")]
    [ProducesResponseType(typeof(ApiResponse<BmiCalculationResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<BmiCalculationResponse>), 400)]
    [ProducesResponseType(500)]
    public IActionResult CalculateBmi([FromBody] BmiCalculationRequest request)
    {
        // Kiểm tra dữ liệu đầu vào
        if (!ModelState.IsValid)
        {
            // Lấy danh sách lỗi từ ModelState
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            // Trả về lỗi dữ liệu không hợp lệ
            return BadRequest(ApiResponse<BmiCalculationResponse>.ErrorResult(
                "Dữ liệu không hợp lệ", errors));
        }

        // Gọi service để tính toán BMI
        var result = _bmiService.CalculateBmi(request);

        // Nếu thành công trả về kết quả
        if (result.Success)
            return Ok(result);

        // Nếu thất bại trả về lỗi
        return BadRequest(result);
    }

    /// <summary>
    /// Gets all BMI categories with their ranges and descriptions
    /// </summary>
    /// <returns>List of BMI categories</returns>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BmiCategoryInfo>>), 200)]
    [ProducesResponseType(500)]
    public IActionResult GetBmiCategories()
    {
        var result = _bmiService.GetBmiCategories();

        if (result.Success)
            return Ok(result);

        return StatusCode(500, result);
    }

    /// <summary>
    /// Gets BMI category for a specific BMI value
    /// </summary>
    /// <param name="bmiValue">BMI value to categorize</param>
    /// <returns>BMI category information</returns>
    [HttpGet("category/{bmiValue:double}")]
    [ProducesResponseType(typeof(ApiResponse<BmiCategoryInfo>), 200)]
    [ProducesResponseType(typeof(ApiResponse<BmiCategoryInfo>), 400)]
    [ProducesResponseType(500)]
    public IActionResult GetBmiCategory(double bmiValue)
    {
        if (bmiValue <= 0)
        {
            return BadRequest(ApiResponse<BmiCategoryInfo>.ErrorResult(
                "Giá trị BMI phải lớn hơn 0"));
        }

        var result = _bmiService.GetBmiCategory(bmiValue);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    /// <summary>
    /// Quick BMI calculation via query parameters
    /// </summary>
    /// <param name="height">Height in meters</param>
    /// <param name="weight">Weight in kilograms</param>
    /// <returns>BMI calculation result</returns>
    [HttpGet("quick")]
    [ProducesResponseType(typeof(ApiResponse<BmiCalculationResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<BmiCalculationResponse>), 400)]
    [ProducesResponseType(500)]
    public IActionResult QuickCalculate(
        [FromQuery] double height,
        [FromQuery] double weight)
    {
        var request = new BmiCalculationRequest(height, weight);

        // Manual validation for query parameters
        if (height < 0.5 || height > 3.0)
        {
            return BadRequest(ApiResponse<BmiCalculationResponse>.ErrorResult(
                "Chiều cao phải từ 0.5m đến 3.0m"));
        }

        if (weight < 10 || weight > 500)
        {
            return BadRequest(ApiResponse<BmiCalculationResponse>.ErrorResult(
                "Cân nặng phải từ 10kg đến 500kg"));
        }

        var result = _bmiService.CalculateBmi(request);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }
}
