using Microsoft.AspNetCore.Mvc;
using LevelUpSandbox.Core.Entities;
using LevelUpSandbox.Core.Interfaces;

namespace LevelUpSandbox.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EvaluationController : ControllerBase
{
    private readonly ICodeEvaluationService _evaluationService;

    public EvaluationController(ICodeEvaluationService evaluationService)
    {
        _evaluationService = evaluationService;
    }

    [HttpPost("evaluate")]
    public async Task<IActionResult> EvaluateCode([FromBody] EvaluateRequest request)
    {
        try
        {
            var submission = new Submission
            {
                Id = Guid.NewGuid(),
                ExerciseId = request.ExerciseId,
                UserCode = request.UserCode,
                Language = request.Language,
                SubmittedAt = DateTime.UtcNow
            };

            var exercise = new Exercise
            {
                Id = request.ExerciseId,
                Title = request.ExerciseTitle,
                Description = request.ExerciseDescription,
                Difficulty = request.Difficulty,
                TestCases = Array.Empty<string>(),
                CreatedAt = DateTime.UtcNow
            };

            var evaluation = await _evaluationService.EvaluateCodeAsync(submission, exercise);
            return Ok(evaluation);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

public record EvaluateRequest(
    Guid ExerciseId,
    string ExerciseTitle,
    string ExerciseDescription,
    DifficultyLevel Difficulty,
    string UserCode,
    string Language
);