using Microsoft.AspNetCore.Mvc;
using TrainMe.Core.DTOs;
using TrainMe.Core.Interfaces.Services;

// Controller xử lý các API tính toán chỉ số BMI (Body Mass Index)

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
    /// Khởi tạo controller với service tính toán BMI
    /// </summary>
    /// <param name="bmiService">Dịch vụ xử lý tính toán BMI</param>
    public BmiController(IBmiService bmiService)
    {
        _bmiService = bmiService ?? throw new ArgumentNullException(nameof(bmiService));
    }

    /// <summary>
    /// Tính toán chỉ số BMI và đánh giá tình trạng sức khỏe
    /// </summary>
    /// <param name="request">Yêu cầu tính BMI chứa chiều cao (m) và cân nặng (kg)</param>
    /// <returns>Kết quả tính toán BMI kèm phân loại và lời khuyên sức khỏe</returns>
    [HttpPost("calculate")]
    [ProducesResponseType(typeof(ApiResponse<BmiCalculationResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<BmiCalculationResponse>), 400)]
    [ProducesResponseType(500)]
    public IActionResult CalculateBmi([FromBody] BmiCalculationRequest request)
    {
        // Kiểm tra tính hợp lệ của dữ liệu đầu vào
        if (!ModelState.IsValid)
        {
            // Thu thập tất cả các thông báo lỗi từ ModelState
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            // Trả về lỗi 400 nếu dữ liệu không hợp lệ
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
    /// Lấy danh sách tất cả các mức phân loại BMI và khoảng giá trị tương ứng
    /// </summary>
    /// <returns>Danh sách các mức phân loại BMI</returns>
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
    /// Lấy thông tin phân loại BMI dựa trên giá trị BMI
    /// </summary>
    /// <param name="bmiValue">Giá trị BMI cần phân loại</param>
    /// <returns>Thông tin phân loại BMI</returns>
    [HttpGet("category/{bmiValue:double}")]
    [ProducesResponseType(typeof(ApiResponse<BmiCategoryInfo>), 200)]
    [ProducesResponseType(typeof(ApiResponse<BmiCategoryInfo>), 400)]
    [ProducesResponseType(500)]
    public IActionResult GetBmiCategory(double bmiValue)
    {
        // Kiểm tra giá trị BMI hợp lệ (phải lớn hơn 0)
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
    /// Tính toán nhanh BMI thông qua tham số truy vấn
    /// </summary>
    /// <param name="height">Chiều cao (đơn vị: mét)</param>
    /// <param name="weight">Cân nặng (đơn vị: kg)</param>
    /// <returns>Kết quả tính toán BMI</returns>
    [HttpGet("quick")]
    [ProducesResponseType(typeof(ApiResponse<BmiCalculationResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<BmiCalculationResponse>), 400)]
    [ProducesResponseType(500)]
    public IActionResult QuickCalculate(
        [FromQuery] double height,
        [FromQuery] double weight)
    {
        // Tạo đối tượng yêu cầu tính toán BMI
        var request = new BmiCalculationRequest(height, weight);

        // Kiểm tra ràng buộc về chiều cao
        if (height < 0.5 || height > 3.0)
        {
            return BadRequest(ApiResponse<BmiCalculationResponse>.ErrorResult(
                "Chiều cao phải từ 0.5m đến 3.0m"));
        }

        // Kiểm tra ràng buộc về cân nặng
        if (weight < 10 || weight > 500)
        {
            return BadRequest(ApiResponse<BmiCalculationResponse>.ErrorResult(
                "Cân nặng phải từ 10kg đến 500kg"));
        }

        // Gọi service để tính toán BMI
        var result = _bmiService.CalculateBmi(request);

        // Trả về kết quả tương ứng
        if (result.Success)
            return Ok(result);  // 200 OK nếu thành công

        return BadRequest(result);  // 400 BadRequest nếu có lỗi
    }
}
