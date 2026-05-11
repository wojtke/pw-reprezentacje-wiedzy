using Ds4.Core.Parser;
using Ds4.Core.Semantics;

namespace Ds4.Tests;

public sealed class SimpleActionEngineTests
{
    private static SimpleActionEngine Engine(string domainText, out TransitionModel model)
    {
        model = TestData.BuildModel(domainText);
        return new SimpleActionEngine(model.Domain, model.Sigma);
    }

    [Fact]
    public void Res_Applies_Cause_Effect()
    {
        var engine = Engine("""
            fluents p
            actions make_p
            initially !p
            make_p causes p if true
            """, out var model);

        var result = engine.Res("make_p", model.Sigma0.Single());

        Assert.Single(result);
        Assert.True(result.Single().IsTrue("p"));
    }

    [Fact]
    public void Res_Returns_Empty_When_Impossible_Condition_Holds()
    {
        var engine = Engine("""
            fluents loaded
            actions load
            initially loaded
            impossible load if loaded
            load causes loaded if !loaded
            """, out var model);

        var result = engine.Res("load", model.Sigma0.Single());

        Assert.Empty(result);
    }

    [Fact]
    public void Res_Ignores_Impossible_When_Condition_Does_Not_Hold()
    {
        var engine = Engine("""
            fluents loaded
            actions load
            initially !loaded
            impossible load if loaded
            load causes loaded if !loaded
            """, out var model);

        var result = engine.Res("load", model.Sigma0.Single());

        Assert.Single(result);
        Assert.True(result.Single().IsTrue("loaded"));
    }

    [Fact]
    public void Res_Uses_Inertia_For_Unmentioned_Fluents()
    {
        var engine = Engine("""
            fluents p, q
            actions make_p
            initially !p and q
            make_p causes p if true
            """, out var model);

        var result = engine.Res("make_p", model.Sigma0.Single());

        Assert.Single(result);
        Assert.True(result.Single().IsTrue("p"));
        Assert.True(result.Single().IsTrue("q"));
    }

    [Fact]
    public void Res_Release_Creates_Nondeterminism_For_Released_Fluent()
    {
        var engine = Engine("""
            fluents loaded
            actions spin
            initially !loaded
            spin releases loaded if true
            """, out var model);

        var result = engine.Res("spin", model.Sigma0.Single());

        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.IsTrue("loaded"));
        Assert.Contains(result, s => !s.IsTrue("loaded"));
    }

    [Fact]
    public void Res_Respects_Always_Constraints_As_Ramifications()
    {
        var engine = Engine("""
            fluents switch_on, lamp_on
            actions flip
            initially !switch_on and !lamp_on
            always switch_on -> lamp_on
            flip causes switch_on if true
            """, out var model);

        var result = engine.Res("flip", model.Sigma0.Single());

        Assert.Single(result);
        Assert.True(result.Single().IsTrue("switch_on"));
        Assert.True(result.Single().IsTrue("lamp_on"));
    }

    [Fact]
    public void ActiveEffects_Only_Return_Rules_Whose_Condition_Holds()
    {
        var engine = Engine("""
            fluents p, q
            actions a
            initially p and !q
            a causes q if p
            a causes !q if !p
            """, out var model);

        var effects = engine.ActiveEffects("a", model.Sigma0.Single());

        Assert.Single(effects);
        Assert.True(effects.Single().Evaluate(new Ds4.Core.Model.State(new[] { "q" })));
    }

    [Fact]
    public void Unknown_Action_With_No_Rules_Is_Executable_As_Noop()
    {
        var engine = Engine("""
            fluents p
            actions noop
            initially p
            """, out var model);

        var result = engine.Res("noop", model.Sigma0.Single());

        Assert.Single(result);
        Assert.True(result.Single().IsTrue("p"));
    }
}
