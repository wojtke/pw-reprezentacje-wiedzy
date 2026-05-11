using System.Text;
using Ds4.Core.Model;
using Ds4.Core.Parser;
using Ds4.Core.Query;
using Ds4.Core.Semantics;

namespace Ds4.Core.Api;

public static class Ds4Facade
{
    public static SolveResult Solve(string domainText, string queryText)
    {
        try
        {
            var domain = DomainParser.Parse(domainText);
            var query = QueryParser.Parse(PrepareQueryText(queryText));
            RegisterQuerySymbols(domain, query);
            var model = ModelBuilder.Build(domain);

            if (model.Sigma.Count == 0)
                return new SolveResult { Ok = false, Error = "Model jest pusty: ograniczenia always są sprzeczne.", SigmaCount = 0, Sigma0Count = 0 };
            if (model.Sigma0.Count == 0)
                return new SolveResult { Ok = false, Error = "Brak stanów początkowych: initially/after/observable after są sprzeczne.", SigmaCount = model.Sigma.Count, Sigma0Count = 0 };

            var evaluator = new QueryEvaluator(model);
            var result = evaluator.Evaluate(query);
            return new SolveResult
            {
                Ok = true,
                Answer = result.Answer,
                Explanation = result.Explanation,
                SigmaCount = model.Sigma.Count,
                Sigma0Count = model.Sigma0.Count,
                Trace = TraceRenderer.Render(result.Tree, model.Domain, maxChars: 60000)
            };
        }
        catch (Exception ex) when (ex is ParseException or ArgumentException or InvalidOperationException)
        {
            return new SolveResult { Ok = false, Error = ex.Message };
        }
    }

    public static ValidationResult ValidateDomain(string domainText)
    {
        try
        {
            var domain = DomainParser.Parse(domainText);
            var model = ModelBuilder.Build(domain);
            if (model.Sigma.Count == 0)
                return new ValidationResult { Ok = false, Error = "Model jest pusty: sprzeczne always.", SigmaCount = 0, Sigma0Count = 0 };
            if (model.Sigma0.Count == 0)
                return new ValidationResult { Ok = false, Error = "Brak stanów początkowych.", SigmaCount = model.Sigma.Count, Sigma0Count = 0 };
            return new ValidationResult { Ok = true, SigmaCount = model.Sigma.Count, Sigma0Count = model.Sigma0.Count };
        }
        catch (Exception ex) when (ex is ParseException or ArgumentException or InvalidOperationException)
        {
            return new ValidationResult { Ok = false, Error = ex.Message };
        }
    }

    public static IReadOnlyList<ExampleSummary> ListExamples()
    {
        return LoadExamplesFromFolder()
            .Select(e => new ExampleSummary(e.Id, e.Name, e.Description))
            .ToArray();
    }

    public static Example LoadExample(string id)
    {
        var fromFolder = LoadExamplesFromFolder()
            .FirstOrDefault(e => string.Equals(e.Id, id, StringComparison.OrdinalIgnoreCase));
        if (fromFolder is not null) return fromFolder;
        throw new ArgumentException("Unknown example id: " + id);
    }

    private static IReadOnlyList<Example> LoadExamplesFromFolder()
    {
        var dir = FindExamplesDirectory();
        if (dir is null || !Directory.Exists(dir)) return Array.Empty<Example>();

        var examples = new List<Example>();
        foreach (var domainPath in Directory.GetFiles(dir, "*.domain").OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
        {
            var id = Path.GetFileNameWithoutExtension(domainPath);
            var queryPath = Path.Combine(dir, id + ".query");
            if (!File.Exists(queryPath)) continue;

            var domainText = NormalizeLineEndings(File.ReadAllText(domainPath));
            var queryLines = File.ReadAllLines(queryPath)
                .Select(line => line.Trim())
                .Where(line => line.Length > 0 && !line.StartsWith("#") && !line.StartsWith("//"))
                .ToArray();
            if (queryLines.Length == 0) continue;

            var metadata = ReadMetadata(domainText);
            var name = metadata.TryGetValue("name", out var metaName) ? metaName : MakePrettyName(id);
            var description = metadata.TryGetValue("description", out var metaDescription) ? metaDescription : "Przykład z folderu examples.";

            examples.Add(new Example(id, name, description, domainText, queryLines));
        }
        return examples;
    }

    private static string PrepareQueryText(string queryText)
    {
        var lines = queryText.Replace("\r\n", "\n").Replace("\r", "\n")
            .Split('\n')
            .Select(StripQueryComment)
            .Select(line => line.Trim())
            .Where(line => line.Length > 0)
            .ToArray();
        return string.Join(" ", lines);
    }

    private static string StripQueryComment(string line)
    {
        var hash = line.IndexOf('#');
        var slash = line.IndexOf("//", StringComparison.Ordinal);
        var cut = new[] { hash, slash }.Where(i => i >= 0).DefaultIfEmpty(line.Length).Min();
        return line[..cut];
    }

    private static string? FindExamplesDirectory()
    {
        var candidates = new List<string>
        {
            Path.Combine(Environment.CurrentDirectory, "examples"),
            Path.Combine(AppContext.BaseDirectory, "examples")
        };

        AddUpwardCandidates(candidates, Environment.CurrentDirectory);
        AddUpwardCandidates(candidates, AppContext.BaseDirectory);

        return candidates.FirstOrDefault(path =>
            Directory.Exists(path) &&
            Directory.GetFiles(path, "*.domain").Any(f => File.Exists(Path.ChangeExtension(f, ".query"))));
    }

    private static void AddUpwardCandidates(List<string> candidates, string start)
    {
        var dir = new DirectoryInfo(start);
        for (var i = 0; i < 8 && dir is not null; i++, dir = dir.Parent)
            candidates.Add(Path.Combine(dir.FullName, "examples"));
    }

    private static Dictionary<string, string> ReadMetadata(string domainText)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var raw in domainText.Replace("\r\n", "\n").Split('\n'))
        {
            var line = raw.Trim();
            if (!line.StartsWith("#")) continue;
            line = line[1..].Trim();
            var colon = line.IndexOf(':');
            if (colon <= 0) continue;
            var key = line[..colon].Trim();
            var value = line[(colon + 1)..].Trim();
            if (key.Length > 0 && value.Length > 0) result[key] = value;
        }
        return result;
    }

    private static string MakePrettyName(string id)
    {
        var text = id.Replace('_', ' ').Replace('-', ' ');
        return string.Join(" ", text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(word =>
            word.Length == 0 ? word : char.ToUpperInvariant(word[0]) + word[1..]));
    }

    private static string NormalizeLineEndings(string text)
        => text.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);

    private static void RegisterQuerySymbols(Domain domain, Ds4Query query)
    {
        foreach (var step in query.Process.Steps)
        foreach (var action in step.Actions)
            domain.AddAction(action);
        if (query.Goal is not null) domain.RegisterFormula(query.Goal);
    }
}

public static class TraceRenderer
{
    public static string Render(ExecutionTree tree, Domain domain, int maxChars = 60000)
    {
        var sb = new StringBuilder();
        var truncated = false;
        foreach (var root in tree.Roots)
        {
            if (sb.Length >= maxChars)
            {
                truncated = true;
                break;
            }
            RenderNode(sb, root, domain, indent: 0, tree.ProcessLength, maxChars, ref truncated);
        }

        if (truncated)
        {
            sb.AppendLine();
            sb.AppendLine($"[trace ucięty po około {maxChars} znakach, żeby GUI nie zjadało pamięci]");
        }

        return sb.ToString();
    }

    private static void RenderNode(StringBuilder sb, ExecutionNode node, Domain domain, int indent, int processLength, int maxChars, ref bool truncated)
    {
        if (sb.Length >= maxChars)
        {
            truncated = true;
            return;
        }
        var prefix = new string(' ', indent * 2);
        var label = node.Depth == 0 ? "start" : node.IncomingStep?.ToString() ?? "?";
        sb.Append(prefix).Append("[").Append(node.Depth).Append("] ").Append(label).Append(" -> ")
            .Append(node.State.ToPrettyString(domain.Fluents)).AppendLine();

        if (node.Blocked)
            sb.Append(prefix).Append("  BLOCKED: brak następników dla kolejnego kroku procesu").AppendLine();

        foreach (var child in node.Children)
            RenderNode(sb, child, domain, indent + 1, processLength, maxChars, ref truncated);
    }
}
