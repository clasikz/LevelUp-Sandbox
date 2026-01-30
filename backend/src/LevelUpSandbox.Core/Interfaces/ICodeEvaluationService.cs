using LevelUpSandbox.Core.Entities;
namespace LevelUpSandbox.Core.Interfaces;

public interface ICodeEvaluationService
{
    Task<Evaluation> EvaluateCodeAsync(Submission submission, Exercise exercise);
}