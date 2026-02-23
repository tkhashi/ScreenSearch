using ScreenSearch.Core.Models;

namespace ScreenSearch.Core.UseCases;

public sealed class FinalizeSessionUseCase
{
    public OperationSession Finalize(OperationSession session, string result, FailureCategory failureCategory)
    {
        return session with
        {
            EndedAt = DateTimeOffset.UtcNow,
            Result = result,
            FailureCategory = failureCategory
        };
    }
}
