namespace Ds4.Tests;

public sealed class CrossGuiSourceTests
{
    private static string MainWindowSource()
        => File.ReadAllText(Path.Combine(TestData.RepoRoot(), "src", "Ds4.CrossGui", "MainWindow.cs"));

    [Fact]
    public void CrossGui_Treats_Examples_As_Optional_Panel()
    {
        var source = MainWindowSource();

        Assert.Contains("Pokaż przykłady", source);
        Assert.Contains("Ukryj przykłady", source);
        Assert.Contains("ToggleExamples", source);
        Assert.Contains("_examples.IsVisible = false", source);
        Assert.DoesNotContain("LoadSelectedExample();\n        }\n        else", source);
    }

    [Fact]
    public void CrossGui_Can_Load_Example_After_User_Chooses_It()
    {
        var source = MainWindowSource();

        Assert.Contains("_examples.SelectionChanged +=", source);
        Assert.Contains("var example = Ds4Facade.LoadExample(item.Id);", source);
        Assert.Contains("_domain.Text = Normalize(example.Domain);", source);
        Assert.Contains("_query.Text = Normalize(string.Join(Environment.NewLine, example.Queries));", source);
        Assert.Contains("if (!_examples.IsVisible) return;", source);
    }

    [Fact]
    public void CrossGui_TextBoxes_Use_Avalonia_ScrollViewer_Attached_Properties()
    {
        var source = MainWindowSource();

        Assert.Contains("ScrollViewer.SetHorizontalScrollBarVisibility", source);
        Assert.Contains("ScrollViewer.SetVerticalScrollBarVisibility", source);
        Assert.DoesNotContain("HorizontalScrollBarVisibility =", source);
        Assert.DoesNotContain("VerticalScrollBarVisibility =", source);
    }

    [Fact]
    public void CrossGui_Solve_Button_Uses_Current_TextBoxes_Through_Facade()
    {
        var source = MainWindowSource();

        Assert.Contains("Ds4Facade.Solve(_domain.Text ?? string.Empty, _query.Text ?? string.Empty)", source);
        Assert.Contains("ODPOWIEDŹ: ", source);
        Assert.Contains("TRACE:", source);
    }

    [Fact]
    public void CrossGui_Exposes_Readable_Dot_Graph_Output()
    {
        var source = MainWindowSource();

        Assert.Contains("Pokaż graf DOT", source);
        Assert.Contains("ShowGraphWindow", source);
        Assert.Contains("_lastGraphDot = result.TraceGraphDot;", source);
    }

    [Fact]
    public void CrossGui_Project_Copies_Examples_To_Output_And_Publish_Folders()
    {
        var csproj = File.ReadAllText(Path.Combine(TestData.RepoRoot(), "src", "Ds4.CrossGui", "Ds4.CrossGui.csproj"));

        Assert.Contains("..\\..\\examples\\**\\*.*", csproj);
        Assert.Contains("CopyToOutputDirectory", csproj);
        Assert.Contains("CopyToPublishDirectory", csproj);
    }
}
