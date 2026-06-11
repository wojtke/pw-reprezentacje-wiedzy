using Ds4.Core.Query;
using Ds4.Core.Semantics;

namespace Ds4.Tests;

public sealed class CompositeActionTests
{
    private static (TransitionModel Model, SimpleActionEngine Simple, ConflictDetector Conflicts, DecompositionGenerator Decompositions, CompositeActionEngine Composite) Build(string domainText)
    {
        var model = TestData.BuildModel(domainText);
        var simple = new SimpleActionEngine(model.Domain, model.Sigma);
        var conflicts = new ConflictDetector(simple);
        var decompositions = new DecompositionGenerator(simple, conflicts);
        var composite = new CompositeActionEngine(simple, decompositions);
        return (model, simple, conflicts, decompositions, composite);
    }

    [Fact]
    public void ConflictDetector_Detects_Opposite_Cause_Effects()
    {
        var env = Build("""
            fluents p
            actions make_p, make_not_p
            initially not p
            make_p causes p if true
            make_not_p causes not p if true
            """);

        var conflict = env.Conflicts.AreInConflict("make_p", "make_not_p", env.Model.Sigma0.Single());

        Assert.True(conflict);
    }

    [Fact]
    public void ConflictDetector_Detects_Cause_Release_Conflict()
    {
        var env = Build("""
            fluents p
            actions make_p, free_p
            initially not p
            make_p causes p if true
            free_p releases p if true
            """);

        var conflict = env.Conflicts.AreInConflict("make_p", "free_p", env.Model.Sigma0.Single());

        Assert.True(conflict);
    }

    [Fact]
    public void ConflictDetector_Does_Not_Report_Independent_Actions()
    {
        var env = Build("""
            fluents p, q
            actions make_p, make_q
            initially not p and not q
            make_p causes p if true
            make_q causes q if true
            """);

        var conflict = env.Conflicts.AreInConflict("make_p", "make_q", env.Model.Sigma0.Single());

        Assert.False(conflict);
    }

    [Fact]
    public void DecompositionGenerator_Returns_One_Set_For_No_Conflict()
    {
        var env = Build("""
            fluents p, q
            actions make_p, make_q
            initially not p and not q
            make_p causes p if true
            make_q causes q if true
            """);

        var decompositions = env.Decompositions.Generate(new CompositeAction(new[] { "make_p", "make_q" }), env.Model.Sigma0.Single());

        Assert.Single(decompositions);
        Assert.Equal(2, decompositions.Single().Count);
    }

    [Fact]
    public void DecompositionGenerator_Returns_Alternatives_For_Conflict()
    {
        var env = Build("""
            fluents p
            actions make_p, make_not_p
            initially not p
            make_p causes p if true
            make_not_p causes not p if true
            """);

        var decompositions = env.Decompositions.Generate(new CompositeAction(new[] { "make_p", "make_not_p" }), env.Model.Sigma0.Single());

        Assert.Equal(2, decompositions.Count);
        Assert.Contains(decompositions, d => d.Count == 1 && d.Contains("make_p", StringComparer.OrdinalIgnoreCase));
        Assert.Contains(decompositions, d => d.Count == 1 && d.Contains("make_not_p", StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void DecompositionGenerator_Drops_Individually_Impossible_Action()
    {
        var env = Build("""
            fluents p, q
            actions make_p, blocked
            initially not p and not q
            make_p causes p if true
            impossible blocked if true
            blocked causes q if true
            """);

        var decompositions = env.Decompositions.Generate(new CompositeAction(new[] { "make_p", "blocked" }), env.Model.Sigma0.Single());

        Assert.Single(decompositions);
        Assert.Single(decompositions.Single());
        Assert.Equal("make_p", decompositions.Single().Single(), StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void CompositeActionEngine_Applies_Independent_Actions_Together()
    {
        var env = Build("""
            fluents p, q
            actions make_p, make_q
            initially not p and not q
            make_p causes p if true
            make_q causes q if true
            """);

        var result = env.Composite.Res(new CompositeAction(new[] { "make_p", "make_q" }), env.Model.Sigma0.Single());

        Assert.Single(result);
        Assert.True(result.Single().IsTrue("p"));
        Assert.True(result.Single().IsTrue("q"));
    }

    [Fact]
    public void CompositeActionEngine_Union_Of_Decomposition_Results_Is_Nondeterministic()
    {
        var env = Build("""
            fluents p
            actions make_p, make_not_p
            initially not p
            make_p causes p if true
            make_not_p causes not p if true
            """);

        var result = env.Composite.Res(new CompositeAction(new[] { "make_p", "make_not_p" }), env.Model.Sigma0.Single());

        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.IsTrue("p"));
        Assert.Contains(result, s => !s.IsTrue("p"));
    }
}
