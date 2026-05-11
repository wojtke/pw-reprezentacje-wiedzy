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

        // Project DS4 rule: concurrent simple actions are executable only when
        // they influence disjoint sets of fluents in the current state.
        var leftAffected = AffectedFluents(leftEffects, leftReleases);
        var rightAffected = AffectedFluents(rightEffects, rightReleases);
        if (leftAffected.Overlaps(rightAffected)) return true;

        // Lecture AC conflict, kept explicitly for clarity: complementary DNF terms.
        foreach (var l in leftEffects)
        foreach (var r in rightEffects)
            if (l.HasComplementWith(r)) return true;

        // Lecture AC conflict: cause against release of the same fluent.
        if (leftEffects.Any(term => term.Literals.Keys.Any(f => rightReleases.Contains(f)))) return true;
        if (rightEffects.Any(term => term.Literals.Keys.Any(f => leftReleases.Contains(f)))) return true;

        return false;
    }

    private static HashSet<string> AffectedFluents(
        IEnumerable<Ds4.Core.Formula.DnfTerm> effectTerms,
        IEnumerable<string> releases)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var term in effectTerms)
        foreach (var fluent in term.Literals.Keys)
            result.Add(fluent);
        foreach (var fluent in releases)
            result.Add(fluent);
        return result;
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
