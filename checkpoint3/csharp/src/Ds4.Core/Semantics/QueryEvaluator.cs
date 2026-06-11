using Ds4.Core.Query;

namespace Ds4.Core.Semantics;

public sealed class QueryEvaluationResult
{
    public bool Answer { get; init; }
    public ExecutionTree Tree { get; init; } = null!;
    public string Explanation { get; init; } = "";
}

public sealed class QueryEvaluator
{
    private readonly TransitionModel _model;
    private readonly ProcessEvaluator _processEvaluator;

    public QueryEvaluator(TransitionModel model)
    {
        _model = model;
        var simple = new SimpleActionEngine(model.Domain, model.Sigma);
        var conflicts = new ConflictDetector(simple);
        var decompositions = new DecompositionGenerator(simple, conflicts);
        var composite = new CompositeActionEngine(simple, decompositions);
        _processEvaluator = new ProcessEvaluator(composite);
    }

    public QueryEvaluationResult Evaluate(Ds4Query query)
    {
        var tree = _processEvaluator.BuildTree(_model.Sigma0, query.Process);
        var finals = tree.FinalNodes().ToArray();
        var blocked = tree.BlockedNodes().ToArray();

        bool answer;
        string explanation;

        if (query.Kind == QueryKind.Executable)
        {
            if (query.Quantifier == Quantifier.Possibly)
            {
                answer = _model.Sigma0.Count > 0
                      && tree.Roots.All(root => tree.FinalNodesUnder(root).Any());
                explanation = answer
                    ? "Z każdego stanu początkowego istnieje co najmniej jedna pełna ścieżka wykonania procesu."
                    : "Istnieje stan początkowy, z którego nie da się wykonać procesu do końca.";
            }
            else
            {
                answer = _model.Sigma0.Count > 0 && blocked.Length == 0 && finals.Length > 0;
                explanation = answer
                    ? "Każda ścieżka rozpoczęta w stanie początkowym dochodzi do końca procesu."
                    : "Istnieje stan początkowy lub ścieżka, na której proces blokuje się przed końcem.";
            }
        }
        else
        {
            var goal = query.Goal ?? throw new InvalidOperationException("Goal query without goal formula.");
            if (query.Quantifier == Quantifier.Possibly)
            {
                answer = _model.Sigma0.Count > 0
                      && tree.Roots.All(root => tree.FinalNodesUnder(root).Any(n => goal.Evaluate(n.State)));
                explanation = answer
                    ? "Z każdego stanu początkowego istnieje pełna ścieżka kończąca się stanem spełniającym cel."
                    : "Istnieje stan początkowy, z którego żadna pełna ścieżka nie kończy się stanem spełniającym cel.";
            }
            else
            {
                answer = _model.Sigma0.Count > 0 && blocked.Length == 0 && finals.Length > 0 && finals.All(n => goal.Evaluate(n.State));
                explanation = answer
                    ? "Proces jest koniecznie wykonalny i wszystkie stany końcowe spełniają cel."
                    : "Proces nie jest koniecznie wykonalny albo istnieje stan końcowy niespełniający celu.";
            }
        }

        return new QueryEvaluationResult { Answer = answer, Tree = tree, Explanation = explanation };
    }
}
