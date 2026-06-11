using Ds4.Core.Api;

namespace Ds4.Tests;

public sealed class PossiblySemanticsTests
{
    [Fact]
    public void Possibly_Goal_Requires_A_Successful_Goal_Path_From_Each_Initial_State()
    {
        var result = Ds4Facade.Solve("""
            fluents p, q
            actions action
            initially not q
            action causes q if p
            """, "possibly q after action");

        Assert.True(result.Ok, result.Error);
        Assert.False(result.Answer);
        Assert.Contains("Istnieje stan początkowy", result.Explanation);
        Assert.Contains("[1] action: {p, q}", result.Trace);
        Assert.Contains("[1] action: {not p, not q}", result.Trace);
    }

    [Fact]
    public void Possibly_Executable_Requires_A_Full_Path_From_Each_Initial_State()
    {
        var result = Ds4Facade.Solve("""
            fluents loaded, safe
            actions load, check
            initially safe
            impossible load if loaded
            load causes loaded if true
            check causes safe if true
            """, "possibly executable after load; check");

        Assert.True(result.Ok, result.Error);
        Assert.False(result.Answer);
    }

    [Fact]
    public void Possibly_Goal_Is_True_When_Each_Initial_State_Has_A_Goal_Witness()
    {
        var result = Ds4Facade.Solve("""
            fluents p, q
            actions set_p
            initially true
            set_p causes p if true
            """, "possibly p after set_p");

        Assert.True(result.Ok, result.Error);
        Assert.True(result.Answer);
    }

    [Fact]
    public void Necessary_Goal_Still_Requires_All_Full_Paths_To_Satisfy_Goal()
    {
        var result = Ds4Facade.Solve("""
            fluents heads
            actions toss
            initially heads
            toss releases heads if true
            """, "necessary heads after toss");

        Assert.True(result.Ok, result.Error);
        Assert.False(result.Answer);
    }
}
