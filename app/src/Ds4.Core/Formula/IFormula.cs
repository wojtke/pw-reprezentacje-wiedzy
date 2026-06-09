using Ds4.Core.Model;

namespace Ds4.Core.Formula;

public interface IFormula
{
    bool Evaluate(State state);
    IEnumerable<string> Atoms();
    IReadOnlyList<DnfTerm> ToDnf();
}

public sealed record Literal(string Fluent, bool IsPositive)
{
    public Literal Negated() => new(Fluent, !IsPositive);
    public override string ToString() => IsPositive ? Fluent : "¬" + Fluent;
}

public sealed class DnfTerm
{
    private readonly Dictionary<string, bool> _literals = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, bool> Literals => _literals;

    public static DnfTerm Empty() => new();

    public DnfTerm Add(Literal literal)
    {
        var copy = new DnfTerm();
        foreach (var kv in _literals) copy._literals[kv.Key] = kv.Value;
        copy._literals[literal.Fluent] = literal.IsPositive;
        return copy;
    }

    public bool ContainsFluent(string fluent) => _literals.ContainsKey(fluent);

    public bool HasComplementWith(DnfTerm other)
    {
        foreach (var (fluent, value) in _literals)
        {
            if (other._literals.TryGetValue(fluent, out var otherValue) && otherValue != value)
                return true;
        }
        return false;
    }

    public override string ToString()
        => _literals.Count == 0 ? "⊤" : string.Join(" ∧ ", _literals.OrderBy(kv => kv.Key).Select(kv => kv.Value ? kv.Key : "¬" + kv.Key));
}
