using Ds4.Core.Api;
using Ds4.Core.Model;
using Ds4.Core.Parser;
using Ds4.Core.Semantics;

namespace Ds4.Tests;

internal static class TestData
{
    public static TransitionModel BuildModel(string domainText)
        => ModelBuilder.Build(DomainParser.Parse(domainText));

    public static SolveResult Solve(string domainText, string queryText)
        => Ds4Facade.Solve(domainText, queryText);

    public static string RepoRoot()
    {
        var dir = new DirectoryInfo(Environment.CurrentDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "Ds4Reasoner.sln")))
                return dir.FullName;
            dir = dir.Parent;
        }
        throw new InvalidOperationException("Cannot find repository root from " + Environment.CurrentDirectory);
    }

    public static string ExamplesDir() => Path.Combine(RepoRoot(), "examples");

    public static IEnumerable<object[]> ExampleFiles()
    {
        foreach (var domainPath in Directory.GetFiles(ExamplesDir(), "*.domain").OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
        {
            var id = Path.GetFileNameWithoutExtension(domainPath);
            var queryPath = Path.Combine(ExamplesDir(), id + ".query");
            var expectedPath = Path.Combine(ExamplesDir(), id + ".expected");
            if (!File.Exists(queryPath) || !File.Exists(expectedPath)) continue;
            yield return new object[] { id, domainPath, queryPath, expectedPath };
        }
    }

    public static bool ExpectedAnswer(string expectedPath)
    {
        var text = File.ReadAllText(expectedPath).Trim();
        if (text.Equals("TAK", StringComparison.OrdinalIgnoreCase)) return true;
        if (text.Equals("NIE", StringComparison.OrdinalIgnoreCase)) return false;
        throw new InvalidOperationException("Unknown expected value in " + expectedPath + ": " + text);
    }

    public static State State(params string[] trueFluents) => new(trueFluents);
}
