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

        // Zbiór Σ₀ wyznaczają wyłącznie zdania initially. Zdania specyfikacyjne
        // after/observable after są warunkami poprawności modelu (M2), a NIE
        // filtrami na stany początkowe: sprawdzamy je globalnie nad całym Σ₀ i nie
        // usuwamy z niego pojedynczych stanów. Jeżeli warunek jest niespełniony,
        // dziedzina jest sprzeczna (brak modelu), co reprezentujemy pustym Σ₀.
        if (domain.AfterAssertions.Count > 0 && initially.Count > 0)
        {
            var simple = new SimpleActionEngine(domain, sigma);
            var conflicts = new ConflictDetector(simple);
            var decompositions = new DecompositionGenerator(simple, conflicts);
            var composite = new CompositeActionEngine(simple, decompositions);
            var processEvaluator = new ProcessEvaluator(composite);

            foreach (var assertion in domain.AfterAssertions)
            {
                bool valid = assertion.Observable
                    // observable α after P: istnieje stan początkowy i ślad kończący się α
                    ? initially.Any(s => processEvaluator
                        .FinalStates(new[] { s }, assertion.Process)
                        .Any(f => assertion.Goal.Evaluate(f)))
                    // α after P: z każdego stanu początkowego każdy zdefiniowany ślad kończy się α
                    : initially.All(s => processEvaluator
                        .FinalStates(new[] { s }, assertion.Process)
                        .All(f => assertion.Goal.Evaluate(f)));

                if (!valid)
                {
                    initially = new List<State>();
                    break;
                }
            }
        }

        return new TransitionModel(domain, sigma, initially);
    }
}
