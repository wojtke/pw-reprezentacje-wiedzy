using Ds4.Core.Parser;
using Ds4.Core.Semantics;

namespace Ds4.Tests;

public sealed class KeywordLikeActionNameRegressionTests
{
    [Fact]
    public void Action_Named_Action_Is_Not_Treated_As_Action_Declaration_When_It_Causes_Effect()
    {
        const string domainText = """
            fluents p, q
            actions action
            initially p and !q
            action causes q if p
            """;

        var domain = DomainParser.Parse(domainText);

        Assert.Contains(domain.CauseRules, r => r.Action == "action");
        Assert.DoesNotContain(domain.Actions, a => a.Equals("causes q if p", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Action_Named_Action_Forces_Conditional_Effect_When_Condition_Holds()
    {
        const string domainText = """
            fluents p, q
            actions action
            initially !q
            action causes q if p
            """;

        var model = TestData.BuildModel(domainText);
        var engine = new SimpleActionEngine(model.Domain, model.Sigma);
        var start = model.Sigma0.Single(s => s.IsTrue("p") && !s.IsTrue("q"));

        var result = engine.Res("action", start);

        Assert.Single(result);
        Assert.True(result.Single().IsTrue("p"));
        Assert.True(result.Single().IsTrue("q"));
    }

    [Fact]
    public void Action_Named_Fluent_Is_Not_Treated_As_Fluent_Declaration_When_It_Causes_Effect()
    {
        const string domainText = """
            fluents p, q
            actions fluent
            initially p and !q
            fluent causes q if p
            """;

        var result = TestData.Solve(domainText, "possibly q after fluent");

        Assert.True(result.Ok, result.Error);
        Assert.True(result.Answer);
    }

    [Fact]
    public void Action_Named_Action_Can_Release_A_Fluent()
    {
        const string domainText = """
            fluents p, q
            actions action
            initially p and !q
            action releases q if p
            """;

        var domain = DomainParser.Parse(domainText);

        Assert.Contains(domain.ReleaseRules, r => r.Action == "action" && r.Fluent == "q");
        Assert.DoesNotContain(domain.Actions, a => a.Equals("releases q if p", StringComparison.OrdinalIgnoreCase));
    }
}
