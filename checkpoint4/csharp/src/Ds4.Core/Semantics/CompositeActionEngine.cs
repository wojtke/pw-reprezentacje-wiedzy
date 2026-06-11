using Ds4.Core.Model;
using Ds4.Core.Query;

namespace Ds4.Core.Semantics;

public sealed class CompositeActionEngine
{
    private readonly SimpleActionEngine _simple;
    private readonly DecompositionGenerator _decompositions;

    public CompositeActionEngine(SimpleActionEngine simple, DecompositionGenerator decompositions)
    {
        _simple = simple;
        _decompositions = decompositions;
    }

    public IReadOnlyList<State> Res(CompositeAction composite, State state)
    {
        var decompositions = _decompositions.Generate(composite, state);
        if (decompositions.Count == 0) return Array.Empty<State>();

        var result = new HashSet<State>();
        foreach (var delta in decompositions)
        {
            var effects = delta.SelectMany(a => _simple.ActiveEffects(a, state)).ToList();
            var releases = delta.SelectMany(a => _simple.ActiveReleases(a, state)).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var s in _simple.ResFromCombinedRules(effects, releases, state))
                result.Add(s);
        }
        return result.ToArray();
    }
}
