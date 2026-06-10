namespace Ds4.Tests;

public sealed class FullSemanticsIntegrationTests
{
    [Fact]
    public void Full_System_Causes_Impossible_Release_Always_And_Process_Agree()
    {
        var domain = """
            fluents has_key, door_open, alarm, power, safe
            actions find_key, open_door, reset_alarm, power_cut
            initially !has_key and !door_open and alarm and power
            always door_open -> has_key
            always !power -> !safe
            find_key causes has_key if true
            open_door causes door_open if has_key
            impossible open_door if !has_key
            reset_alarm causes !alarm if door_open
            power_cut releases power if true
            """;

        var possibleOpen = TestData.Solve(domain, "possibly door_open after find_key; open_door");
        var necessaryOpen = TestData.Solve(domain, "necessary door_open after find_key; open_door");
        var blockedOpen = TestData.Solve(domain, "possibly executable after open_door");
        var possibleNoPower = TestData.Solve(domain, "possibly !power after power_cut");
        var necessaryNoPower = TestData.Solve(domain, "necessary !power after power_cut");

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
            initially !p and !q
            make_p causes p if true
            make_not_p causes !p if true
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
            initially !s1 and !s2
            always (s1 or s2) -> light
            always light -> alarm
            noninertial light
            noninertial alarm
            toggle1 causes s1 if !s1
            toggle2 causes s2 if !s2
            """;

        var result = TestData.Solve(domain, "necessary alarm after {toggle1,toggle2}");

        Assert.True(result.Ok, result.Error);
        Assert.True(result.Answer);
    }

    [Fact]
    public void After_Assertion_Is_Validity_Condition_Not_A_Filter_On_Initial_States()
    {
        // initially !q pozostawia dwa stany początkowe: {p,!q} oraz {!p,!q}.
        // Z {!p,!q} akcja a (causes q if p) nie ustala q, więc "q after a" jest tam
        // fałszywe. Σ₀ wyznaczają WYŁĄCZNIE zdania initially; zdanie after jest
        // warunkiem poprawności modelu (M2), a nie filtrem na Σ₀. Naruszenie z
        // któregokolwiek stanu początkowego czyni dziedzinę sprzeczną (brak modelu,
        // puste Σ₀) — nie usuwa pojedynczego stanu, jak robiła to wcześniejsza wersja.
        var domain = """
            fluents p, q
            actions a
            initially !q
            a causes q if p
            q after a
            """;

        var result = TestData.Solve(domain, "necessary p after epsilon");

        Assert.False(result.Ok); // sprzeczna dziedzina: zdanie after niespełnione z {!p,!q}
    }

    [Fact]
    public void Observable_After_Is_Satisfied_When_Some_Initial_State_Supports_A_Path()
    {
        // observable α after P jest warunkiem poprawności modelu (M2) o charakterze
        // egzystencjalnym: wymaga, by ISTNIAŁ stan początkowy ze śladem kończącym się α.
        // Nie zawęża Σ₀ — tutaj jest spełniony (ze stanu z loaded), więc model istnieje
        // i kwerenda possibly !alive jest prawdziwa.
        var domain = """
            fluents loaded, alive
            actions spin, shoot
            initially alive
            spin releases loaded if true
            shoot causes !alive if loaded
            observable !alive after spin; shoot
            """;

        var result = TestData.Solve(domain, "possibly !alive after spin; shoot");

        Assert.True(result.Ok, result.Error);
        Assert.True(result.Answer);
    }

    [Fact]
    public void Every_Returned_Successor_Of_An_Active_Cause_Satisfies_That_Cause_Effect()
    {
        var model = TestData.BuildModel("""
            fluents p, q, r
            actions a
            initially p and !q and r
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
