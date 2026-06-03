using Ds4.Core.Model;
using Ds4.Core.Query;

namespace Ds4.Core.Semantics;

public sealed class ProcessEvaluator
{
    private readonly CompositeActionEngine _composite;

    public ProcessEvaluator(CompositeActionEngine composite)
    {
        _composite = composite;
    }

    public ExecutionTree BuildTree(IEnumerable<State> initialStates, Ds4Process process)
    {
        var roots = initialStates.Select(s => new ExecutionNode(s, 0, null)).ToList();
        foreach (var root in roots)
            Expand(root, process);
        return new ExecutionTree(roots, process.Steps.Count);
    }

    public IReadOnlyList<State> FinalStates(IEnumerable<State> initialStates, Ds4Process process)
        => BuildTree(initialStates, process).FinalNodes().Select(n => n.State).Distinct().ToArray();

    private void Expand(ExecutionNode node, Ds4Process process)
    {
        if (node.Depth >= process.Steps.Count) return;

        var step = process.Steps[node.Depth];
        var nextStates = _composite.Res(step, node.State);
        if (nextStates.Count == 0)
        {
            node.Blocked = true;
            return;
        }

        foreach (var next in nextStates)
        {
            var child = new ExecutionNode(next, node.Depth + 1, step);
            node.Children.Add(child);
            Expand(child, process);
        }
    }
}
