using Ds4.Core.Model;
using Ds4.Core.Query;

namespace Ds4.Core.Semantics;

public sealed class ConflictDetector
{
    private readonly SimpleActionEngine _simple;

    public ConflictDetector(SimpleActionEngine simple)
    {
        _simple = simple;
    }

    public HashSet<(string A, string B)> BuildConflictEdges(CompositeAction composite, State state)
    {
        var actions = composite.Actions.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var edges = new HashSet<(string A, string B)>(new EdgeComparer());

        for (var i = 0; i < actions.Length; i++)
        for (var j = i + 1; j < actions.Length; j++)
        {
            if (AreInConflict(actions[i], actions[j], state))
                edges.Add(NormalizeEdge(actions[i], actions[j]));
        }
        return edges;
    }

    public bool AreInConflict(string leftAction, string rightAction, State state)
    {
        var leftEffects = _simple.ActiveEffects(leftAction, state).SelectMany(e => e.ToDnf()).ToList();
        var rightEffects = _simple.ActiveEffects(rightAction, state).SelectMany(e => e.ToDnf()).ToList();
        var leftReleases = _simple.ActiveReleases(leftAction, state);
        var rightReleases = _simple.ActiveReleases(rightAction, state);

        // Conflict is defined (theory v2, sec. Konflikty) by exactly two cases.
        // Sharing a fluent alone is NOT a conflict (theorem: disjoint Vars => no
        // conflict, but the converse does not hold).

        // Case 1 (causes vs causes): complementary literals in some DNF terms.
        foreach (var l in leftEffects)
        foreach (var r in rightEffects)
            if (l.HasComplementWith(r)) return true;

        // Case 2 (causes vs releases): one action's effect mentions a fluent the
        // other releases.
        if (leftEffects.Any(term => term.Literals.Keys.Any(f => rightReleases.Contains(f)))) return true;
        if (rightEffects.Any(term => term.Literals.Keys.Any(f => leftReleases.Contains(f)))) return true;

        return false;
    }

    public static (string A, string B) NormalizeEdge(string a, string b)
        => string.Compare(a, b, StringComparison.OrdinalIgnoreCase) <= 0 ? (a, b) : (b, a);

    private sealed class EdgeComparer : IEqualityComparer<(string A, string B)>
    {
        public bool Equals((string A, string B) x, (string A, string B) y)
            => string.Equals(x.A, y.A, StringComparison.OrdinalIgnoreCase)
            && string.Equals(x.B, y.B, StringComparison.OrdinalIgnoreCase);

        public int GetHashCode((string A, string B) obj)
            => HashCode.Combine(StringComparer.OrdinalIgnoreCase.GetHashCode(obj.A), StringComparer.OrdinalIgnoreCase.GetHashCode(obj.B));
    }
}
