namespace Ds4.Core.Api;

public sealed class SolveResult
{
    public bool Ok { get; init; }
    public string? Error { get; init; }
    public bool? Answer { get; init; }
    public string Explanation { get; init; } = "";
    public int SigmaCount { get; init; }
    public int Sigma0Count { get; init; }
    public string Trace { get; init; } = "";
    public string TraceGraphDot { get; init; } = "";
}

public sealed class ValidationResult
{
    public bool Ok { get; init; }
    public string? Error { get; init; }
    public int SigmaCount { get; init; }
    public int Sigma0Count { get; init; }
}

public sealed record ExampleSummary(string Id, string Name, string Description);
public sealed record Example(string Id, string Name, string Description, string Domain, IReadOnlyList<string> Queries);

public sealed record CacheStats(
    int FormulaCount,
    int DomainModelCount,
    int ParsedQueryCount,
    int SolveResultCount,
    int ExampleCount);
