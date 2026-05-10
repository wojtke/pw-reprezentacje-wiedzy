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
            var query = QueryParser.Parse(queryText);
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
        var fromFolder = LoadExamplesFromFolder().Select(e => new ExampleSummary(e.Id, e.Name, e.Description)).ToArray();
        if (fromFolder.Length > 0) return fromFolder;
        return BuiltInExamples().Select(e => new ExampleSummary(e.Id, e.Name, e.Description)).ToArray();
    }

    public static Example LoadExample(string id)
    {
        var fromFolder = LoadExamplesFromFolder()
            .FirstOrDefault(e => string.Equals(e.Id, id, StringComparison.OrdinalIgnoreCase));
        if (fromFolder is not null) return fromFolder;

        var builtIn = BuiltInExamples()
            .FirstOrDefault(e => string.Equals(e.Id, id, StringComparison.OrdinalIgnoreCase));
        if (builtIn is not null) return builtIn;

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

    private static IReadOnlyList<Example> BuiltInExamples() => new[]
    {
        new Example(
            "tak_01_switches_necessary_alarm",
            "TAK 01 - Dwa przełączniki",
            "Ramifikacja always: wykonanie obu przełączników koniecznie włącza alarm.",
            """
            fluents s1, s2, light, alarm
            actions toggle1, toggle2
            always (s1 or s2) -> light
            always light -> alarm
            noninertial light
            noninertial alarm
            initially !s1 and !s2
            toggle1 causes s1 if !s1
            toggle1 causes !s1 if s1
            toggle2 causes s2 if !s2
            toggle2 causes !s2 if s2
            """,
            new[] { "necessary alarm after {toggle1,toggle2}" }),
        new Example(
            "tak_02_roulette_possibly_death",
            "TAK 02 - Ruletka: możliwa śmierć",
            "Po spin możliwy jest stan loaded, więc shoot może spowodować !alive.",
            """
            fluents loaded, alive
            actions spin, shoot, load
            initially alive
            spin releases loaded if true
            shoot causes !alive if loaded
            impossible load if loaded
            load causes loaded if !loaded
            """,
            new[] { "possibly !alive after spin; shoot" }),
        new Example(
            "tak_03_unlock_necessary_open",
            "TAK 03 - Otwarcie drzwi",
            "Klucz jest dostępny, więc unlock koniecznie prowadzi do door_open.",
            """
            fluents key, door_open
            actions unlock
            initially key and !door_open
            unlock causes door_open if key
            """,
            new[] { "necessary door_open after unlock" }),
        new Example(
            "nie_01_roulette_necessary_death",
            "NIE 01 - Ruletka: śmierć nie jest konieczna",
            "Po spin możliwe jest loaded=false, więc shoot nie musi zabić.",
            """
            fluents loaded, alive
            actions spin, shoot, load
            initially alive
            spin releases loaded if true
            shoot causes !alive if loaded
            impossible load if loaded
            load causes loaded if !loaded
            """,
            new[] { "necessary !alive after spin; shoot" }),
        new Example(
            "nie_02_impossible_load_executable",
            "NIE 02 - Load niewykonalne",
            "Stan początkowy ma loaded=true, a wtedy load jest impossible.",
            """
            fluents loaded
            actions load
            initially loaded
            impossible load if loaded
            load causes loaded if !loaded
            """,
            new[] { "possibly executable after load" }),
        new Example(
            "nie_03_conflict_necessary_p",
            "NIE 03 - Konflikt akcji",
            "Dekompozycje konfliktu prowadzą do alternatywnych stanów, więc p nie jest konieczne.",
            """
            fluents p
            actions make_p, make_not_p
            initially !p
            make_p causes p if true
            make_not_p causes !p if true
            """,
            new[] { "necessary p after {make_p,make_not_p}" })
    };

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
