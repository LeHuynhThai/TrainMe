using Microsoft.AspNetCore.Mvc;
using TrainMe.Core.DTOs;
using TrainMe.Core.Interfaces.Services;

namespace TrainMe.API.Controllers;

/// <summary>
/// Controller for BMI calculations and health assessments
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BmiController : ControllerBase
{
    private readonly IBmiService _bmiService;

    public BmiController(IBmiService bmiService)
    {
        _bmiService = bmiService ?? throw new ArgumentNullException(nameof(bmiService));
    }

    /// <summary>
    /// Calculates BMI and provides health assessment
    /// </summary>
    /// <param name="request">BMI calculation request with height (m) and weight (kg)</param>
    /// <returns>BMI calculation result with category and health advice</returns>
    [HttpPost("calculate")]
    [ProducesResponseType(typeof(ApiResponse<BmiCalculationResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<BmiCalculationResponse>), 400)]
    [ProducesResponseType(500)]
    public IActionResult CalculateBmi([FromBody] BmiCalculationRequest request)
    {
        // Validate model state
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(ApiResponse<BmiCalculationResponse>.ErrorResult(
                "Dữ liệu không hợp lệ", errors));
        }

        var result = _bmiService.CalculateBmi(request);

        if (result.Success)
            return Ok(result);

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
