using Ds4.Core.Model;
using Ds4.Core.Parser;

namespace Ds4.Tests;

public sealed class ModelTests
{
    [Fact]
    public void State_Equality_Is_Based_On_True_Fluents()
    {
        var left = new State(new[] { "p", "q" });
        var right = new State(new[] { "q", "p" });

        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void State_Is_Case_Insensitive_For_Lookups()
    {
        var state = new State(new[] { "Loaded" });

        Assert.True(state.IsTrue("loaded"));
        Assert.True(state.IsTrue("LOADED"));
    }

    [Fact]
    public void Domain_InertialFluents_Exclude_NonInertial_Ones()
    {
        var domain = DomainParser.Parse("""
            fluents alarm, door_open
            actions tick
            noninertial alarm
            """);

        Assert.Contains(domain.InertialFluents, f => string.Equals(f, "door_open", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(domain.InertialFluents, f => string.Equals(f, "alarm", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void DomainParser_Infers_Fluents_From_Formulas()
    {
        var domain = DomainParser.Parse("""
            actions open
            initially !closed
            open causes opened if !closed
            """);

        Assert.Contains(domain.Fluents, f => string.Equals(f, "closed", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(domain.Fluents, f => string.Equals(f, "opened", StringComparison.OrdinalIgnoreCase));
    }
}
