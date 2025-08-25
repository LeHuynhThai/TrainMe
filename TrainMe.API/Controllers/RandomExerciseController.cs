using Microsoft.AspNetCore.Mvc;
using TrainMe.Core.Interfaces.Services;

namespace TrainMe.API.Controllers;

/// <summary>
/// Controller for RandomExercise operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RandomExerciseController : ControllerBase
{
    private readonly IRandomExerciseService _randomExerciseService;

    /// <summary>
    /// Hàm khởi tạo controller, inject IRandomExerciseService
    /// </summary>
    /// <param name="randomExerciseService">Service xử lý logic bài tập ngẫu nhiên</param>
    public RandomExerciseController(IRandomExerciseService randomExerciseService)
    {
        _randomExerciseService = randomExerciseService ?? throw new ArgumentNullException(nameof(randomExerciseService));
    }

    /// <summary>
    /// API lấy tất cả các bài tập ngẫu nhiên
    /// </summary>
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetAll()
    {
        // Gọi service lấy danh sách bài tập ngẫu nhiên
        var result = await _randomExerciseService.GetAllAsync();

        // Nếu thành công trả về kết quả
        if (result.Success)
            return Ok(result);

        // Nếu thất bại trả về lỗi server
        return StatusCode(500, result);
    }

    /// <summary>
    /// API lấy một bài tập ngẫu nhiên
    /// </summary>
    [HttpGet("random")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetRandom()
    {
        // Gọi service lấy một bài tập ngẫu nhiên
        var result = await _randomExerciseService.GetRandomAsync();

        // Nếu thành công trả về kết quả
        if (result.Success)
            return Ok(result);

        // Nếu không tìm thấy bài tập nào trả về 404
        if (result.Message?.Contains("No exercises found") == true)
            return NotFound(result);

        // Nếu lỗi server trả về 500
        return StatusCode(500, result);
    }

    /// <summary>
    /// API lấy nhiều bài tập ngẫu nhiên
    /// </summary>
    [HttpGet("random/{count:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetRandom(int count)
    {
        if (count <= 0)
            return BadRequest("Count must be greater than 0");

        var result = await _randomExerciseService.GetRandomAsync(count);

        if (result.Success)
            return Ok(result);

        return StatusCode(500, result);
    }
}
