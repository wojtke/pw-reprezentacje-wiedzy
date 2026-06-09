using Ds4.Core.Formula;

namespace Ds4.Core.Query;

public sealed record CompositeAction(IReadOnlyList<string> Actions)
{
    public override string ToString() => Actions.Count == 1 ? Actions[0] : "{" + string.Join(",", Actions) + "}";
}

public sealed record Ds4Process(IReadOnlyList<CompositeAction> Steps)
{
    public static Ds4Process Empty { get; } = new(Array.Empty<CompositeAction>());
    public override string ToString() => Steps.Count == 0 ? "ε" : string.Join("; ", Steps);
}

public enum Quantifier
{
    Possibly,
    Necessary
}

public enum QueryKind
{
    Executable,
    AfterGoal
}

public sealed record Ds4Query(Quantifier Quantifier, QueryKind Kind, Ds4Process Process, IFormula? Goal)
{
    public override string ToString()
    {
        var q = Quantifier == Quantifier.Possibly ? "possibly" : "necessary";
        return Kind == QueryKind.Executable
            ? $"{q} executable after {Process}"
            : $"{q} {Goal} after {Process}";
    }
}
