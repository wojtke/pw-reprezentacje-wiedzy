using Ds4.Core.Parser;

namespace Ds4.Tests;

public sealed class WordOnlySyntaxTests
{
    [Theory]
    [InlineData("not p")]
    [InlineData("p and q")]
    [InlineData("p or q")]
    [InlineData("p implies q")]
    [InlineData("p iff q")]
    [InlineData("(p and q) implies (r or s)")]
    public void Word_Operators_Are_Accepted(string text)
    {
        Assert.NotNull(FormulaParser.Parse(text));
    }

    [Theory]
    [InlineData("!p")]
    [InlineData("~p")]
    [InlineData("¬p")]
    [InlineData("p & q")]
    [InlineData("p | q")]
    [InlineData("p -> q")]
    [InlineData("p => q")]
    [InlineData("p <-> q")]
    [InlineData("⊤")]
    [InlineData("⊥")]
    public void Symbolic_Operators_Are_Rejected(string text)
    {
        Assert.Throws<ParseException>(() => FormulaParser.Parse(text));
    }

    [Fact]
    public void ProcessParser_Uses_Word_Epsilon_Instead_Of_Symbol()
    {
        Assert.Empty(ProcessParser.Parse("epsilon").Steps);
        Assert.Throws<ParseException>(() => ProcessParser.Parse("ε"));
    }

    [Fact]
    public void User_Facing_Examples_Use_Word_Operators_In_Formulas()
    {
        foreach (var path in Directory.GetFiles(TestData.ExamplesDir(), "*.*")
                     .Where(p => p.EndsWith(".domain", StringComparison.OrdinalIgnoreCase) || p.EndsWith(".query", StringComparison.OrdinalIgnoreCase)))
        {
            var text = File.ReadAllText(path);
            Assert.DoesNotContain("!", text);
            Assert.DoesNotContain("->", text);
            Assert.DoesNotContain("<->", text);
            Assert.DoesNotContain("=>", text);
            Assert.DoesNotContain("¬", text);
            Assert.DoesNotContain("⊤", text);
            Assert.DoesNotContain("⊥", text);
            Assert.DoesNotContain("∧", text);
            Assert.DoesNotContain("∨", text);
        }
    }
}
