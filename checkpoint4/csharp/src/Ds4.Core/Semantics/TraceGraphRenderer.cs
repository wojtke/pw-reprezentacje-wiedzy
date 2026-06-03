using System.Text;
using Ds4.Core.Model;
using Ds4.Core.Query;

namespace Ds4.Core.Semantics;

public static class TraceGraphRenderer
{
    public static string RenderDot(ExecutionTree tree, Domain domain, int maxNodes = 250)
    {
        var sb = new StringBuilder();
        sb.AppendLine("digraph ExecutionTrace {");
        sb.AppendLine("  graph [rankdir=TB, splines=ortho, nodesep=1.35, ranksep=1.20, pad=0.55, margin=0.30, concentrate=false];");
        sb.AppendLine("  node [shape=box, style=\"rounded,filled\", fillcolor=\"#f8fafc\", color=\"#64748b\", fontname=\"Consolas\", fontsize=13, margin=0.20];");
        sb.AppendLine("  edge [color=\"#334155\", fontname=\"Segoe UI\", fontsize=12, arrowsize=0.80, penwidth=1.15];");
        sb.AppendLine("  labelloc=\"t\";");
        sb.AppendLine("  label=\"DS4 execution trace\";");

        var ids = new Dictionary<ExecutionNode, string>();
        var counter = 0;
        var truncated = false;

        foreach (var root in tree.Roots)
            RenderNode(root, parent: null, incoming: null);

        if (truncated)
        {
            sb.AppendLine("  truncated [label=\"graph truncated\\nuse textual trace for complete output\", shape=note, fillcolor=\"#fff7ed\", color=\"#ea580c\"]; ");
        }

        sb.AppendLine("}");
        return sb.ToString();

        void RenderNode(ExecutionNode node, string? parent, CompositeAction? incoming)
        {
            if (counter >= maxNodes)
            {
                truncated = true;
                return;
            }

            if (!ids.TryGetValue(node, out var id))
            {
                id = "n" + counter++;
                ids[node] = id;
                var state = Escape(WrapState(node.State.ToPrettyString(domain.Fluents)));
                var role = node.Blocked ? "blocked" : node.Depth == 0 ? "initial" : node.Depth == tree.ProcessLength ? "final" : "state";
                var fill = node.Blocked ? "#fee2e2" : node.Depth == 0 ? "#e0f2fe" : node.Depth == tree.ProcessLength ? "#dcfce7" : "#f8fafc";
                var border = node.Blocked ? "#dc2626" : node.Depth == 0 ? "#0284c7" : node.Depth == tree.ProcessLength ? "#16a34a" : "#64748b";
                sb.AppendLine($"  {id} [label=\"step {node.Depth}\\n{role}\\n{state}\", fillcolor=\"{fill}\", color=\"{border}\"]; ");
            }

            if (parent is not null)
            {
                var label = Escape(incoming?.ToString() ?? "step");
                sb.AppendLine($"  {parent} -> {id} [label=\"{label}\"]; ");
            }

            foreach (var child in node.Children)
                RenderNode(child, id, child.IncomingStep);
        }
    }

    private static string WrapState(string state)
    {
        if (state.Length <= 42) return state;
        var inner = state.Trim('{', '}');
        var parts = inner.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length <= 3) return state;

        var lines = new List<string>();
        for (var i = 0; i < parts.Length; i += 3)
            lines.Add(string.Join(", ", parts.Skip(i).Take(3)));
        return "{" + string.Join("\\n ", lines) + "}";
    }

    private static string Escape(string text)
        => text.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "\\n");
}
