using System.Text.Json;

namespace LevelUpSandbox.Core.Entities;

public class Feedback
{
    public int Score { get; set; }
    public string? Remarks { get; set; }
    public string? ImprovedSolution { get; set; }

    public Feedback(string rawJson)
    {
        try
        {
            var cleanJson = rawJson.Trim();

            using var doc = JsonDocument.Parse(cleanJson);
            var root = doc.RootElement;

            Score = root.TryGetProperty("score", out var scoreEl) ? scoreEl.GetInt32() : 0;
            Remarks = root.TryGetProperty("remarks", out var remarksEl) ? remarksEl.GetString()?.Trim() ?? string.Empty : string.Empty;
            ImprovedSolution = root.TryGetProperty("improvedSolution", out var solEl) ? solEl.GetString()?.Trim() ?? string.Empty : string.Empty;
        }
        catch (JsonException ex)
        {
            Remarks = $"Failed to parse AI response: {ex.Message}";
            Score = 0;
            ImprovedSolution = string.Empty;
        }
    }

    public Feedback() { }
}