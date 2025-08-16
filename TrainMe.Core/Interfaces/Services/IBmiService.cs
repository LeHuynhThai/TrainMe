using TrainMe.Core.DTOs;

namespace TrainMe.Core.Interfaces.Services;

/// <summary>
/// Service interface for BMI calculations and health assessments
/// </summary>
public interface IBmiService
{
    /// <summary>
    /// Calculates BMI and provides health assessment
    /// </summary>
    /// <param name="request">BMI calculation request with height and weight</param>
    /// <returns>BMI calculation result with category and health advice</returns>
    ApiResponse<BmiCalculationResponse> CalculateBmi(BmiCalculationRequest request);

    /// <summary>
    /// Gets all BMI categories with their ranges and descriptions
    /// </summary>
    /// <returns>List of BMI categories</returns>
    ApiResponse<IEnumerable<BmiCategoryInfo>> GetBmiCategories();

    /// <summary>
    /// Gets BMI category for a specific BMI value
    /// </summary>
    /// <param name="bmiValue">BMI value to categorize</param>
    /// <returns>BMI category information</returns>
    ApiResponse<BmiCategoryInfo> GetBmiCategory(double bmiValue);
}
