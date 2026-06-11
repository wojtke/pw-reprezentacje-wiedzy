using Ds4.Core.Api;
using Ds4.Core.Parser;

namespace Ds4.Tests;

public sealed class CacheAndGraphTests
{
    [Fact]
    public void Facade_Caches_Repeated_Solve_Results_In_Memory()
    {
        Ds4Facade.ClearCaches();
        var domain = """
            fluents p
            actions make_p
            initially not p
            make_p causes p if true
            """;
        var query = "necessary p after make_p";

        var first = Ds4Facade.Solve(domain, query);
        var afterFirst = Ds4Facade.GetCacheStats();
        var second = Ds4Facade.Solve(domain, query);
        var afterSecond = Ds4Facade.GetCacheStats();

        Assert.True(first.Ok, first.Error);
        Assert.True(second.Ok, second.Error);
        Assert.True(first.Answer);
        Assert.Equal(first.Answer, second.Answer);
        Assert.Equal(1, afterFirst.SolveResultCount);
        Assert.Equal(afterFirst.SolveResultCount, afterSecond.SolveResultCount);
        Assert.True(afterSecond.FormulaCount > 0);
    }

    [Fact]
    public void FormulaParser_Cache_Can_Be_Cleared_And_Reused()
    {
        FormulaParser.ClearCache();
        Assert.Equal(0, FormulaParser.CacheCount);

        var first = FormulaParser.Parse("p implies q");
        var second = FormulaParser.Parse("p   implies    q");

        Assert.Same(first, second);
        Assert.Equal(1, FormulaParser.CacheCount);

        FormulaParser.ClearCache();
        Assert.Equal(0, FormulaParser.CacheCount);
    }

    [Fact]
    public void Solve_Result_Contains_Readable_Dot_Graph()
    {
        var result = Ds4Facade.Solve("""
            fluents p, q
            actions a, b
            initially not p and not q
            a causes p if true
            b causes q if p
            """, "necessary q after a; b");

        Assert.True(result.Ok, result.Error);
        Assert.True(result.Answer);
        Assert.Contains("digraph ExecutionTrace", result.TraceGraphDot);
        Assert.Contains("ranksep=1.20", result.TraceGraphDot);
        Assert.Contains("nodesep=1.35", result.TraceGraphDot);
        Assert.Contains("shape=box", result.TraceGraphDot);
        Assert.Contains("final", result.TraceGraphDot);
        Assert.Contains("DS4 execution trace", result.TraceGraphDot);
        Assert.Contains("step 0", result.TraceGraphDot);
    }
}

public sealed class DomainModelCacheTests
{
    [Fact]
    public void Facade_Reuses_Built_Domain_Model_For_Different_Queries()
    {
        Ds4Facade.ClearCaches();
        var domain = """
            fluents p, q
            actions set_p, set_q
            initially not p and not q
            set_p causes p if true
            set_q causes q if true
            """;

        var first = Ds4Facade.Solve(domain, "necessary p after set_p");
        var afterFirst = Ds4Facade.GetCacheStats();
        var second = Ds4Facade.Solve(domain, "necessary q after set_q");
        var afterSecond = Ds4Facade.GetCacheStats();

        Assert.True(first.Ok, first.Error);
        Assert.True(second.Ok, second.Error);
        Assert.True(first.Answer);
        Assert.True(second.Answer);
        Assert.Equal(1, afterFirst.DomainModelCount);
        Assert.Equal(1, afterSecond.DomainModelCount);
        Assert.Equal(1, afterFirst.ParsedQueryCount);
        Assert.Equal(2, afterSecond.ParsedQueryCount);
        Assert.Equal(1, afterFirst.SolveResultCount);
        Assert.Equal(2, afterSecond.SolveResultCount);
    }

    [Fact]
    public void Facade_Normalizes_Domain_Before_Using_Domain_Model_Cache()
    {
        Ds4Facade.ClearCaches();
        var compactDomain = """
            fluents p
            actions set_p
            initially not p
            set_p causes p if true
            """;
        var spacedDomain = """
              fluents    p

              actions    set_p
              initially  not p
              set_p causes p if true
            """;

        var first = Ds4Facade.Solve(compactDomain, "necessary p after set_p");
        var second = Ds4Facade.Solve(spacedDomain, "possibly p after set_p");
        var stats = Ds4Facade.GetCacheStats();

        Assert.True(first.Ok, first.Error);
        Assert.True(second.Ok, second.Error);
        Assert.True(first.Answer);
        Assert.True(second.Answer);
        Assert.Equal(1, stats.DomainModelCount);
        Assert.Equal(2, stats.ParsedQueryCount);
        Assert.Equal(2, stats.SolveResultCount);
    }

    [Fact]
    public void Facade_Does_Not_Mutate_Cached_Domain_Model_With_Query_Only_Symbols()
    {
        Ds4Facade.ClearCaches();
        var domain = """
            fluents p
            actions set_p
            initially not p
            set_p causes p if true
            """;

        var bad = Ds4Facade.Solve(domain, "possibly q after set_p");
        var good = Ds4Facade.Solve(domain, "necessary p after set_p");
        var stats = Ds4Facade.GetCacheStats();

        Assert.False(bad.Ok);
        Assert.Contains("Fluent 'q'", bad.Error);
        Assert.True(good.Ok, good.Error);
        Assert.True(good.Answer);
        Assert.Equal(1, stats.DomainModelCount);
    }
}
