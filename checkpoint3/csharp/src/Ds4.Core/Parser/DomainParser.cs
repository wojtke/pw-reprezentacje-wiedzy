using System.Text.RegularExpressions;
using Ds4.Core.Formula;
using Ds4.Core.Model;

namespace Ds4.Core.Parser;

public static class DomainParser
{
    public static Domain Parse(string text)
    {
        var domain = new Domain();
        var statements = SplitStatements(text);

        foreach (var statement in statements)
        {
            try { ParseStatement(domain, statement.Text); }
            catch (Exception ex) when (ex is ParseException or ArgumentException)
            {
                throw new ParseException($"Statement {statement.Number}: {ex.Message}");
            }
        }

        InferSymbols(domain);
        return domain;
    }

    private static void ParseStatement(Domain domain, string line)
    {
        if (StartsWithKeyword(line, "always"))
        {
            var formula = FormulaParser.Parse(RemoveFirstWord(line));
            domain.RegisterFormula(formula);
            domain.AlwaysConstraints.Add(new AlwaysConstraint(formula));
            return;
        }

        if (StartsWithKeyword(line, "initially"))
        {
            var formula = FormulaParser.Parse(RemoveFirstWord(line));
            domain.RegisterFormula(formula);
            domain.InitiallyConstraints.Add(new InitiallyConstraint(formula));
            return;
        }

        if (StartsWithKeyword(line, "noninertial"))
        {
            var fluent = RemoveFirstWord(line).Trim();
            if (fluent.Length == 0) throw new ParseException("noninertial statement must name a fluent.");
            domain.AddFluent(fluent);
            domain.NonInertialFluents.Add(fluent);
            return;
        }

        if (StartsWithKeyword(line, "observable"))
        {
            var rest = RemoveFirstWord(line);
            var idx = IndexOfKeyword(rest, "after");
            if (idx < 0) throw new ParseException("observable statement must contain 'after'.");
            var goal = FormulaParser.Parse(rest[..idx].Trim());
            var process = ProcessParser.Parse(rest[(idx + "after".Length)..].Trim());
            domain.RegisterFormula(goal);
            RegisterProcess(domain, process);
            domain.AfterAssertions.Add(new AfterAssertion(goal, process, Observable: true));
            return;
        }

        if (StartsWithKeyword(line, "impossible"))
        {
            var rest = RemoveFirstWord(line).Trim();
            var idx = IndexOfKeyword(rest, "if");
            string action;
            IFormula condition;
            if (idx >= 0)
            {
                action = rest[..idx].Trim();
                condition = FormulaParser.Parse(rest[(idx + "if".Length)..].Trim());
            }
            else
            {
                action = rest;
                condition = FormulaTools.True;
            }
            if (action.Length == 0) throw new ParseException("impossible statement must name an action.");
            domain.AddAction(action);
            domain.RegisterFormula(condition);
            domain.ImpossibleRules.Add(new ImpossibleRule(action, condition));
            return;
        }

        var causesIdx = IndexOfKeyword(line, "causes");
        if (causesIdx >= 0)
        {
            var action = line[..causesIdx].Trim();
            if (action.Length == 0) throw new ParseException("causes statement must name an action.");
            var rest = line[(causesIdx + "causes".Length)..].Trim();
            var ifIdx = IndexOfKeyword(rest, "if");
            var effectText = ifIdx >= 0 ? rest[..ifIdx].Trim() : rest;
            var conditionText = ifIdx >= 0 ? rest[(ifIdx + "if".Length)..].Trim() : "true";
            if (effectText.Length == 0) throw new ParseException("causes statement must contain an effect formula.");
            var effect = FormulaParser.Parse(effectText);
            var condition = FormulaParser.Parse(conditionText);
            domain.AddAction(action);
            domain.RegisterFormula(effect);
            domain.RegisterFormula(condition);
            domain.CauseRules.Add(new CauseRule(action, effect, condition));
            return;
        }

        var releasesIdx = IndexOfKeyword(line, "releases");
        if (releasesIdx >= 0)
        {
            var action = line[..releasesIdx].Trim();
            if (action.Length == 0) throw new ParseException("releases statement must name an action.");
            var rest = line[(releasesIdx + "releases".Length)..].Trim();
            var ifIdx = IndexOfKeyword(rest, "if");
            var fluent = ifIdx >= 0 ? rest[..ifIdx].Trim() : rest;
            var conditionText = ifIdx >= 0 ? rest[(ifIdx + "if".Length)..].Trim() : "true";
            if (fluent.Length == 0) throw new ParseException("releases statement must name a fluent.");
            var condition = FormulaParser.Parse(conditionText);
            domain.AddAction(action);
            domain.AddFluent(fluent);
            domain.RegisterFormula(condition);
            domain.ReleaseRules.Add(new ReleaseRule(action, fluent, condition));
            return;
        }

        var afterIdx = IndexOfKeyword(line, "after");
        if (afterIdx >= 0)
        {
            var goal = FormulaParser.Parse(line[..afterIdx].Trim());
            var process = ProcessParser.Parse(line[(afterIdx + "after".Length)..].Trim());
            domain.RegisterFormula(goal);
            RegisterProcess(domain, process);
            domain.AfterAssertions.Add(new AfterAssertion(goal, process, Observable: false));
            return;
        }

        // Optional convenience declarations. They are not required by the project language:
        // fluents and actions are normally inferred from formulas and action statements.
        // These declarations are parsed after effect statements so an action literally named
        // "action" still works in a line like: action causes q if p.
        if (StartsWithKeyword(line, "fluents"))
        {
            foreach (var f in SplitNames(RemoveFirstWord(line))) domain.AddFluent(f);
            return;
        }
        if (StartsWithKeyword(line, "actions"))
        {
            foreach (var a in SplitNames(RemoveFirstWord(line))) domain.AddAction(a);
            return;
        }

        throw new ParseException("Unknown statement: " + line);
    }

    private static IReadOnlyList<StatementText> SplitStatements(string text)
    {
        var result = new List<StatementText>();
        var lines = text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');

        for (var lineNo = 0; lineNo < lines.Length; lineNo++)
        {
            var line = StripComment(lines[lineNo]).Trim();
            if (line.Length == 0) continue;

            foreach (var part in SplitLineStatements(line))
            {
                var statement = TrimTerminator(part.Trim());
                if (statement.Length > 0) result.Add(new StatementText(lineNo + 1, statement));
            }
        }
        return result;
    }

    private static IEnumerable<string> SplitLineStatements(string line)
    {
        // In domain descriptions from the lecture, semicolons usually terminate statements.
        // In our project, after/observable statements may also contain semicolons inside a process.
        // For such lines we keep the whole line intact.
        if (IndexOfKeyword(line, "after") >= 0) return new[] { line };
        return ProcessParser.SplitTopLevel(line, ';');
    }

    private static string TrimTerminator(string line)
    {
        while (line.EndsWith('.') || line.EndsWith(';')) line = line[..^1].Trim();
        return line;
    }

    private static void InferSymbols(Domain domain)
    {
        foreach (var rule in domain.CauseRules) domain.AddAction(rule.Action);
        foreach (var rule in domain.ReleaseRules) { domain.AddAction(rule.Action); domain.AddFluent(rule.Fluent); }
        foreach (var rule in domain.ImpossibleRules) domain.AddAction(rule.Action);
        foreach (var f in domain.NonInertialFluents) domain.AddFluent(f);
    }

    private static void RegisterProcess(Domain domain, Ds4.Core.Query.Ds4Process process)
    {
        foreach (var step in process.Steps)
        foreach (var action in step.Actions)
            domain.AddAction(action);
    }

    private static IEnumerable<string> SplitNames(string text)
        => text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static string StripComment(string line)
    {
        var hash = line.IndexOf('#');
        var slash = line.IndexOf("//", StringComparison.Ordinal);
        var cut = new[] { hash, slash }.Where(i => i >= 0).DefaultIfEmpty(line.Length).Min();
        return line[..cut];
    }

    private static bool StartsWithKeyword(string line, string keyword)
        => Regex.IsMatch(line, "^" + Regex.Escape(keyword) + "(\\s+|$)", RegexOptions.IgnoreCase);

    private static string RemoveFirstWord(string line)
    {
        var idx = line.IndexOf(' ');
        return idx < 0 ? "" : line[(idx + 1)..].Trim();
    }

    private static int IndexOfKeyword(string text, string keyword)
    {
        var match = Regex.Match(text, "(^|\\s)" + Regex.Escape(keyword) + "(\\s|$)", RegexOptions.IgnoreCase);
        if (!match.Success) return -1;
        return match.Index + (match.Value.StartsWith(' ') ? 1 : 0);
    }

    private sealed record StatementText(int Number, string Text);
}
