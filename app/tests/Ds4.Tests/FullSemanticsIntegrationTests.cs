namespace Ds4.Tests;

public sealed class FullSemanticsIntegrationTests
{
    [Fact]
    public void Full_System_Causes_Impossible_Release_Always_And_Process_Agree()
    {
        var domain = """
            fluents has_key, door_open, alarm, power, safe
            actions find_key, open_door, reset_alarm, power_cut
            initially not has_key and not door_open and alarm and power
            always door_open implies has_key
            always not power implies not safe
            find_key causes has_key if true
            open_door causes door_open if has_key
            impossible open_door if not has_key
            reset_alarm causes not alarm if door_open
            power_cut releases power if true
            """;

        var possibleOpen = TestData.Solve(domain, "possibly door_open after find_key; open_door");
        var necessaryOpen = TestData.Solve(domain, "necessary door_open after find_key; open_door");
        var blockedOpen = TestData.Solve(domain, "possibly executable after open_door");
        var possibleNoPower = TestData.Solve(domain, "possibly not power after power_cut");
        var necessaryNoPower = TestData.Solve(domain, "necessary not power after power_cut");

        Assert.True(possibleOpen.Ok, possibleOpen.Error);
        Assert.True(necessaryOpen.Ok, necessaryOpen.Error);
        Assert.True(blockedOpen.Ok, blockedOpen.Error);
        Assert.True(possibleNoPower.Ok, possibleNoPower.Error);
        Assert.True(necessaryNoPower.Ok, necessaryNoPower.Error);

        Assert.True(possibleOpen.Answer);
        Assert.True(necessaryOpen.Answer);
        Assert.False(blockedOpen.Answer);
        Assert.True(possibleNoPower.Answer);
        Assert.False(necessaryNoPower.Answer);
    }

    [Fact]
    public void Composite_Conflict_Nondeterminism_Makes_Possibly_True_And_Necessary_False()
    {
        var domain = """
            fluents p, q
            actions make_p, make_not_p, make_q
            initially not p and not q
            make_p causes p if true
            make_not_p causes not p if true
            make_q causes q if true
            """;

        var possibleP = TestData.Solve(domain, "possibly p after {make_p,make_not_p,make_q}");
        var necessaryP = TestData.Solve(domain, "necessary p after {make_p,make_not_p,make_q}");
        var necessaryQ = TestData.Solve(domain, "necessary q after {make_p,make_not_p,make_q}");

        Assert.True(possibleP.Ok, possibleP.Error);
        Assert.True(necessaryP.Ok, necessaryP.Error);
        Assert.True(necessaryQ.Ok, necessaryQ.Error);

        Assert.True(possibleP.Answer);
        Assert.False(necessaryP.Answer);
        Assert.True(necessaryQ.Answer);
    }

    [Fact]
    public void Noninertial_And_Always_Can_Recompute_Derived_Fluents()
    {
        var domain = """
            fluents s1, s2, light, alarm
            actions toggle1, toggle2
            initially not s1 and not s2
            always (s1 or s2) implies light
            always light implies alarm
            noninertial light
            noninertial alarm
            toggle1 causes s1 if not s1
            toggle2 causes s2 if not s2
            """;

        var result = TestData.Solve(domain, "necessary alarm after {toggle1,toggle2}");

        Assert.True(result.Ok, result.Error);
        Assert.True(result.Answer);
    }

    [Fact]
    public void After_Assertion_Is_Global_Validity_Not_An_Initial_State_Filter()
    {
        // Σ₀ pochodzi wyłącznie z initially. "q after a" jest globalnym warunkiem
        // poprawności modelu (M2), a NIE filtrem stanów początkowych. Przy częściowym
        // opisie {not q} stan {not p, not q} łamie warunek (a nie produkuje q),
        // wiec cala dziedzina jest sprzeczna; filtr zostawilby {p, not q}.
        var partialInitial = """
            fluents p, q
            actions a
            initially not q
            a causes q if p
            q after a
            """;

        var inconsistent = TestData.Solve(partialInitial, "necessary p after epsilon");

        Assert.False(inconsistent.Ok);
        Assert.Equal(0, inconsistent.Sigma0Count);

        // Gdy każdy stan początkowy spełnia warunek, dziedzina jest spójna.
        var determinedInitial = """
            fluents p, q
            actions a
            initially p and not q
            a causes q if p
            q after a
            """;

        var consistent = TestData.Solve(determinedInitial, "necessary p after epsilon");

        Assert.True(consistent.Ok, consistent.Error);
        Assert.True(consistent.Answer);
    }

    [Fact]
    public void Observable_After_Assertion_Keeps_States_With_At_Least_One_Supporting_Path()
    {
        var domain = """
            fluents loaded, alive
            actions spin, shoot
            initially alive
            spin releases loaded if true
            shoot causes not alive if loaded
            observable not alive after spin; shoot
            """;

        var result = TestData.Solve(domain, "possibly not alive after spin; shoot");

        Assert.True(result.Ok, result.Error);
        Assert.True(result.Answer);
    }

    [Fact]
    public void Every_Returned_Successor_Of_An_Active_Cause_Satisfies_That_Cause_Effect()
    {
        var model = TestData.BuildModel("""
            fluents p, q, r
            actions a
            initially p and not q and r
            a causes q if p
            a causes r if true
            """);
        var engine = new Ds4.Core.Semantics.SimpleActionEngine(model.Domain, model.Sigma);

        var result = engine.Res("a", model.Sigma0.Single());

        Assert.NotEmpty(result);
        Assert.All(result, state => Assert.True(state.IsTrue("q")));
        Assert.All(result, state => Assert.True(state.IsTrue("r")));
    }
}
