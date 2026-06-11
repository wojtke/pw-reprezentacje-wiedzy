using Ds4.Core.Api;

static string ReadFileOrStdin(string? path, string prompt)
{
    if (!string.IsNullOrWhiteSpace(path)) return File.ReadAllText(path);
    Console.Error.WriteLine(prompt);
    Console.Error.WriteLine("Zakończ wejście linią zawierającą tylko: ---");
    var lines = new List<string>();
    while (true)
    {
        var line = Console.ReadLine();
        if (line is null || line.Trim() == "---") break;
        lines.Add(line);
    }
    return string.Join(Environment.NewLine, lines);
}

if (args.Length == 1 && args[0] == "--examples")
{
    foreach (var ex in Ds4Facade.ListExamples())
        Console.WriteLine($"{ex.Id}: {ex.Name} - {ex.Description}");
    return;
}

if (args.Length == 2 && args[0] == "--example")
{
    var ex = Ds4Facade.LoadExample(args[1]);
    Console.WriteLine($"# {ex.Name}");
    Console.WriteLine(ex.Description);
    Console.WriteLine("\nDOMAIN:\n" + ex.Domain);
    Console.WriteLine("\nQUERIES:");
    foreach (var q in ex.Queries) Console.WriteLine("  " + q);
    return;
}

string domainText;
string queryText;

if (args.Length >= 2)
{
    domainText = File.ReadAllText(args[0]);
    queryText = File.ReadAllText(args[1]);
}
else
{
    domainText = ReadFileOrStdin(null, "Wklej dziedzinę:");
    queryText = ReadFileOrStdin(null, "Wklej kwerendę:");
}

var result = Ds4Facade.Solve(domainText, queryText);
if (!result.Ok)
{
    Console.WriteLine("BŁĄD: " + result.Error);
    return;
}

Console.WriteLine(result.Answer == true ? "TAK" : "NIE");
Console.WriteLine(result.Explanation);
Console.WriteLine($"|Σ| = {result.SigmaCount}, |Σ₀| = {result.Sigma0Count}");
Console.WriteLine("\nTRACE:");
Console.WriteLine(result.Trace);
