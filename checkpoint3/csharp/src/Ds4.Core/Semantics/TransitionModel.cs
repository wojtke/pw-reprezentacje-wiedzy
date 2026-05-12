using Ds4.Core.Model;
using Ds4.Core.Query;

namespace Ds4.Core.Semantics;

public sealed class TransitionModel
{
    public Domain Domain { get; }
    public IReadOnlyList<State> Sigma { get; }
    public IReadOnlyList<State> Sigma0 { get; }
    private readonly Lazy<List<(State, State, CompositeAction)>> _transitions;
    public IReadOnlyList<(State, State, CompositeAction)> Transitions
    {
        get => _transitions.Value;
    } 

    public TransitionModel(Domain domain, IReadOnlyList<State> sigma, IReadOnlyList<State> sigma0)
    {
        Domain = domain;
        Sigma = sigma;
        Sigma0 = sigma0;

        _transitions = new(() =>
        {
            var simple = new SimpleActionEngine(domain, sigma);
            var conflicts = new ConflictDetector(simple);
            var decompositions = new DecompositionGenerator(simple, conflicts);
            var composite = new CompositeActionEngine(simple, decompositions);

            var transitions = new List<(State, State, CompositeAction)>();
            foreach(var state in Sigma)
            {
                foreach(var action in Domain.CompositeActions)
                {
                    var newStates = composite.Res(action, state);
                    foreach(var newState in newStates)
                    {
                        transitions.Add((state, newState, action));
                    }
                }
            }

            return transitions;
        });
    }
}

public sealed record TransitionResult(IReadOnlyList<State> States, string Reason = "");
