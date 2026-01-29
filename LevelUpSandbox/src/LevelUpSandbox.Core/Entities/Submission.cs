namespace LevelUpSandbox.Core.Entities;

public class Submission
{
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public string UserCode { get; set; }
    public string Language { get; set; }
    public DateTime SubmittedAt { get; set; }
}
