using Ds4.Core.Model;
using Ds4.Core.Parser;
using Ds4.Core.Query;

namespace Ds4.Tests;

public sealed class LanguageCoverageTests
{
    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("p", true)]
    [InlineData("q", true)]
    [InlineData("r", false)]
    [InlineData("not p", false)]
    [InlineData("not r", true)]
    [InlineData("p and q", true)]
    [InlineData("p and r", false)]
    [InlineData("r or q", true)]
    [InlineData("p implies q", true)]
    [InlineData("q implies r", false)]
    [InlineData("p iff q", true)]
    [InlineData("p iff r", false)]
    [InlineData("(p and q) or r", true)]
    [InlineData("p and (q or r)", true)]
    public void FormulaParser_Covers_All_Logical_Words(string formulaText, bool expected)
    {
        var formula = FormulaParser.Parse(formulaText);
        var state = new State(new[] { "p", "q" });

        Assert.Equal(expected, formula.Evaluate(state));
    }

    [Theory]
    [InlineData("not p")]
    [InlineData("p and q")]
    [InlineData("p or q")]
    [InlineData("p implies q")]
    [InlineData("p iff q")]
    public void FormulaParser_Accepts_Only_Word_Operators(string formulaText)
    {
        var formula = FormulaParser.Parse(formulaText);
        Assert.NotNull(formula);
    }

    [Theory]
    [InlineData("not p", "!p")]
    [InlineData("not p", "~p")]
    [InlineData("not p", "¬p")]
    [InlineData("p and q", "p & q")]
    [InlineData("p and q", "p ∧ q")]
    [InlineData("p or q", "p | q")]
    [InlineData("p or q", "p ∨ q")]
        [InlineData("p implies q", "p => q")]
        public void FormulaParser_Rejects_Symbolic_Operators(string wordForm, string symbolicForm)
    {
        Assert.NotNull(FormulaParser.Parse(wordForm));
        Assert.Throws<ParseException>(() => FormulaParser.Parse(symbolicForm));
    }

    [Theory]
    [InlineData("possibly executable", Quantifier.Possibly, QueryKind.Executable, 0)]
    [InlineData("possibly executable after epsilon", Quantifier.Possibly, QueryKind.Executable, 0)]
    [InlineData("possibly executable after a", Quantifier.Possibly, QueryKind.Executable, 1)]
    [InlineData("necessary executable after a; b", Quantifier.Necessary, QueryKind.Executable, 2)]
    [InlineData("possibly p after a", Quantifier.Possibly, QueryKind.AfterGoal, 1)]
    [InlineData("necessary p after {a,b}", Quantifier.Necessary, QueryKind.AfterGoal, 1)]
    [InlineData("necessary (p and q) after {a,b}; c", Quantifier.Necessary, QueryKind.AfterGoal, 2)]
    public void QueryParser_Covers_All_Query_Forms(string queryText, Quantifier expectedQuantifier, QueryKind expectedKind, int expectedSteps)
    {
        var query = QueryParser.Parse(queryText);

        Assert.Equal(expectedQuantifier, query.Quantifier);
        Assert.Equal(expectedKind, query.Kind);
        Assert.Equal(expectedSteps, query.Process.Steps.Count);
    }

    [Fact]
    public void ProcessParser_Covers_Empty_Sequential_Comma_And_Composite_Syntax()
    {
        Assert.Empty(ProcessParser.Parse("epsilon").Steps);
        Assert.Empty(ProcessParser.Parse("epsilon").Steps);

        var commaSequential = ProcessParser.Parse("a, b, c");
        Assert.Equal(3, commaSequential.Steps.Count);
        Assert.All(commaSequential.Steps, step => Assert.Single(step.Actions));

        var compositeOnly = ProcessParser.Parse("{a,b,c}");
        Assert.Single(compositeOnly.Steps);
        Assert.Equal(new[] { "a", "b", "c" }, compositeOnly.Steps[0].Actions);

        var mixed = ProcessParser.Parse("{a,b}; c; {d,e}");
        Assert.Equal(3, mixed.Steps.Count);
        Assert.Equal(2, mixed.Steps[0].Actions.Count);
        Assert.Single(mixed.Steps[1].Actions);
        Assert.Equal(2, mixed.Steps[2].Actions.Count);
    }

    [Fact]
    public void DomainParser_Covers_All_Domain_Keywords_And_Default_If_True_Conditions()
    {
        var domain = DomainParser.Parse("""
            # metadata and comments are allowed
            fluent p, q
            action a, b, c
            always p implies q
            initially not p
            noninertial q
            a causes p
            b releases q
            impossible c
            observable q after a
            q after a
            """);

        Assert.Contains("p", domain.Fluents);
        Assert.Contains("q", domain.Fluents);
        Assert.Contains("a", domain.Actions);
        Assert.Contains("b", domain.Actions);
        Assert.Contains("c", domain.Actions);
        Assert.Single(domain.AlwaysConstraints);
        Assert.Single(domain.InitiallyConstraints);
        Assert.Single(domain.NonInertialFluents);
        Assert.Single(domain.CauseRules);
        Assert.Single(domain.ReleaseRules);
        Assert.Single(domain.ImpossibleRules);
        Assert.Equal(2, domain.AfterAssertions.Count);
        Assert.Contains(domain.AfterAssertions, assertion => assertion.Observable);
        Assert.Contains(domain.AfterAssertions, assertion => !assertion.Observable);
        Assert.True(domain.CauseRules.Single().Condition.Evaluate(new State(Array.Empty<string>())));
        Assert.True(domain.ReleaseRules.Single().Condition.Evaluate(new State(Array.Empty<string>())));
        Assert.True(domain.ImpossibleRules.Single().Condition.Evaluate(new State(Array.Empty<string>())));
    }

    [Fact]
    public void Facade_Prepares_Multiline_Query_And_Strips_Query_Comments()
    {
        var result = TestData.Solve("""
            fluents p, q
            actions a
            initially not p and not q
            a causes p if true
            a causes q if p
            """, """
            // first action makes p
            possibly q
            after a; a # second action sees p and makes q
            """);

        Assert.True(result.Ok, result.Error);
        Assert.True(result.Answer);
        Assert.Contains("[2] a: {p, q}", result.Trace);
    }

    [Fact]
    public void EndToEnd_Covers_Causes_Releases_Impossible_Always_Noninertial_Initially_And_Composite_Process()
    {
        var domain = """
            fluents key, door_open, alarm, power, safe
            actions find_key, open_door, reset_alarm, cut_power
            initially not key and not door_open and alarm and power
            always door_open implies key
            always not power implies not safe
            noninertial safe
            find_key causes key if true
            open_door causes door_open if key
            impossible open_door if not key
            reset_alarm causes not alarm if door_open
            cut_power releases power if true
            """;

        var blocked = TestData.Solve(domain, "possibly executable after open_door");
        var full = TestData.Solve(domain, "necessary not alarm after find_key; open_door; reset_alarm");
        var releasedPossible = TestData.Solve(domain, "possibly not safe after cut_power");
        var releasedNecessary = TestData.Solve(domain, "necessary not safe after cut_power");

        Assert.True(blocked.Ok, blocked.Error);
        Assert.True(full.Ok, full.Error);
        Assert.True(releasedPossible.Ok, releasedPossible.Error);
        Assert.True(releasedNecessary.Ok, releasedNecessary.Error);

        Assert.False(blocked.Answer);
        Assert.True(full.Answer);
        Assert.True(releasedPossible.Answer);
        Assert.False(releasedNecessary.Answer);
    }

    [Fact]
    public void Observable_After_And_After_Assertions_Filter_Initial_States_Differently()
    {
        var necessaryDomain = """
            fluents p, q
            actions a
            initially not q
            a causes q if p
            q after a
            """;
        var observableDomain = """
            fluents p, q
            actions a
            initially not q
            a causes q if p
            observable q after a
            """;

        var necessary = TestData.Solve(necessaryDomain, "necessary p after epsilon");
        var observable = TestData.Solve(observableDomain, "possibly p after epsilon");

        Assert.True(necessary.Ok, necessary.Error);
        Assert.True(observable.Ok, observable.Error);
        Assert.True(necessary.Answer);
        Assert.True(observable.Answer);
    }

    [Fact]
    public void Conditional_Cause_Regression_Is_Covered_At_Query_Level_With_Per_Initial_State_Possibly_Semantics()
    {
        var domain = """
            fluents p, q
            actions action
            initially not q
            action causes q if p
            """;

        var possibly = TestData.Solve(domain, "possibly q after action");
        var necessary = TestData.Solve(domain, "necessary q after action");

        Assert.True(possibly.Ok, possibly.Error);
        Assert.True(necessary.Ok, necessary.Error);
        Assert.False(possibly.Answer);
        Assert.False(necessary.Answer);
        Assert.Contains("[0] start: {p, not q}", possibly.Trace);
        Assert.Contains("[1] action: {p, q}", possibly.Trace);
        Assert.DoesNotContain("[1] action: {p, not q}", possibly.Trace);
    }
}
