using Ds4.Core.Query;

namespace Ds4.Core.Parser;

public static class ProcessParser
{
    public static Ds4Process Parse(string text)
    {
        text = text.Trim();
        if (text.Length == 0 || text == "ε" || text.Equals("epsilon", StringComparison.OrdinalIgnoreCase))
            return Ds4Process.Empty;

        var parts = SplitTopLevel(text, ';');
        if (parts.Count == 1 && ContainsTopLevel(text, ','))
            parts = SplitTopLevel(text, ',');

        var steps = new List<CompositeAction>();
        foreach (var raw in parts)
        {
            var part = raw.Trim();
            if (part.Length == 0) continue;
            if (part.StartsWith('{') && part.EndsWith('}'))
            {
                var inner = part[1..^1];
                var actions = SplitTopLevel(inner, ',').Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();
                if (actions.Length == 0) throw new ParseException("Empty composite action is not allowed.");
                steps.Add(new CompositeAction(actions));
            }
            else
            {
                steps.Add(new CompositeAction(new[] { part }));
            }
        }
        return new Ds4Process(steps);
    }

    public static List<string> SplitTopLevel(string text, char delimiter)
    {
        var result = new List<string>();
        var depth = 0;
        var start = 0;
        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == '{' || text[i] == '(') depth++;
            else if (text[i] == '}' || text[i] == ')') depth--;
            else if (text[i] == delimiter && depth == 0)
            {
                result.Add(text[start..i]);
                start = i + 1;
            }
        }
        result.Add(text[start..]);
        return result;
    }

    private static bool ContainsTopLevel(string text, char delimiter)
    {
        var depth = 0;
        foreach (var c in text)
        {
            if (c == '{' || c == '(') depth++;
            else if (c == '}' || c == ')') depth--;
            else if (c == delimiter && depth == 0) return true;
        }
        return false;
    }
}
