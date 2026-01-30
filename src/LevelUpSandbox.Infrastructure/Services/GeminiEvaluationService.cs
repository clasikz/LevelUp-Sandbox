using System.Net.Http.Json;
using System.Text.Json;
using LevelUpSandbox.Core.Entities;
using LevelUpSandbox.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace LevelUpSandbox.Infrastructure.Services;

public class GeminiEvaluationService : ICodeEvaluationService
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;
    private readonly string? _model;

    public GeminiEvaluationService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _model = configuration["Gemini:Model"];
        _apiKey = configuration["Gemini:ApiKey"];
    }

    public async Task<Evaluation> EvaluateCodeAsync(Submission submission, Exercise exercise)
    {
        ValidateInput(submission, exercise);

        var prompt = BuildPrompt(submission, exercise);
        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";
        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GeminiResponse>();
        var aiResponse = result?.Candidates?[0]?.Content?.Parts?[0]?.Text ?? "{\"Remarks\": \"No feedback\"}";
        var feedback = new Feedback(aiResponse);

        return new Evaluation
        {
            Id = Guid.NewGuid(),
            SubmissionId = submission.Id,
            Score = feedback.Score,
            Remarks = feedback.Remarks,
            ImprovedSolution = feedback.ImprovedSolution,
            DetectedLevel = DifficultyLevel.Junior,
            EvaluatedAt = DateTime.UtcNow
        };
    }

    private void ValidateInput(Submission submission, Exercise exercise)
    {
        var placeholders = new[] { "string", "test", "" };

        if (placeholders.Any(p => exercise.Title?.Equals(p, StringComparison.OrdinalIgnoreCase) == true) ||
            placeholders.Any(p => exercise.Description?.Equals(p, StringComparison.OrdinalIgnoreCase) == true) ||
            placeholders.Any(p => submission.UserCode?.Equals(p, StringComparison.OrdinalIgnoreCase) == true) ||
            placeholders.Any(p => submission.Language?.Equals(p, StringComparison.OrdinalIgnoreCase) == true) ||
            string.IsNullOrWhiteSpace(exercise.Title) ||
            string.IsNullOrWhiteSpace(exercise.Description) ||
            string.IsNullOrWhiteSpace(submission.UserCode) ||
            string.IsNullOrWhiteSpace(submission.Language))
        {
            throw new ArgumentException("Placeholder or empty values detected. Provide actual exercise details and code.");
        }
    }

    private string BuildPrompt(Submission submission, Exercise exercise)
    {
        return $@"
            You are a senior code reviewer evaluating a coding solution.
            === EXERCISE ===
            Title: {exercise.Title}
            Description: {exercise.Description}
            Difficulty Level: {exercise.Difficulty}
            === USER'S SOLUTION ===
            Language: {submission.Language}
            Code: {submission.UserCode}
            === EVALUATION CRITERIA ===
            Assess the code based on:
            1. Correctness - Does it solve the problem?
            2. Code quality - Clean, readable, follows best practices?
            3. Efficiency - Optimal approach for the difficulty level?
            === SCORING SCALE ===
            95-100: Perfect or near-perfect solution
            85-94: Excellent solution with very minor improvements possible
            70-84: Good solution but has noticeable issues (naming, style, minor logic)
            50-69: Works but has significant problems (inefficient, poor structure, unclear code)
            30-49: Partially works with major flaws or misses key requirements
            0-29: Doesn't work, completely wrong, or fails to address the problem
            JSON String structure:
            {{
              ""score"": [number 0-100],
              ""remarks"": ""[2-3 sentences max. Be direct and helpful. Include documentation links if relevant.]"",
              ""improvedSolution"": ""[Only if score < 95. Show better code. If score >= 95, use empty string.]"",
            }}
            === IMPORTANT ===
            - DO NOT ever have ```json at the start and ``` at the end of the json string. i clearly want curly braces at the front and the end of the json string.
            - If the exercise or code contains only placeholder text (like ""string"", ""test"", empty content), 
            score it 0 and provide feedback that actual content is required. This includes all the parameters of the request body.
            - Make sure it starts and ends with ONE curly brace each.
        ";
    }
}

public class GeminiResponse
{
    public Candidate[]? Candidates { get; set; }
}

public class Candidate
{
    public Content? Content { get; set; }
}

public class Content
{
    public Part[]? Parts { get; set; }
}

public class Part
{
    public string? Text { get; set; }
}