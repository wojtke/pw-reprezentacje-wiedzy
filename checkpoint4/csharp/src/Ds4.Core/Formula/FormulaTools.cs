namespace Ds4.Core.Formula;

public static class FormulaTools
{
    public static IFormula True { get; } = new TrueFormula();
    public static IFormula False { get; } = new FalseFormula();

    public static IFormula ToNegationNormalForm(IFormula formula) => formula switch
    {
        TrueFormula => formula,
        FalseFormula => formula,
        Atom => formula,
        And a => new And(ToNegationNormalForm(a.Left), ToNegationNormalForm(a.Right)),
        Or o => new Or(ToNegationNormalForm(o.Left), ToNegationNormalForm(o.Right)),
        Implies i => ToNegationNormalForm(new Or(new Not(i.Left), i.Right)),
        Iff iff => ToNegationNormalForm(new And(new Implies(iff.Left, iff.Right), new Implies(iff.Right, iff.Left))),
        Not { Inner: TrueFormula } => False,
        Not { Inner: FalseFormula } => True,
        Not { Inner: Atom a } => new NegativeAtom(a.Name),
        Not { Inner: NegativeAtom na } => new Atom(na.Name),
        Not { Inner: Not n } => ToNegationNormalForm(n.Inner),
        Not { Inner: And a } => ToNegationNormalForm(new Or(new Not(a.Left), new Not(a.Right))),
        Not { Inner: Or o } => ToNegationNormalForm(new And(new Not(o.Left), new Not(o.Right))),
        Not { Inner: Implies i } => ToNegationNormalForm(new And(i.Left, new Not(i.Right))),
        Not { Inner: Iff iff } => ToNegationNormalForm(new Or(new And(iff.Left, new Not(iff.Right)), new And(new Not(iff.Left), iff.Right))),
        _ => formula
    };

    public static DnfTerm? TryMerge(DnfTerm left, DnfTerm right)
    {
        if (left.HasComplementWith(right)) return null;
        var term = DnfTerm.Empty();
        foreach (var kv in left.Literals)
            term = term.Add(new Literal(kv.Key, kv.Value));
        foreach (var kv in right.Literals)
            term = term.Add(new Literal(kv.Key, kv.Value));
        return term;
    }
}

public sealed record NegativeAtom(string Name) : IFormula
{
    public bool Evaluate(Ds4.Core.Model.State state) => !state.IsTrue(Name);
    public IEnumerable<string> Atoms() { yield return Name; }
    public IReadOnlyList<DnfTerm> ToDnf() => new[] { DnfTerm.Empty().Add(new Literal(Name, false)) };
    public override string ToString() => "not " + Name;
}
