using Ds4.Core.Model;

namespace Ds4.Core.Formula;

public sealed record TrueFormula : IFormula
{
    public bool Evaluate(State state) => true;
    public IEnumerable<string> Atoms() { yield break; }
    public IReadOnlyList<DnfTerm> ToDnf() => new[] { DnfTerm.Empty() };
    public override string ToString() => "true";
}

public sealed record FalseFormula : IFormula
{
    public bool Evaluate(State state) => false;
    public IEnumerable<string> Atoms() { yield break; }
    public IReadOnlyList<DnfTerm> ToDnf() => Array.Empty<DnfTerm>();
    public override string ToString() => "false";
}

public sealed record Atom(string Name) : IFormula
{
    public bool Evaluate(State state) => state.IsTrue(Name);
    public IEnumerable<string> Atoms() { yield return Name; }
    public IReadOnlyList<DnfTerm> ToDnf() => new[] { DnfTerm.Empty().Add(new Literal(Name, true)) };
    public override string ToString() => Name;
}

public sealed record Not(IFormula Inner) : IFormula
{
    public bool Evaluate(State state) => !Inner.Evaluate(state);
    public IEnumerable<string> Atoms() => Inner.Atoms();
    public IReadOnlyList<DnfTerm> ToDnf() => FormulaTools.ToNegationNormalForm(this).ToDnf();
    public override string ToString() => "¬(" + Inner + ")";
}

public sealed record And(IFormula Left, IFormula Right) : IFormula
{
    public bool Evaluate(State state) => Left.Evaluate(state) && Right.Evaluate(state);
    public IEnumerable<string> Atoms() => Left.Atoms().Concat(Right.Atoms()).Distinct(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<DnfTerm> ToDnf()
    {
        var result = new List<DnfTerm>();
        foreach (var l in Left.ToDnf())
        foreach (var r in Right.ToDnf())
        {
            var merged = FormulaTools.TryMerge(l, r);
            if (merged is not null) result.Add(merged);
        }
        return result;
    }
    public override string ToString() => "(" + Left + " ∧ " + Right + ")";
}

public sealed record Or(IFormula Left, IFormula Right) : IFormula
{
    public bool Evaluate(State state) => Left.Evaluate(state) || Right.Evaluate(state);
    public IEnumerable<string> Atoms() => Left.Atoms().Concat(Right.Atoms()).Distinct(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<DnfTerm> ToDnf() => Left.ToDnf().Concat(Right.ToDnf()).ToArray();
    public override string ToString() => "(" + Left + " ∨ " + Right + ")";
}

public sealed record Implies(IFormula Left, IFormula Right) : IFormula
{
    public bool Evaluate(State state) => !Left.Evaluate(state) || Right.Evaluate(state);
    public IEnumerable<string> Atoms() => Left.Atoms().Concat(Right.Atoms()).Distinct(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<DnfTerm> ToDnf() => new Or(new Not(Left), Right).ToDnf();
    public override string ToString() => "(" + Left + " → " + Right + ")";
}

public sealed record Iff(IFormula Left, IFormula Right) : IFormula
{
    public bool Evaluate(State state) => Left.Evaluate(state) == Right.Evaluate(state);
    public IEnumerable<string> Atoms() => Left.Atoms().Concat(Right.Atoms()).Distinct(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<DnfTerm> ToDnf()
        => new And(new Implies(Left, Right), new Implies(Right, Left)).ToDnf();
    public override string ToString() => "(" + Left + " ↔ " + Right + ")";
}
