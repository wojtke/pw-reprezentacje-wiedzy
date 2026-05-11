using Ds4.Core.Formula;
using Ds4.Core.Model;
using Ds4.Core.Parser;

namespace Ds4.Tests;

public sealed class FormulaTests
{
    [Fact]
    public void Atom_Uses_State_Value()
    {
        var state = new State(new[] { "p" });

        Assert.True(new Atom("p").Evaluate(state));
        Assert.False(new Atom("q").Evaluate(state));
    }

    [Fact]
    public void Negation_And_Conjunction_Work()
    {
        var formula = new And(new Atom("p"), new Not(new Atom("q")));
        var state = new State(new[] { "p" });

        Assert.True(formula.Evaluate(state));
    }

    [Theory]
    [InlineData("p and !q", true)]
    [InlineData("p and q", false)]
    [InlineData("p or q", true)]
    [InlineData("q or r", false)]
    [InlineData("p -> q", false)]
    [InlineData("q -> p", true)]
    [InlineData("p <-> q", false)]
    [InlineData("p <-> !q", true)]
    [InlineData("(p and !q) or r", true)]
    public void FormulaParser_Evaluates_Common_Operators(string text, bool expected)
    {
        var formula = FormulaParser.Parse(text);
        var state = new State(new[] { "p" });

        Assert.Equal(expected, formula.Evaluate(state));
    }

    [Fact]
    public void Parser_Respects_Operator_Precedence()
    {
        var parsed = FormulaParser.Parse("p or q and r");
        var state = new State(new[] { "q" });

        Assert.False(parsed.Evaluate(state));
    }

    [Fact]
    public void ToDnf_For_Disjunction_Returns_Alternative_Terms()
    {
        var formula = FormulaParser.Parse("p or q");
        var dnf = formula.ToDnf();

        Assert.Equal(2, dnf.Count);
        Assert.Contains(dnf, t => t.ContainsFluent("p"));
        Assert.Contains(dnf, t => t.ContainsFluent("q"));
    }

    [Fact]
    public void ToDnf_Removes_Contradictory_Conjunction()
    {
        var formula = FormulaParser.Parse("p and !p");
        var dnf = formula.ToDnf();

        Assert.Empty(dnf);
    }

    [Fact]
    public void Negation_Normal_Form_Handles_DeMorgan()
    {
        var formula = FormulaParser.Parse("!(p and q)");
        var dnf = formula.ToDnf();

        Assert.Equal(2, dnf.Count);
        Assert.Contains(dnf, t => t.Literals.TryGetValue("p", out var value) && value == false);
        Assert.Contains(dnf, t => t.Literals.TryGetValue("q", out var value) && value == false);
    }
}
