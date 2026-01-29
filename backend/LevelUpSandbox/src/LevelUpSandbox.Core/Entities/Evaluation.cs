namespace LevelUpSandbox.Core.Entities;

public class Evaluation
{
    public Guid Id { get; set; }
    public Guid SubmissionId { get; set; }
    public int Score { get; set; }
    public string? Remarks { get; set; }
    public string? ImprovedSolution { get; set; }
    public DifficultyLevel DetectedLevel { get; set; }
    public DateTime EvaluatedAt { get; set; }
}
