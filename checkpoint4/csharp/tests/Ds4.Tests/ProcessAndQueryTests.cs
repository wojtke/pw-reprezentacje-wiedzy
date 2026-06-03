using Ds4.Core.Parser;
using Ds4.Core.Query;
using Ds4.Core.Semantics;

namespace Ds4.Tests;

public sealed class ProcessAndQueryTests
{
    [Fact]
    public void ProcessEvaluator_Propagates_States_Through_Multiple_Steps()
    {
        var model = TestData.BuildModel("""
            fluents p, q
            actions make_p, make_q
            initially not p and not q
            make_p causes p if true
            make_q causes q if p
            """);
        var process = ProcessParser.Parse("make_p; make_q");
        var evaluator = BuildProcessEvaluator(model);

        var finals = evaluator.FinalStates(model.Sigma0, process);

        Assert.Single(finals);
        Assert.True(finals.Single().IsTrue("p"));
        Assert.True(finals.Single().IsTrue("q"));
    }

    [Fact]
    public void ProcessEvaluator_Marks_Blocked_Nodes()
    {
        var model = TestData.BuildModel("""
            fluents loaded
            actions load
            initially loaded
            impossible load if loaded
            """);
        var process = ProcessParser.Parse("load");
        var evaluator = BuildProcessEvaluator(model);

        var tree = evaluator.BuildTree(model.Sigma0, process);

        Assert.Empty(tree.FinalNodes());
        Assert.Single(tree.BlockedNodes());
    }

    [Fact]
    public void PossiblyExecutable_Is_True_When_At_Least_One_Full_Path_Exists()
    {
        var result = TestData.Solve("""
            fluents p
            actions free, stop
            initially not p
            free releases p if true
            impossible stop if p
            """, "possibly executable after free; stop");

        Assert.True(result.Ok);
        Assert.True(result.Answer);
    }

    [Fact]
    public void NecessaryExecutable_Is_False_When_One_Path_Blocks()
    {
        var result = TestData.Solve("""
            fluents p
            actions free, stop
            initially not p
            free releases p if true
            impossible stop if p
            """, "necessary executable after free; stop");

        Assert.True(result.Ok);
        Assert.False(result.Answer);
    }

    [Fact]
    public void PossiblyGoal_Is_True_When_One_Final_State_Satisfies_Goal()
    {
        var result = TestData.Solve("""
            fluents loaded, alive
            actions spin, shoot
            initially alive
            spin releases loaded if true
            shoot causes not alive if loaded
            """, "possibly not alive after spin; shoot");

        Assert.True(result.Ok);
        Assert.True(result.Answer);
    }

    [Fact]
    public void NecessaryGoal_Is_False_When_Not_All_Final_States_Satisfy_Goal()
    {
        var result = TestData.Solve("""
            fluents loaded, alive
            actions spin, shoot
            initially alive
            spin releases loaded if true
            shoot causes not alive if loaded
            """, "necessary not alive after spin; shoot");

        Assert.True(result.Ok);
        Assert.False(result.Answer);
    }

    [Fact]
    public void NecessaryGoal_Implies_NecessaryExecutability()
    {
        var result = TestData.Solve("""
            fluents loaded, done
            actions load, finish
            initially loaded
            impossible load if loaded
            finish causes done if true
            """, "necessary done after load; finish");

        Assert.True(result.Ok);
        Assert.False(result.Answer);
    }

    [Fact]
    public void Empty_Process_Query_Uses_Initial_States_As_Final_States()
    {
        var result = TestData.Solve("""
            fluents p
            actions a
            initially p
            """, "necessary p after epsilon");

        Assert.True(result.Ok);
        Assert.True(result.Answer);
    }

    [Fact]
    public void Trace_Is_Returned_For_Successful_Solve()
    {
        var result = TestData.Solve("""
            fluents p
            actions make_p
            initially not p
            make_p causes p if true
            """, "possibly p after make_p");

        Assert.True(result.Ok);
        Assert.NotNull(result.Trace);
        Assert.Contains("start", result.Trace.ToLowerInvariant());
        Assert.Contains("make_p", result.Trace.ToLowerInvariant());
    }

    private static ProcessEvaluator BuildProcessEvaluator(TransitionModel model)
    {
        var simple = new SimpleActionEngine(model.Domain, model.Sigma);
        var conflicts = new ConflictDetector(simple);
        var decompositions = new DecompositionGenerator(simple, conflicts);
        var composite = new CompositeActionEngine(simple, decompositions);
        return new ProcessEvaluator(composite);
    }
}
