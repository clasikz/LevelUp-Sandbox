namespace LevelUpSandbox.Core.Entities;

public class Exercise
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public string[] TestCases { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum DifficultyLevel
{
    Junior = 1,
    Mid = 2,
    Senior = 3
}
