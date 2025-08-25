using TrainMe.Core.DTOs;
using TrainMe.Core.Interfaces.Services;

namespace TrainMe.Services;

/// <summary>
/// Service tính chỉ số BMI (Body Mass Index) và đánh giá sức khỏe.
/// </summary>
public class BmiService : IBmiService
{
    private static readonly List<BmiCategoryInfo> BmiCategories = new()
    {
        new BmiCategoryInfo(
            "Thiếu cân nghiêm trọng",
            "Cân nặng quá thấp so với chiều cao",
            0,
            16,
            "Nên tăng cân và tham khảo ý kiến bác sĩ. Tăng cường dinh dưỡng và tập luyện phù hợp."
        ),
        new BmiCategoryInfo(
            "Thiếu cân",
            "Cân nặng thấp hơn mức bình thường",
            16,
            18.5,
            "Nên tăng cân bằng chế độ ăn uống lành mạnh và tập luyện thể dục."
        ),
        new BmiCategoryInfo(
            "Bình thường",
            "Cân nặng lý tưởng cho sức khỏe",
            18.5,
            25,
            "Duy trì lối sống lành mạnh với chế độ ăn cân bằng và tập thể dục đều đặn."
        ),
        new BmiCategoryInfo(
            "Thừa cân",
            "Cân nặng cao hơn mức bình thường",
            25,
            30,
            "Nên giảm cân bằng chế độ ăn ít calo và tăng cường hoạt động thể chất."
        ),
        new BmiCategoryInfo(
            "Béo phì độ I",
            "Béo phì mức độ nhẹ",
            30,
            35,
            "Cần giảm cân nghiêm túc. Tham khảo chuyên gia dinh dưỡng và tập luyện đều đặn."
        ),
        new BmiCategoryInfo(
            "Béo phì độ II",
            "Béo phì mức độ vừa",
            35,
            40,
            "Cần can thiệp y tế. Tham khảo bác sĩ về kế hoạch giảm cân an toàn."
        ),
        new BmiCategoryInfo(
            "Béo phì độ III",
            "Béo phì nghiêm trọng",
            40,
            null,
            "Cần can thiệp y tế khẩn cấp. Tham khảo bác sĩ chuyên khoa về các phương pháp điều trị."
        )
    };

    /// <summary>
    /// Tính BMI và đưa ra đánh giá sức khỏe.
    /// </summary>
    /// <param name="request">Dữ liệu đầu vào gồm chiều cao (m) và cân nặng (kg)</param>
    /// <returns>Kết quả gồm giá trị BMI và lời khuyên sức khỏe</returns>
    public ApiResponse<BmiCalculationResponse> CalculateBmi(BmiCalculationRequest request)
    {
        try
        {
            // Kiểm tra đầu vào
            if (request.Height <= 0 || request.Weight <= 0)
            {
                return ApiResponse<BmiCalculationResponse>.ErrorResult(
                    "Chiều cao và cân nặng phải lớn hơn 0");
            }

            // Tính BMI: cân nặng (kg) / (chiều cao (m))^2
            var bmiValue = Math.Round(request.Weight / (request.Height * request.Height), 2);

            // Xác định phân loại BMI tương ứng
            var categoryResponse = GetBmiCategory(bmiValue);
            if (!categoryResponse.Success || categoryResponse.Data == null)
            {
                return ApiResponse<BmiCalculationResponse>.ErrorResult(
                    "Không thể xác định phân loại BMI");
            }

            var category = categoryResponse.Data;

            // Tạo đối tượng phản hồi (response)
            var response = new BmiCalculationResponse(
                Height: request.Height,
                Weight: request.Weight,
                BmiValue: bmiValue,
                Category: category.Category,
                Description: category.Description,
                HealthAdvice: category.HealthAdvice,
                CalculatedAt: DateTime.UtcNow
            );

            return ApiResponse<BmiCalculationResponse>.SuccessResult(
                response,
                $"BMI của bạn là {bmiValue} - {category.Category}");
        }
        catch (Exception ex)
        {
            return ApiResponse<BmiCalculationResponse>.ErrorResult(
                $"Lỗi khi tính BMI: {ex.Message}");
        }
    }

    /// <summary>
    /// Lấy tất cả phân loại BMI cùng khoảng giá trị và mô tả.
    /// </summary>
    /// <returns>Danh sách phân loại BMI</returns>
    public ApiResponse<IEnumerable<BmiCategoryInfo>> GetBmiCategories()
    {
        try
        {
            return ApiResponse<IEnumerable<BmiCategoryInfo>>.SuccessResult(
                BmiCategories,
                "Lấy danh sách phân loại BMI thành công");
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<BmiCategoryInfo>>.ErrorResult(
                $"Lỗi khi lấy danh sách phân loại BMI: {ex.Message}");
        }
    }

    /// <summary>
    /// Lấy thông tin phân loại BMI cho một giá trị BMI cụ thể.
    /// </summary>
    /// <param name="bmiValue">Giá trị BMI đã tính</param>
    /// <returns>Thông tin phân loại BMI tương ứng</returns>
    public ApiResponse<BmiCategoryInfo> GetBmiCategory(double bmiValue)
    {
        try
        {
            var category = BmiCategories.FirstOrDefault(c =>
                bmiValue >= c.MinBmi && (c.MaxBmi == null || bmiValue < c.MaxBmi));

            if (category == null)
            {
                return ApiResponse<BmiCategoryInfo>.ErrorResult(
                    "Không tìm thấy phân loại BMI phù hợp");
            }

            return ApiResponse<BmiCategoryInfo>.SuccessResult(
                category,
                "Xác định phân loại BMI thành công");
        }
        catch (Exception ex)
        {
            return ApiResponse<BmiCategoryInfo>.ErrorResult(
                $"Lỗi khi xác định phân loại BMI: {ex.Message}");
        }
    }
}
