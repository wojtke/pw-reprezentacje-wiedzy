namespace Ds4.Core.Model;

public sealed class State : IEquatable<State>
{
    private readonly SortedSet<string> _trueFluents;
    public IReadOnlyCollection<string> TrueFluents => _trueFluents;

    public State(IEnumerable<string> trueFluents)
    {
        _trueFluents = new SortedSet<string>(trueFluents.Select(Normalize), StringComparer.OrdinalIgnoreCase);
    }

    public bool IsTrue(string fluentName) => _trueFluents.Contains(Normalize(fluentName));

    public bool Equals(State? other)
        => other is not null && _trueFluents.SetEquals(other._trueFluents);

    public override bool Equals(object? obj) => Equals(obj as State);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var f in _trueFluents) hash.Add(f, StringComparer.OrdinalIgnoreCase);
        return hash.ToHashCode();
    }

    public string ToPrettyString(IEnumerable<string> allFluents)
    {
        var parts = allFluents.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .Select(f => IsTrue(f) ? f : "not " + f);
        return "{" + string.Join(", ", parts) + "}";
    }

    public override string ToString() => "{" + string.Join(", ", _trueFluents) + "}";

    private static string Normalize(string name) => name.Trim();
}
