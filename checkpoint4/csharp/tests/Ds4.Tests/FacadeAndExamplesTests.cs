using Ds4.Core.Api;

namespace Ds4.Tests;

public sealed class FacadeAndExamplesTests
{
    [Fact]
    public void ListExamples_Loads_Examples_From_Folder()
    {
        var examples = Ds4Facade.ListExamples();

        Assert.NotEmpty(examples);
        Assert.Contains(examples, e => e.Id.StartsWith("tak_", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(examples, e => e.Id.StartsWith("nie_", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void LoadExample_Returns_Domain_And_Query_Text()
    {
        var first = Ds4Facade.ListExamples().First();
        var example = Ds4Facade.LoadExample(first.Id);

        Assert.Equal(first.Id, example.Id);
        Assert.NotEmpty(example.Domain);
        Assert.NotEmpty(example.Queries);
    }

    [Theory]
    [MemberData(nameof(AllExamples))]
    public void Every_Example_Returns_Its_Expected_Answer(string id, string domainPath, string queryPath, string expectedPath)
    {
        var domain = File.ReadAllText(domainPath);
        var query = File.ReadAllText(queryPath);
        var expected = TestData.ExpectedAnswer(expectedPath);

        var result = Ds4Facade.Solve(domain, query);

        Assert.True(result.Ok, id + " failed with error: " + result.Error);
        Assert.Equal(expected, result.Answer);
    }

    [Fact]
    public void Examples_Contain_Tex_Cases_Extra_Tak_Cases_And_Extra_Nie_Cases()
    {
        var dir = TestData.ExamplesDir();
        var ids = Directory.GetFiles(dir, "*.domain")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(x => x is not null)
            .Select(x => x!)
            .ToArray();

        Assert.True(ids.Count(id => id.StartsWith("tak_tex_", StringComparison.OrdinalIgnoreCase)) >= 10);
        Assert.True(ids.Count(id => id.StartsWith("nie_tex_", StringComparison.OrdinalIgnoreCase)) >= 5);
        Assert.Equal(10, ids.Count(id => id.StartsWith("tak_extra_", StringComparison.OrdinalIgnoreCase)));
        Assert.Equal(10, ids.Count(id => id.StartsWith("nie_extra_", StringComparison.OrdinalIgnoreCase)));
        Assert.True(ids.Count(id => id.StartsWith("scenario_", StringComparison.OrdinalIgnoreCase)) >= 16);
    }

    [Fact]
    public void Every_Domain_Has_Query_And_Expected_File()
    {
        var dir = TestData.ExamplesDir();
        foreach (var domainPath in Directory.GetFiles(dir, "*.domain"))
        {
            var id = Path.GetFileNameWithoutExtension(domainPath);
            Assert.True(File.Exists(Path.Combine(dir, id + ".query")), id + " has no .query file");
            Assert.True(File.Exists(Path.Combine(dir, id + ".expected")), id + " has no .expected file");
        }
    }

    [Fact]
    public void Expected_Files_Use_Only_Tak_Or_Nie()
    {
        foreach (var expectedPath in Directory.GetFiles(TestData.ExamplesDir(), "*.expected"))
        {
            var text = File.ReadAllText(expectedPath).Trim();
            Assert.True(
                text.Equals("TAK", StringComparison.OrdinalIgnoreCase) || text.Equals("NIE", StringComparison.OrdinalIgnoreCase),
                Path.GetFileName(expectedPath) + " contains invalid expected answer: " + text);
        }
    }

    [Fact]
    public void LoadExample_Throws_For_Unknown_Id()
    {
        Assert.Throws<ArgumentException>(() => Ds4Facade.LoadExample("definitely_missing_example"));
    }

    public static IEnumerable<object[]> AllExamples() => TestData.ExampleFiles();
}
