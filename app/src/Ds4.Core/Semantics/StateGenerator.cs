using Ds4.Core.Model;

namespace Ds4.Core.Semantics;

public static class StateGenerator
{
    public static IReadOnlyList<State> GenerateAll(IEnumerable<string> fluents)
    {
        var names = fluents.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray();
        var count = 1 << names.Length;
        var result = new List<State>();
        for (var mask = 0; mask < count; mask++)
        {
            var trueFluents = new List<string>();
            for (var i = 0; i < names.Length; i++)
                if ((mask & (1 << i)) != 0) trueFluents.Add(names[i]);
            result.Add(new State(trueFluents));
        }
        return result;
    }
}
