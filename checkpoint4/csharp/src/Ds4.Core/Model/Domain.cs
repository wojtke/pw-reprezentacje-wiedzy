using Ds4.Core.Formula;
using Ds4.Core.Query;

namespace Ds4.Core.Model;

public sealed class Domain
{
    public HashSet<string> Fluents { get; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> Actions { get; } = new(StringComparer.OrdinalIgnoreCase);

    public List<CauseRule> CauseRules { get; } = new();
    public List<ReleaseRule> ReleaseRules { get; } = new();
    public List<ImpossibleRule> ImpossibleRules { get; } = new();
    public List<AlwaysConstraint> AlwaysConstraints { get; } = new();
    public List<InitiallyConstraint> InitiallyConstraints { get; } = new();
    public HashSet<string> NonInertialFluents { get; } = new(StringComparer.OrdinalIgnoreCase);
    public List<AfterAssertion> AfterAssertions { get; } = new();

    public IReadOnlyCollection<string> InertialFluents
        => Fluents.Where(f => !NonInertialFluents.Contains(f)).ToArray();

    public void AddFluent(string fluent) => Fluents.Add(fluent.Trim());
    public void AddAction(string action) => Actions.Add(action.Trim());

    public void RegisterFormula(IFormula formula)
    {
        foreach (var atom in formula.Atoms()) AddFluent(atom);
    }
}

public sealed record CauseRule(string Action, IFormula Effect, IFormula Condition);
public sealed record ReleaseRule(string Action, string Fluent, IFormula Condition);
public sealed record ImpossibleRule(string Action, IFormula Condition);
public sealed record AlwaysConstraint(IFormula Constraint);
public sealed record InitiallyConstraint(IFormula Constraint);
public sealed record AfterAssertion(IFormula Goal, Ds4Process Process, bool Observable);
