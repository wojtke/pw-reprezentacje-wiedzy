using Ds4.Core.Model;
using Ds4.Core.Query;

namespace Ds4.Core.Semantics;

public sealed class ExecutionNode
{
    public State State { get; }
    public int Depth { get; }
    public CompositeAction? IncomingStep { get; }
    public bool Blocked { get; set; }
    public List<ExecutionNode> Children { get; } = new();

    public ExecutionNode(State state, int depth, CompositeAction? incomingStep)
    {
        State = state;
        Depth = depth;
        IncomingStep = incomingStep;
    }
}

public sealed class ExecutionTree
{
    public IReadOnlyList<ExecutionNode> Roots { get; }
    public int ProcessLength { get; }

    public ExecutionTree(IReadOnlyList<ExecutionNode> roots, int processLength)
    {
        Roots = roots;
        ProcessLength = processLength;
    }

    public IEnumerable<ExecutionNode> AllNodes()
    {
        foreach (var root in Roots)
        foreach (var node in Traverse(root))
            yield return node;
    }

    public IEnumerable<ExecutionNode> FinalNodes() => AllNodes().Where(n => n.Depth == ProcessLength);
    public IEnumerable<ExecutionNode> BlockedNodes() => AllNodes().Where(n => n.Blocked && n.Depth < ProcessLength);

    public IEnumerable<ExecutionNode> FinalNodesUnder(ExecutionNode root)
        => Traverse(root).Where(n => n.Depth == ProcessLength);

    public IEnumerable<ExecutionNode> BlockedNodesUnder(ExecutionNode root)
        => Traverse(root).Where(n => n.Blocked && n.Depth < ProcessLength);

    private static IEnumerable<ExecutionNode> Traverse(ExecutionNode node)
    {
        yield return node;
        foreach (var child in node.Children)
        foreach (var sub in Traverse(child))
            yield return sub;
    }
}
