using Ds4.Core.Api;

namespace Ds4.Tests;

public sealed class ScenarioExamplesTests
{
    [Fact]
    public void Scenario_Examples_Include_Additional_Story_Cases()
    {
        var ids = Directory.GetFiles(TestData.ExamplesDir(), "scenario_*.domain")
            .Select(Path.GetFileNameWithoutExtension)
            .ToArray();

        Assert.True(ids.Length >= 16, "Expected at least sixteen scenario examples, got " + ids.Length);
        Assert.Contains("scenario_01_philosophers_single_eats", ids);
        Assert.Contains("scenario_02_philosophers_adjacent_pair_blocked", ids);
        Assert.Contains("scenario_03_philosophers_non_adjacent_pair", ids);
    }

    [Theory]
    [InlineData("scenario_01_philosophers_single_eats", true)]
    [InlineData("scenario_02_philosophers_adjacent_pair_blocked", false)]
    [InlineData("scenario_03_philosophers_non_adjacent_pair", true)]
    [InlineData("scenario_04_philosopher_missing_fork", false)]
    [InlineData("scenario_05_robot_unlock_delivery", true)]
    [InlineData("scenario_06_robot_delivery_blocked", false)]
    [InlineData("scenario_07_alarm_ramification", true)]
    [InlineData("scenario_08_alarm_release_not_necessary", false)]
    [InlineData("scenario_09_coin_release_possible_tail", true)]
    [InlineData("scenario_10_coin_release_not_necessary_tail", false)]
    [InlineData("scenario_11_painting_blocks_integrity", true)]
    [InlineData("scenario_12_card_door_blocked", false)]
    [InlineData("scenario_13_turkey_ramification_walking", true)]
    [InlineData("scenario_14_switches_toggle_changes_light", true)]
    [InlineData("scenario_15_soup_and_door_independent", true)]
    [InlineData("scenario_16_disjunction_with_supporting_action", true)]
    public void Scenario_Examples_Return_Expected_Answers(string id, bool expected)
    {
        var domain = File.ReadAllText(Path.Combine(TestData.ExamplesDir(), id + ".domain"));
        var query = File.ReadAllText(Path.Combine(TestData.ExamplesDir(), id + ".query"));

        var result = Ds4Facade.Solve(domain, query);

        Assert.True(result.Ok, id + " failed: " + result.Error);
        Assert.Equal(expected, result.Answer);
    }

    [Fact]
    public void Dining_Philosophers_Adjacent_Actions_Are_Blocked_By_Integrity_Constraint()
    {
        var id = "scenario_02_philosophers_adjacent_pair_blocked";
        var result = Ds4Facade.Solve(
            File.ReadAllText(Path.Combine(TestData.ExamplesDir(), id + ".domain")),
            File.ReadAllText(Path.Combine(TestData.ExamplesDir(), id + ".query")));

        Assert.True(result.Ok, result.Error);
        Assert.False(result.Answer);
        Assert.Contains("BLOCKED", result.Trace);
    }
}
