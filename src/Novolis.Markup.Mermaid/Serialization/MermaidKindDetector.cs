namespace Novolis.Markup.Mermaid;

/// <summary>Detects <see cref="MermaidDiagramKind"/> from Mermaid source (header token, ignoring front matter).</summary>
public static class MermaidKindDetector
{
    /// <summary>Returns the diagram kind, or <see langword="null"/> when the header is missing or unknown.</summary>
    public static MermaidDiagramKind? TryDetect(string? source)
    {
        if (string.IsNullOrWhiteSpace(source))
            return null;

        if (!MermaidSourceReader.TryRead(source, out _, out var header, out _))
            return null;

        return FromHeader(header);
    }

    /// <summary>Maps a header token such as <c>flowchart</c> or <c>xychart-beta</c> to a catalog kind.</summary>
    public static MermaidDiagramKind? FromHeader(string? header)
    {
        if (string.IsNullOrWhiteSpace(header))
            return null;

        var token = header.Trim();
        var space = token.IndexOfAny([' ', '\t']);
        if (space > 0)
            token = token[..space];

        return token.ToLowerInvariant() switch
        {
            "flowchart" or "graph" => MermaidDiagramKind.Flowchart,
            "sequencediagram" => MermaidDiagramKind.Sequence,
            "classdiagram" => MermaidDiagramKind.Class,
            "statediagram" or "statediagram-v2" => MermaidDiagramKind.State,
            "erdiagram" => MermaidDiagramKind.Er,
            "journey" => MermaidDiagramKind.Journey,
            "gantt" => MermaidDiagramKind.Gantt,
            "pie" => MermaidDiagramKind.Pie,
            "quadrantchart" => MermaidDiagramKind.Quadrant,
            "requirementdiagram" => MermaidDiagramKind.Requirement,
            "gitgraph" => MermaidDiagramKind.GitGraph,
            "mindmap" => MermaidDiagramKind.Mindmap,
            "timeline" => MermaidDiagramKind.Timeline,
            "sankey" or "sankey-beta" => MermaidDiagramKind.Sankey,
            "xychart" or "xychart-beta" => MermaidDiagramKind.XyChart,
            "block" or "block-beta" => MermaidDiagramKind.Block,
            "architecture" or "architecture-beta" => MermaidDiagramKind.Architecture,
            "c4context" or "c4container" or "c4component" or "c4dynamic" or "c4deployment" => MermaidDiagramKind.C4,
            "packet" or "packet-beta" => MermaidDiagramKind.Packet,
            "radar" or "radar-beta" => MermaidDiagramKind.Radar,
            "treemap" or "treemap-beta" => MermaidDiagramKind.Treemap,
            "kanban" => MermaidDiagramKind.Kanban,
            "venn" or "venn-beta" => MermaidDiagramKind.Venn,
            "treeview" or "tree" => MermaidDiagramKind.TreeView,
            "ishikawa" or "ishikawa-beta" => MermaidDiagramKind.Ishikawa,
            "usecase" or "usecase-beta" => MermaidDiagramKind.UseCase,
            "wardley" or "wardley-beta" => MermaidDiagramKind.Wardley,
            "cynefin" or "cynefin-beta" => MermaidDiagramKind.Cynefin,
            "railroad-beta" or "railroad-ebnf-beta" or "railroad-abnf-beta" or "railroad-peg-beta" => MermaidDiagramKind.Railroad,
            "eventmodeling" => MermaidDiagramKind.EventModeling,
            _ => null,
        };
    }
}
