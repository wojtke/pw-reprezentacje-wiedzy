namespace Ds4.Core.Model;

public sealed record Fluent(string Name)
{
    public override string ToString() => Name;
}

public sealed record ActionSymbol(string Name)
{
    public override string ToString() => Name;
}
