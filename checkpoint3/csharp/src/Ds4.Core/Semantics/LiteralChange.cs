namespace Ds4.Core.Semantics;

public sealed record LiteralChange(string Fluent, bool Value)
{
    public override string ToString() => Value ? Fluent : "¬" + Fluent;
}
