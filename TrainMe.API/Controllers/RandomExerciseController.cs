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

    public RandomExerciseController(IRandomExerciseService randomExerciseService)
    {
        _randomExerciseService = randomExerciseService ?? throw new ArgumentNullException(nameof(randomExerciseService));
    }

    /// <summary>
    /// Gets all random exercises
    /// </summary>
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _randomExerciseService.GetAllAsync();

        if (result.Success)
            return Ok(result);

        return StatusCode(500, result);
    }

    /// <summary>
    /// Gets a random exercise
    /// </summary>
    [HttpGet("random")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetRandom()
    {
        var result = await _randomExerciseService.GetRandomAsync();

        if (result.Success)
            return Ok(result);

        if (result.Message?.Contains("No exercises found") == true)
            return NotFound(result);

        return StatusCode(500, result);
    }

    /// <summary>
    /// Gets multiple random exercises
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
