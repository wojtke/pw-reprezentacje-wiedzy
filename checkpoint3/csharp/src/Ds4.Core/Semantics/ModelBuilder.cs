using Ds4.Core.Model;

namespace Ds4.Core.Semantics;

public static class ModelBuilder
{
    public static TransitionModel Build(Domain domain)
    {
        var allStates = StateGenerator.GenerateAll(domain.Fluents);
        var sigma = allStates
            .Where(s => domain.AlwaysConstraints.All(c => c.Constraint.Evaluate(s)))
            .ToArray();

        var initially = sigma
            .Where(s => domain.InitiallyConstraints.All(c => c.Constraint.Evaluate(s)))
            .ToList();

        // Specyfikacyjne zdania after/observable after traktujemy jako dodatkowe
        // ograniczenia na możliwe stany początkowe. To jest praktyczna wersja
        // zgodna z dokumentem: after = własność konieczna, observable = możliwa.
        if (domain.AfterAssertions.Count > 0)
        {
            var simple = new SimpleActionEngine(domain, sigma);
            var conflicts = new ConflictDetector(simple);
            var decompositions = new DecompositionGenerator(simple, conflicts);
            var composite = new CompositeActionEngine(simple, decompositions);
            var processEvaluator = new ProcessEvaluator(composite);

            initially = initially.Where(s =>
            {
                foreach (var assertion in domain.AfterAssertions)
                {
                    var finals = processEvaluator.FinalStates(new[] { s }, assertion.Process).ToArray();
                    if (assertion.Observable)
                    {
                        if (!finals.Any(f => assertion.Goal.Evaluate(f))) return false;
                    }
                    else
                    {
                        if (finals.Length == 0 || finals.Any(f => !assertion.Goal.Evaluate(f))) return false;
                    }
                }
                return true;
            }).ToList();
        }

        return new TransitionModel(domain, sigma, initially);
    }
}
