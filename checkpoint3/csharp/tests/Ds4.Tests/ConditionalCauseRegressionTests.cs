using Ds4.Core.Query;
using Ds4.Core.Semantics;

namespace Ds4.Tests;

public sealed class ConditionalCauseRegressionTests
{
    private const string DomainText = """
        fluents p, q
        actions action
        initially !q
        action causes q if p
        """;

    [Fact]
    public void Conditional_Cause_Forces_Effect_On_Branch_Where_Condition_Holds()
    {
        var model = TestData.BuildModel(DomainText);
        var engine = new SimpleActionEngine(model.Domain, model.Sigma);
        var start = model.Sigma0.Single(s => s.IsTrue("p") && !s.IsTrue("q"));

        var result = engine.Res("action", start);

        Assert.Single(result);
        Assert.True(result.Single().IsTrue("p"));
        Assert.True(result.Single().IsTrue("q"));
    }

    [Fact]
    public void Conditional_Cause_Is_Noop_On_Branch_Where_Condition_Does_Not_Hold()
    {
        var model = TestData.BuildModel(DomainText);
        var engine = new SimpleActionEngine(model.Domain, model.Sigma);
        var start = model.Sigma0.Single(s => !s.IsTrue("p") && !s.IsTrue("q"));

        var result = engine.Res("action", start);

        Assert.Single(result);
        Assert.False(result.Single().IsTrue("p"));
        Assert.False(result.Single().IsTrue("q"));
    }

    [Fact]
    public void Query_Possibly_Q_After_Action_Is_False_Under_Partial_Initial()
    {
        // Sigma0 zawiera {p, ¬q} oraz {¬p, ¬q}. Galaz bez p nigdy nie produkuje q,
        // wiec pod semantyka possibly = (dla kazdego sigma0 istnieje sciezka) odpowiedz jest NIE.
        var result = TestData.Solve(DomainText, "possibly q after action");

        Assert.True(result.Ok, result.Error);
        Assert.False(result.Answer);
    }

    [Fact]
    public void Query_Necessary_Q_After_Action_Is_False_For_Regression_Domain()
    {
        var result = TestData.Solve(DomainText, "necessary q after action");

        Assert.True(result.Ok, result.Error);
        Assert.False(result.Answer);
    }

    [Fact]
    public void Trace_Shows_Both_Branches_Under_Partial_Initial()
    {
        var result = TestData.Solve(DomainText, "possibly q after action");

        Assert.True(result.Ok, result.Error);
        Assert.Contains("[0] start -> {p, ¬q}", result.Trace);
        Assert.Contains("[1] action -> {p, q}", result.Trace);
        Assert.Contains("[0] start -> {¬p, ¬q}", result.Trace);
        Assert.Contains("[1] action -> {¬p, ¬q}", result.Trace);
    }

    [Fact]
    public void Process_Evaluator_Final_States_Contain_Both_Expected_Branches()
    {
        var model = TestData.BuildModel(DomainText);
        var simple = new SimpleActionEngine(model.Domain, model.Sigma);
        var conflicts = new ConflictDetector(simple);
        var decompositions = new DecompositionGenerator(simple, conflicts);
        var composite = new CompositeActionEngine(simple, decompositions);
        var process = new Ds4Process(new[] { new CompositeAction(new[] { "action" }) });
        var evaluator = new ProcessEvaluator(composite);

        var finals = evaluator.FinalStates(model.Sigma0, process);

        Assert.Equal(2, finals.Count);
        Assert.Contains(finals, s => !s.IsTrue("p") && !s.IsTrue("q"));
        Assert.Contains(finals, s => s.IsTrue("p") && s.IsTrue("q"));
        Assert.DoesNotContain(finals, s => s.IsTrue("p") && !s.IsTrue("q"));
    }
}
