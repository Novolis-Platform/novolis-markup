namespace Novolis.Markup.Mermaid;

/// <summary>Maps <see cref="IMermaidable"/> builders onto <see cref="MermaidDiagramKind"/>.</summary>
public static class MermaidKinds
{
    /// <summary>Returns the catalog kind for a root diagram builder.</summary>
    /// <exception cref="NotSupportedException">The instance is not a known root diagram type.</exception>
    public static MermaidDiagramKind Of(IMermaidable source) =>
        TryOf(source, out var kind)
            ? kind
            : throw new NotSupportedException($"Unknown Mermaid builder '{source.GetType().FullName}'.");

    /// <summary>Tries to map a builder instance to a catalog kind. Nested nodes/links return <see langword="false"/>.</summary>
    public static bool TryOf(IMermaidable source, out MermaidDiagramKind kind)
    {
        ArgumentNullException.ThrowIfNull(source);
        MermaidDiagramKind? mapped = source switch
        {
            RawMermaidDiagram raw => raw.Kind,
            Flowchart => MermaidDiagramKind.Flowchart,
            SequenceDiagram => MermaidDiagramKind.Sequence,
            ClassDiagram => MermaidDiagramKind.Class,
            StateDiagram => MermaidDiagramKind.State,
            ErDiagram => MermaidDiagramKind.Er,
            Journey => MermaidDiagramKind.Journey,
            Gantt => MermaidDiagramKind.Gantt,
            PieChart => MermaidDiagramKind.Pie,
            QuadrantChart => MermaidDiagramKind.Quadrant,
            RequirementDiagram => MermaidDiagramKind.Requirement,
            GitGraph => MermaidDiagramKind.GitGraph,
            Mindmap => MermaidDiagramKind.Mindmap,
            Timeline => MermaidDiagramKind.Timeline,
            Sankey => MermaidDiagramKind.Sankey,
            XyChart => MermaidDiagramKind.XyChart,
            BlockDiagram => MermaidDiagramKind.Block,
            ArchitectureDiagram => MermaidDiagramKind.Architecture,
            C4Diagram => MermaidDiagramKind.C4,
            PacketDiagram => MermaidDiagramKind.Packet,
            RadarChart => MermaidDiagramKind.Radar,
            Treemap => MermaidDiagramKind.Treemap,
            Kanban => MermaidDiagramKind.Kanban,
            VennDiagram => MermaidDiagramKind.Venn,
            TreeView => MermaidDiagramKind.TreeView,
            IshikawaDiagram => MermaidDiagramKind.Ishikawa,
            UseCaseDiagram => MermaidDiagramKind.UseCase,
            WardleyMap => MermaidDiagramKind.Wardley,
            CynefinDiagram => MermaidDiagramKind.Cynefin,
            RailroadDiagram => MermaidDiagramKind.Railroad,
            EventModelingDiagram => MermaidDiagramKind.EventModeling,
            _ => null,
        };

        if (mapped is null)
        {
            kind = default;
            return false;
        }

        kind = mapped.Value;
        return true;
    }

    /// <summary>Default header token emitted for a kind (beta suffixes included where Mermaid still requires them).</summary>
    public static string Header(this MermaidDiagramKind kind) => kind switch
    {
        MermaidDiagramKind.Flowchart => "flowchart",
        MermaidDiagramKind.Sequence => "sequenceDiagram",
        MermaidDiagramKind.Class => "classDiagram",
        MermaidDiagramKind.State => "stateDiagram-v2",
        MermaidDiagramKind.Er => "erDiagram",
        MermaidDiagramKind.Journey => "journey",
        MermaidDiagramKind.Gantt => "gantt",
        MermaidDiagramKind.Pie => "pie",
        MermaidDiagramKind.Quadrant => "quadrantChart",
        MermaidDiagramKind.Requirement => "requirementDiagram",
        MermaidDiagramKind.GitGraph => "gitGraph",
        MermaidDiagramKind.Mindmap => "mindmap",
        MermaidDiagramKind.Timeline => "timeline",
        MermaidDiagramKind.Sankey => "sankey-beta",
        MermaidDiagramKind.XyChart => "xychart",
        MermaidDiagramKind.Block => "block-beta",
        MermaidDiagramKind.Architecture => "architecture-beta",
        MermaidDiagramKind.C4 => "C4Context",
        MermaidDiagramKind.Packet => "packet-beta",
        MermaidDiagramKind.Radar => "radar-beta",
        MermaidDiagramKind.Treemap => "treemap-beta",
        MermaidDiagramKind.Kanban => "kanban",
        MermaidDiagramKind.Venn => "venn-beta",
        MermaidDiagramKind.TreeView => "treeView",
        MermaidDiagramKind.Ishikawa => "ishikawa-beta",
        MermaidDiagramKind.UseCase => "usecase-beta",
        MermaidDiagramKind.Wardley => "wardley-beta",
        MermaidDiagramKind.Cynefin => "cynefin-beta",
        MermaidDiagramKind.Railroad => "railroad-ebnf-beta",
        MermaidDiagramKind.EventModeling => "eventmodeling",
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown Mermaid diagram kind."),
    };
}
