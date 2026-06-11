using Ds4.Core.Semantics;

namespace Ds4.Tests;

public sealed class StateAndModelBuilderTests
{
    [Fact]
    public void StateGenerator_Generates_All_Valuations()
    {
        var states = StateGenerator.GenerateAll(new[] { "p", "q", "r" });

        Assert.Equal(8, states.Count);
        Assert.Contains(states, s => !s.IsTrue("p") && !s.IsTrue("q") && !s.IsTrue("r"));
        Assert.Contains(states, s => s.IsTrue("p") && s.IsTrue("q") && s.IsTrue("r"));
    }

    [Fact]
    public void ModelBuilder_Filters_Sigma_By_Always()
    {
        var model = TestData.BuildModel("""
            fluents p, q
            actions noop
            always p implies q
            initially true
            """);

        Assert.Equal(3, model.Sigma.Count);
        Assert.DoesNotContain(model.Sigma, s => s.IsTrue("p") && !s.IsTrue("q"));
    }

    [Fact]
    public void ModelBuilder_Filters_Sigma0_By_Initially()
    {
        var model = TestData.BuildModel("""
            fluents p, q
            actions noop
            initially p
            """);

        Assert.Equal(4, model.Sigma.Count);
        Assert.Equal(2, model.Sigma0.Count);
        Assert.All(model.Sigma0, s => Assert.True(s.IsTrue("p")));
    }

    [Fact]
    public void ModelBuilder_Allows_Partial_Initial_Description()
    {
        var model = TestData.BuildModel("""
            fluents p, q, r
            actions noop
            initially p
            """);

        Assert.Equal(4, model.Sigma0.Count);
    }

    [Fact]
    public void ValidateDomain_Returns_Error_For_Contradictory_Always()
    {
        var result = TestData.Solve("""
            fluents p
            actions a
            always p
            always not p
            initially p
            """, "possibly executable after a");

        Assert.False(result.Ok);
        Assert.Contains("Model jest pusty", result.Error ?? "");
    }

    [Fact]
    public void ValidateDomain_Returns_Error_For_Contradictory_Initially()
    {
        var result = TestData.Solve("""
            fluents p
            actions a
            initially p
            initially not p
            """, "possibly executable after a");

        Assert.False(result.Ok);
        Assert.Contains("Brak stan", result.Error ?? "");
    }
}
