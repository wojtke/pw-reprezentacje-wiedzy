namespace Ds4.Tests;

public sealed class StaticProjectTests
{
    [Fact]
    public void Repository_Does_Not_Contain_Em_Dash()
    {
        var root = TestData.RepoRoot();
        var files = Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories)
            .Where(path => !path.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar))
            .Where(path => !path.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar))
            .Where(path => !path.Contains(Path.DirectorySeparatorChar + ".git" + Path.DirectorySeparatorChar))
            .Where(path => Path.GetExtension(path) is ".cs" or ".csproj" or ".sln" or ".md" or ".txt" or ".domain" or ".query" or ".expected" or ".bat")
            .ToArray();

        foreach (var file in files)
        {
            var text = File.ReadAllText(file);
            Assert.DoesNotContain("\u2014", text);
        }
    }

    [Fact]
    public void Gitignore_Excludes_Bin_And_Obj()
    {
        var gitignore = File.ReadAllText(Path.Combine(TestData.RepoRoot(), ".gitignore"));

        Assert.Contains("bin/", gitignore);
        Assert.Contains("obj/", gitignore);
    }

    [Fact]
    public void Solution_Contains_Test_Project()
    {
        var solution = File.ReadAllText(Path.Combine(TestData.RepoRoot(), "Ds4Reasoner.sln"));

        Assert.Contains("Ds4.Tests", solution);
    }

    [Fact]
    public void Solution_Contains_Cross_Platform_Gui_Project()
    {
        var solution = File.ReadAllText(Path.Combine(TestData.RepoRoot(), "Ds4Reasoner.sln"));

        Assert.Contains("Ds4.CrossGui", solution);
    }

    [Fact]
    public void Test_Project_References_Core_Project()
    {
        var csproj = File.ReadAllText(Path.Combine(TestData.RepoRoot(), "tests", "Ds4.Tests", "Ds4.Tests.csproj"));

        Assert.Contains("Ds4.Core.csproj", csproj);
        Assert.Contains("xunit", csproj.ToLowerInvariant());
    }

    [Fact]
    public void Cross_Gui_Project_References_Core_And_Examples()
    {
        var csproj = File.ReadAllText(Path.Combine(TestData.RepoRoot(), "src", "Ds4.CrossGui", "Ds4.CrossGui.csproj"));

        Assert.Contains("Ds4.Core.csproj", csproj);
        Assert.Contains("Avalonia", csproj);
        Assert.Contains("examples", csproj);
    }

    [Fact]
    public void Cross_Platform_Publish_Scripts_Are_Present()
    {
        var root = TestData.RepoRoot();

        Assert.True(File.Exists(Path.Combine(root, "PUBLISH_LINUX_GUI.sh")));
        Assert.True(File.Exists(Path.Combine(root, "PUBLISH_MAC_CROSS_GUI.sh")));
        Assert.True(File.Exists(Path.Combine(root, "PUBLISH_WINDOWS_GUI.bat")));
    }

}
