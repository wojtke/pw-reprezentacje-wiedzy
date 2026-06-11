namespace Ds4.Tests;

// Semantyka kwerendy possibly w DS4: dla kazdego sigma0 ∈ Sigma0 istnieje sciezka spelniajaca warunek.
// Te testy chronia przed cofnieciem sie do bledniej "istnieje sigma0, istnieje sciezka".
public sealed class PossiblySemanticsTests
{
    [Fact]
    public void Possibly_Goal_Requires_Path_From_Every_Initial_State()
    {
        // Sigma0 = { {¬p, ¬q}, {p, ¬q} }. Tylko galaz z p produkuje q. NIE.
        var result = TestData.Solve("""
            initially !q
            action causes q if p
            """, "possibly q after action");

        Assert.True(result.Ok, result.Error);
        Assert.False(result.Answer);
    }

    [Fact]
    public void Possibly_Goal_Is_True_When_Every_Initial_State_Has_A_Satisfying_Path()
    {
        // initially true -> Sigma0 = { {}, {p} }. setQ ustawia q w obu przypadkach.
        var result = TestData.Solve("""
            initially true
            setQ causes q if true
            """, "possibly q after setQ");

        Assert.True(result.Ok, result.Error);
        Assert.True(result.Answer);
    }

    [Fact]
    public void Possibly_Executable_Requires_Full_Path_From_Every_Initial_State()
    {
        // initially true -> Sigma0 = { {}, {p} }. Galaz z p blokuje sie. NIE.
        var result = TestData.Solve("""
            initially true
            impossible a if p
            a causes done if true
            """, "possibly executable after a");

        Assert.True(result.Ok, result.Error);
        Assert.False(result.Answer);
    }

    [Fact]
    public void Possibly_Executable_Is_True_When_Every_Initial_State_Has_A_Full_Path()
    {
        // initially true -> Sigma0 = { {}, {p} }. Akcja a wykonalna z obu, daje pelna sciezke.
        var result = TestData.Solve("""
            initially true
            a causes done if true
            """, "possibly executable after a");

        Assert.True(result.Ok, result.Error);
        Assert.True(result.Answer);
    }

    [Fact]
    public void Possibly_With_Single_Initial_State_Matches_Old_Existential_Semantics()
    {
        // Pojedynczy stan poczatkowy: stara i nowa semantyka pokrywaja sie.
        var result = TestData.Solve("""
            initially !p
            toss releases p if true
            """, "possibly p after toss");

        Assert.True(result.Ok, result.Error);
        Assert.True(result.Answer);
    }

    [Fact]
    public void Possibly_Possibly_Of_Released_Goal_Holds_For_Every_Initial_State()
    {
        // Sigma0 = { {}, {p} }: po wykonaniu toss z {} mamy {} lub {p}, z {p} mamy {} lub {p}.
        // W obu przypadkach jest sciezka konczaca sie z p. TAK.
        var result = TestData.Solve("""
            initially true
            toss releases p if true
            """, "possibly p after toss");

        Assert.True(result.Ok, result.Error);
        Assert.True(result.Answer);
    }

    [Fact]
    public void Possibly_Goal_After_Empty_Process_Equals_Necessary_Goal()
    {
        // Sigma0 = { {}, {p} }. possibly p after epsilon = (dla kazdego sigma) sigma |= p => NIE.
        var result = TestData.Solve("""
            initially true
            """, "possibly p after epsilon");

        Assert.True(result.Ok, result.Error);
        Assert.False(result.Answer);
    }
}
