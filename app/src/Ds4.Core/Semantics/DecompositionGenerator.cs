using Ds4.Core.Model;
using Ds4.Core.Query;

namespace Ds4.Core.Semantics;

public sealed class DecompositionGenerator
{
    private readonly SimpleActionEngine _simple;
    private readonly ConflictDetector _conflicts;

    public DecompositionGenerator(SimpleActionEngine simple, ConflictDetector conflicts)
    {
        _simple = simple;
        _conflicts = conflicts;
    }

    public IReadOnlyList<IReadOnlyList<string>> Generate(CompositeAction composite, State state)
    {
        var executable = composite.Actions
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(a => _simple.Res(a, state).Count > 0)
            .ToArray();

        if (executable.Length == 0) return Array.Empty<IReadOnlyList<string>>();

        var graph = _conflicts.BuildConflictEdges(new CompositeAction(executable), state);
        var independent = new List<HashSet<string>>();
        var totalMasks = 1 << executable.Length;

        for (var mask = 1; mask < totalMasks; mask++)
        {
            var subset = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < executable.Length; i++)
                if ((mask & (1 << i)) != 0) subset.Add(executable[i]);

            if (IsIndependent(subset, graph)) independent.Add(subset);
        }

        var maximal = independent
            .Where(s => !independent.Any(other => other.Count > s.Count && s.IsSubsetOf(other)))
            .Select(s => (IReadOnlyList<string>)s.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray())
            .ToArray();

        return maximal;
    }

    private static bool IsIndependent(HashSet<string> subset, HashSet<(string A, string B)> edges)
    {
        var arr = subset.ToArray();
        for (var i = 0; i < arr.Length; i++)
        for (var j = i + 1; j < arr.Length; j++)
            if (edges.Contains(ConflictDetector.NormalizeEdge(arr[i], arr[j]))) return false;
        return true;
    }
}
