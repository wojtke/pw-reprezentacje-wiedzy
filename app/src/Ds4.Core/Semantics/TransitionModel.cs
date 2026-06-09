using Ds4.Core.Model;

namespace Ds4.Core.Semantics;

public sealed class TransitionModel
{
    public Domain Domain { get; }
    public IReadOnlyList<State> Sigma { get; }
    public IReadOnlyList<State> Sigma0 { get; }

    public TransitionModel(Domain domain, IReadOnlyList<State> sigma, IReadOnlyList<State> sigma0)
    {
        Domain = domain;
        Sigma = sigma;
        Sigma0 = sigma0;
    }
}

public sealed record TransitionResult(IReadOnlyList<State> States, string Reason = "");
