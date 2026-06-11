using Ds4.Core.Parser;
using Ds4.Core.Query;

namespace Ds4.Tests;

public sealed class ParserTests
{
    [Fact]
    public void DomainParser_Parses_Basic_Statements()
    {
        var domain = DomainParser.Parse("""
            # comment should be ignored
            fluents loaded, alive
            actions spin, shoot
            initially alive
            always alive or not loaded
            spin releases loaded if true
            shoot causes not alive if loaded
            impossible shoot if not loaded
            noninertial loaded
            """);

        Assert.Equal(2, domain.Fluents.Count);
        Assert.Equal(2, domain.Actions.Count);
        Assert.Single(domain.InitiallyConstraints);
        Assert.Single(domain.AlwaysConstraints);
        Assert.Single(domain.ReleaseRules);
        Assert.Single(domain.CauseRules);
        Assert.Single(domain.ImpossibleRules);
        Assert.Contains(domain.NonInertialFluents, f => string.Equals(f, "loaded", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void DomainParser_Allows_Trailing_Dot()
    {
        var domain = DomainParser.Parse("""
            fluents p.
            actions a.
            initially not p.
            a causes p if true.
            """);

        Assert.Contains(domain.Fluents, f => string.Equals(f, "p", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(domain.Actions, a => string.Equals(a, "a", StringComparison.OrdinalIgnoreCase));
        Assert.Single(domain.CauseRules);
    }

    [Fact]
    public void DomainParser_Parses_After_And_Observable_After()
    {
        var domain = DomainParser.Parse("""
            fluents p
            actions make_p
            initially not p
            make_p causes p if true
            p after make_p
            observable p after make_p
            """);

        Assert.Equal(2, domain.AfterAssertions.Count);
        Assert.Contains(domain.AfterAssertions, a => a.Observable == false);
        Assert.Contains(domain.AfterAssertions, a => a.Observable == true);
    }

    [Theory]
    [InlineData("possibly executable after a", Quantifier.Possibly, QueryKind.Executable, 1)]
    [InlineData("necessary executable after a; b", Quantifier.Necessary, QueryKind.Executable, 2)]
    [InlineData("possibly p after a", Quantifier.Possibly, QueryKind.AfterGoal, 1)]
    [InlineData("necessary p after {a,b}", Quantifier.Necessary, QueryKind.AfterGoal, 1)]
    public void QueryParser_Parses_Query_Forms(string text, Quantifier quantifier, QueryKind kind, int steps)
    {
        var query = QueryParser.Parse(text);

        Assert.Equal(quantifier, query.Quantifier);
        Assert.Equal(kind, query.Kind);
        Assert.Equal(steps, query.Process.Steps.Count);
    }

    [Fact]
    public void QueryParser_Parses_Empty_Process_For_Executable_Query()
    {
        var query = QueryParser.Parse("possibly executable");

        Assert.Equal(QueryKind.Executable, query.Kind);
        Assert.Empty(query.Process.Steps);
    }

    [Fact]
    public void ProcessParser_Semicolon_Means_Sequential_Process()
    {
        var process = ProcessParser.Parse("a; b; c");

        Assert.Equal(3, process.Steps.Count);
        Assert.All(process.Steps, s => Assert.Single(s.Actions));
    }

    [Fact]
    public void ProcessParser_Braces_Mean_Composite_Action()
    {
        var process = ProcessParser.Parse("{a,b}; c");

        Assert.Equal(2, process.Steps.Count);
        Assert.Equal(2, process.Steps[0].Actions.Count);
        Assert.Single(process.Steps[1].Actions);
    }

    [Theory]
    [InlineData("p $")]
    [InlineData("p and")]
    [InlineData("(p or q")]
    public void FormulaParser_Rejects_Broken_Formulas(string text)
    {
        Assert.Throws<ParseException>(() => FormulaParser.Parse(text));
    }

    [Theory]
    [InlineData("maybe executable after a")]
    [InlineData("possibly p a")]
    public void QueryParser_Rejects_Broken_Queries(string text)
    {
        Assert.Throws<ParseException>(() => QueryParser.Parse(text));
    }
}
