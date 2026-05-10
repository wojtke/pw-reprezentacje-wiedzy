using Ds4.Core.Model;

namespace Ds4.Core.Semantics;

public sealed class SimpleActionEngine
{
    private readonly Domain _domain;
    private readonly IReadOnlyList<State> _sigma;

    public SimpleActionEngine(Domain domain, IReadOnlyList<State> sigma)
    {
        _domain = domain;
        _sigma = sigma;
    }

    public IReadOnlyList<State> Res(string action, State state)
    {
        if (IsImpossible(action, state)) return Array.Empty<State>();
        var activeEffects = ActiveEffects(action, state);
        var activeReleases = ActiveReleases(action, state);
        return ResFromCombinedRules(activeEffects, activeReleases, state);
    }

    public List<Ds4.Core.Formula.IFormula> ActiveEffects(string action, State state)
        => _domain.CauseRules
            .Where(r => Same(r.Action, action) && r.Condition.Evaluate(state))
            .Select(r => r.Effect)
            .ToList();

    public HashSet<string> ActiveReleases(string action, State state)
        => _domain.ReleaseRules
            .Where(r => Same(r.Action, action) && r.Condition.Evaluate(state))
            .Select(r => r.Fluent)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

    public bool IsImpossible(string action, State state)
        => _domain.ImpossibleRules.Any(r => Same(r.Action, action) && r.Condition.Evaluate(state));

    public IReadOnlyList<State> ResFromCombinedRules(
        IEnumerable<Ds4.Core.Formula.IFormula> effects,
        ISet<string> releasedFluents,
        State state)
    {
        var effectList = effects.ToList();
        var res0 = _sigma.Where(candidate => effectList.All(e => e.Evaluate(candidate))).ToList();
        var newSets = res0.ToDictionary(s => s, s => NewSet(state, s, releasedFluents));

        var minimal = new List<State>();
        foreach (var candidate in res0)
        {
            var candidateSet = newSets[candidate];
            var dominated = res0.Any(other =>
                !ReferenceEquals(other, candidate) &&
                IsProperSubset(newSets[other], candidateSet));
            if (!dominated) minimal.Add(candidate);
        }
        return minimal;
    }

    private HashSet<LiteralChange> NewSet(State from, State to, ISet<string> releasedFluents)
    {
        var result = new HashSet<LiteralChange>();
        foreach (var fluent in _domain.InertialFluents)
        {
            if (from.IsTrue(fluent) != to.IsTrue(fluent))
                result.Add(new LiteralChange(fluent, to.IsTrue(fluent)));
        }
        foreach (var fluent in releasedFluents)
            result.Add(new LiteralChange(fluent, to.IsTrue(fluent)));
        return result;
    }

    private static bool IsProperSubset(HashSet<LiteralChange> left, HashSet<LiteralChange> right)
        => left.Count < right.Count && left.IsSubsetOf(right);

    private static bool Same(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
}
