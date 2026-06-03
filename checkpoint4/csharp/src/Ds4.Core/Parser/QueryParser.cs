using Ds4.Core.Query;

namespace Ds4.Core.Parser;

public static class QueryParser
{
    public static Ds4Query Parse(string text)
    {
        text = text.Trim();
        if (text.EndsWith('.')) text = text[..^1].Trim();

        if (Starts(text, "possibly executable"))
        {
            var process = ParseAfterPart(text["possibly executable".Length..]);
            return new Ds4Query(Quantifier.Possibly, QueryKind.Executable, process, null);
        }
        if (Starts(text, "necessary executable"))
        {
            var process = ParseAfterPart(text["necessary executable".Length..]);
            return new Ds4Query(Quantifier.Necessary, QueryKind.Executable, process, null);
        }
        if (Starts(text, "possibly"))
        {
            return ParseGoal(text["possibly".Length..], Quantifier.Possibly);
        }
        if (Starts(text, "necessary"))
        {
            return ParseGoal(text["necessary".Length..], Quantifier.Necessary);
        }

        throw new ParseException("Query must start with possibly/necessary.");
    }

    private static Ds4Query ParseGoal(string rest, Quantifier quantifier)
    {
        rest = rest.Trim();
        var idx = IndexOfKeyword(rest, "after");
        if (idx < 0) throw new ParseException("Goal query must contain 'after'.");
        var goalText = rest[..idx].Trim();
        var processText = rest[(idx + "after".Length)..].Trim();
        var goal = FormulaParser.Parse(goalText);
        var process = ProcessParser.Parse(processText);
        return new Ds4Query(quantifier, QueryKind.AfterGoal, process, goal);
    }

    private static Ds4Process ParseAfterPart(string rest)
    {
        rest = rest.Trim();
        if (rest.Length == 0) return Ds4Process.Empty;
        if (!Starts(rest, "after")) throw new ParseException("Executable query must use 'after <process>'.");
        return ProcessParser.Parse(rest["after".Length..].Trim());
    }

    private static bool Starts(string text, string prefix)
        => text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);

    private static int IndexOfKeyword(string text, string keyword)
    {
        var lower = text.ToLowerInvariant();
        var needle = " " + keyword.ToLowerInvariant() + " ";
        var idx = lower.IndexOf(needle, StringComparison.Ordinal);
        if (idx >= 0) return idx + 1;
        if (lower.EndsWith(" " + keyword.ToLowerInvariant(), StringComparison.Ordinal)) return lower.Length - keyword.Length;
        return -1;
    }
}
