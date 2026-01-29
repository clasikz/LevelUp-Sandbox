using LevelUpSandbox.Api.Controllers;
using LevelUpSandbox.Core.Entities;
using LevelUpSandbox.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LevelUpSandbox.Tests.Controllers;

public class EvaluationControllerTests
{
    private readonly Mock<ICodeEvaluationService> _mockService;
    private readonly EvaluationController _controller;

    public EvaluationControllerTests()
    {
        _mockService = new Mock<ICodeEvaluationService>();
        _controller = new EvaluationController(_mockService.Object);
    }

    [Fact]
    [Trait("TestType", "Positive")]
    [Trait("Category", "Unit")]
    [Trait("Component", "Controller")]
    public async Task EvaluateCode_ValidRequest_ReturnsOkResult()
    {
        _mockService.Setup(s => s.EvaluateCodeAsync(It.IsAny<Submission>(), It.IsAny<Exercise>()))
            .ReturnsAsync(new Evaluation
            {
                Id = Guid.NewGuid(),
                Score = 95,
                Remarks = "Great job",
                ImprovedSolution = "",
                DetectedLevel = DifficultyLevel.Senior,
                EvaluatedAt = DateTime.UtcNow
            });

        var request = new EvaluateRequest(
            Guid.NewGuid(),
            "Test Exercise",
            "Test Description",
            DifficultyLevel.Junior,
            "var x = 1;",
            "C#"
        );

        var result = await _controller.EvaluateCode(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var evaluation = Assert.IsType<Evaluation>(okResult.Value);
        Assert.Equal(95, evaluation.Score);
    }

    [Fact]
    [Trait("TestType", "Negative")]
    [Trait("Category", "Unit")]
    [Trait("Component", "Controller")]
    public async Task EvaluateCode_PlaceholderValues_ReturnsBadRequest()
    {
        _mockService.Setup(s => s.EvaluateCodeAsync(It.IsAny<Submission>(), It.IsAny<Exercise>()))
            .ThrowsAsync(new ArgumentException("Placeholder or empty values detected. Provide actual exercise details and code."));

        var request = new EvaluateRequest(
            Guid.NewGuid(),
            "string",
            "string",
            DifficultyLevel.Junior,
            "string",
            "string"
        );

        var result = await _controller.EvaluateCode(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }
}