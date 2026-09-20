using System.Globalization;
using System.Text.RegularExpressions;

namespace Novolis.Markup.Mermaid;

/// <summary>Reconstructs typed builders from Mermaid source. Falls back to <see langword="null"/> on failure.</summary>
static class MermaidTypedParser
{
    public static IMermaidable? Parse(MermaidDiagramKind kind, string header, IReadOnlyList<string> body)
    {
        try
        {
            return kind switch
            {
                MermaidDiagramKind.Flowchart => ParseFlowchart(header, body),
                MermaidDiagramKind.Sequence => ParseSequence(body),
                MermaidDiagramKind.Class => ParseClass(body),
                MermaidDiagramKind.State => ParseState(body),
                MermaidDiagramKind.Er => ParseEr(body),
                MermaidDiagramKind.Journey => ParseJourney(body),
                MermaidDiagramKind.Gantt => ParseGantt(body),
                MermaidDiagramKind.Pie => ParsePie(header, body),
                MermaidDiagramKind.Quadrant => ParseQuadrant(body),
                MermaidDiagramKind.Requirement => ParseRequirement(body),
                MermaidDiagramKind.GitGraph => ParseGitGraph(body),
                MermaidDiagramKind.Mindmap => ParseMindmap(body),
                MermaidDiagramKind.Timeline => ParseTimeline(body),
                MermaidDiagramKind.Sankey => ParseSankey(body),
                MermaidDiagramKind.XyChart => ParseXy(header, body),
                MermaidDiagramKind.Block => ParseBlock(body),
                MermaidDiagramKind.Architecture => ParseArchitecture(body),
                MermaidDiagramKind.C4 => ParseC4(header, body),
                MermaidDiagramKind.Packet => ParsePacket(body),
                MermaidDiagramKind.Radar => ParseRadar(body),
                MermaidDiagramKind.Treemap => ParseTreemap(body),
                MermaidDiagramKind.Kanban => ParseKanban(body),
                MermaidDiagramKind.Venn => ParseVenn(body),
                MermaidDiagramKind.TreeView => ParseTreeView(body),
                MermaidDiagramKind.Ishikawa => ParseIshikawa(body),
                MermaidDiagramKind.UseCase => ParseUseCase(body),
                MermaidDiagramKind.Wardley => ParseWardley(body),
                MermaidDiagramKind.Cynefin => ParseCynefin(body),
                MermaidDiagramKind.Railroad => ParseRailroad(header, body),
                MermaidDiagramKind.EventModeling => ParseEventModeling(body),
                _ => null,
            };
        }
        catch
        {
            return null;
        }
    }

    static Flowchart ParseFlowchart(string header, IReadOnlyList<string> body)
    {
        var dir = Direction.TopToBottom;
        var parts = header.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 1)
            dir = ParseDirection(parts[1]);
        var chart = new Flowchart(dir);
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            if (line.StartsWith("subgraph ", StringComparison.Ordinal))
            {
                var label = line["subgraph ".Length..].Trim();
                chart.AddSubgraph(new Subgraph(label, Direction.TopToBottom));
                continue;
            }

            if (TryParseNamedShape(line, out var named))
            {
                chart.AddNode(named);
                continue;
            }

            if (TryParseNode(line, out var node))
            {
                chart.AddNode(node);
                continue;
            }

            if (TryParseLink(line, out var link))
                chart.AddLink(link);
        }

        return chart;
    }

    static SequenceDiagram ParseSequence(IReadOnlyList<string> body)
    {
        var diagram = new SequenceDiagram();
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            if (line.StartsWith("participant ", StringComparison.Ordinal) || line.StartsWith("actor ", StringComparison.Ordinal))
            {
                var asActor = line.StartsWith("actor ", StringComparison.Ordinal);
                var rest = line[(asActor ? 6 : 12)..].Trim();
                var aliasIdx = rest.IndexOf(" as ", StringComparison.Ordinal);
                if (aliasIdx > 0)
                    diagram.AddParticipant(rest[..aliasIdx].Trim(), rest[(aliasIdx + 4)..].Trim(), asActor);
                else
                    diagram.AddParticipant(rest, asActor: asActor);
                continue;
            }

            diagram.AddStatement(line);
        }

        return diagram;
    }

    static ClassDiagram ParseClass(IReadOnlyList<string> body)
    {
        var diagram = new ClassDiagram();
        for (var i = 0; i < body.Count; i++)
        {
            var line = MermaidSourceReader.Meaningful(body[i]);
            if (line is null)
                continue;
            if (line.StartsWith("direction ", StringComparison.Ordinal))
            {
                diagram.Direction(ParseDirection(line["direction ".Length..].Trim()));
                continue;
            }

            if (line.StartsWith("class ", StringComparison.Ordinal) && line.Contains('{', StringComparison.Ordinal))
            {
                var name = line["class ".Length..].Trim().TrimEnd('{').Trim();
                var node = new ClassNode(name);
                i++;
                while (i < body.Count)
                {
                    var inner = body[i].Trim();
                    i++;
                    if (inner == "}")
                        break;
                    if (inner.StartsWith("<<", StringComparison.Ordinal) && inner.EndsWith(">>", StringComparison.Ordinal))
                        node.WithStereotype(inner.Trim('<', '>'));
                    else if (!string.IsNullOrWhiteSpace(inner) && !MermaidSourceReader.IsComment(inner))
                        node.AddMember(inner);
                }

                i--;
                diagram.AddClass(node);
                continue;
            }

            if (line.StartsWith("class ", StringComparison.Ordinal))
            {
                diagram.AddClass(new ClassNode(line["class ".Length..].Trim()));
                continue;
            }

            if (TryParseClassRelation(line, out var rel))
            {
                diagram.AddRelation(rel);
                continue;
            }

            diagram.AddStatement(line);
        }

        return diagram;
    }

    static StateDiagram ParseState(IReadOnlyList<string> body)
    {
        var diagram = new StateDiagram();
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            diagram.AddStatement(line);
        }

        return diagram;
    }

    static ErDiagram ParseEr(IReadOnlyList<string> body)
    {
        var diagram = new ErDiagram();
        for (var i = 0; i < body.Count; i++)
        {
            var line = MermaidSourceReader.Meaningful(body[i]);
            if (line is null)
                continue;
            if (line.Contains('{', StringComparison.Ordinal) && !line.Contains("--", StringComparison.Ordinal) && !line.Contains("..", StringComparison.Ordinal))
            {
                var name = line.Trim().TrimEnd('{').Trim();
                var entity = new ErEntity(name);
                i++;
                while (i < body.Count)
                {
                    var inner = body[i].Trim();
                    i++;
                    if (inner == "}")
                        break;
                    if (!string.IsNullOrWhiteSpace(inner) && !MermaidSourceReader.IsComment(inner))
                        entity.AddAttribute(inner);
                }

                i--;
                diagram.AddEntity(entity);
                continue;
            }

            var relMatch = Regex.Match(line, @"^(\S+)\s+(\S+?)(--|\.\.)(\S+)\s+(\S+)\s*:\s*(.+)$");
            if (relMatch.Success)
            {
                diagram.AddRelationship(new ErRelationship(
                    relMatch.Groups[1].Value,
                    relMatch.Groups[2].Value,
                    relMatch.Groups[4].Value,
                    relMatch.Groups[5].Value,
                    relMatch.Groups[6].Value.Trim(),
                    identifying: relMatch.Groups[3].Value == "--"));
                continue;
            }

            if (!line.Contains("--", StringComparison.Ordinal) && !line.Contains("..", StringComparison.Ordinal))
                diagram.AddEntity(new ErEntity(line));
        }

        return diagram;
    }

    static Journey ParseJourney(IReadOnlyList<string> body)
    {
        var title = "Journey";
        JourneySection? current = null;
        var journey = new Journey(title);
        var built = false;
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            if (line.StartsWith("title ", StringComparison.Ordinal))
            {
                title = line["title ".Length..].Trim();
                journey = new Journey(title);
                built = true;
                continue;
            }

            if (line.StartsWith("section ", StringComparison.Ordinal))
            {
                current = new JourneySection(line["section ".Length..].Trim());
                journey.AddSection(current);
                continue;
            }

            var colon = line.LastIndexOf(':');
            if (colon > 0 && current is not null)
            {
                var label = line[..colon].Trim();
                var rest = line[(colon + 1)..].Split(':', 2);
                if (rest.Length == 2 && int.TryParse(rest[0].Trim(), out var score))
                    current.AddTask(label, score, rest[1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
            }
        }

        return built || journey is not null ? journey : new Journey(title);
    }

    static Gantt ParseGantt(IReadOnlyList<string> body)
    {
        var title = "Gantt";
        var gantt = new Gantt(title);
        GanttSection? section = null;
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            if (line.StartsWith("title ", StringComparison.Ordinal))
            {
                title = line["title ".Length..].Trim();
                gantt = new Gantt(title);
                continue;
            }

            if (line.StartsWith("dateFormat ", StringComparison.Ordinal))
            {
                gantt.WithDateFormat(line["dateFormat ".Length..].Trim());
                continue;
            }

            if (line.StartsWith("excludes ", StringComparison.Ordinal))
            {
                gantt.Excludes(line["excludes ".Length..].Trim());
                continue;
            }

            if (line.StartsWith("axisFormat ", StringComparison.Ordinal))
            {
                gantt.AxisFormat(line["axisFormat ".Length..].Trim());
                continue;
            }

            if (line.StartsWith("tickInterval ", StringComparison.Ordinal))
            {
                gantt.TickInterval(line["tickInterval ".Length..].Trim());
                continue;
            }

            if (line.StartsWith("todayMarker ", StringComparison.Ordinal))
            {
                gantt.TodayMarker(line["todayMarker ".Length..].Trim());
                continue;
            }

            if (line.StartsWith("section ", StringComparison.Ordinal))
            {
                section = new GanttSection(line["section ".Length..].Trim());
                gantt.AddSection(section);
                continue;
            }

            var split = line.Split(':', 2);
            if (split.Length == 2 && section is not null)
            {
                var fields = split[1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (fields.Length >= 3)
                    section.AddTask(split[0].Trim(), fields[^3], fields[^2], fields[^1], fields.Length > 3 ? fields[..^3] : []);
            }
        }

        return gantt;
    }

    static PieChart ParsePie(string header, IReadOnlyList<string> body)
    {
        var show = header.Contains("showData", StringComparison.OrdinalIgnoreCase);
        var title = "Pie";
        var values = new List<ChartValue>();
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            if (line.Equals("showData", StringComparison.OrdinalIgnoreCase))
            {
                show = true;
                continue;
            }

            if (line.StartsWith("title ", StringComparison.Ordinal))
            {
                title = line["title ".Length..].Trim();
                continue;
            }

            var parts = line.Split(':', 2);
            if (parts.Length == 2 && double.TryParse(Unquote(parts[0].Trim()), NumberStyles.Float, CultureInfo.InvariantCulture, out var n)
                is false && double.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out n))
            {
                values.Add(new ChartValue(Unquote(parts[0].Trim()), n));
            }
        }

        var pie = new PieChart(title, show);
        pie.AddValues(values);
        return pie;
    }

    static QuadrantChart ParseQuadrant(IReadOnlyList<string> body)
    {
        var title = "Quadrant";
        string? x = null, y = null, q1 = null, q2 = null, q3 = null, q4 = null;
        var points = new List<(string Name, double X, double Y)>();
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            if (line.StartsWith("title ", StringComparison.Ordinal))
                title = line["title ".Length..].Trim();
            else if (line.StartsWith("x-axis ", StringComparison.Ordinal))
                x = line["x-axis ".Length..].Trim();
            else if (line.StartsWith("y-axis ", StringComparison.Ordinal))
                y = line["y-axis ".Length..].Trim();
            else if (line.StartsWith("quadrant-1 ", StringComparison.Ordinal))
                q1 = line["quadrant-1 ".Length..].Trim();
            else if (line.StartsWith("quadrant-2 ", StringComparison.Ordinal))
                q2 = line["quadrant-2 ".Length..].Trim();
            else if (line.StartsWith("quadrant-3 ", StringComparison.Ordinal))
                q3 = line["quadrant-3 ".Length..].Trim();
            else if (line.StartsWith("quadrant-4 ", StringComparison.Ordinal))
                q4 = line["quadrant-4 ".Length..].Trim();
            else
            {
                var idx = line.IndexOf(':');
                if (idx > 0)
                {
                    var name = line[..idx].Trim();
                    var nums = Regex.Match(line, @"\[\s*([-\d.]+)\s*,\s*([-\d.]+)\s*\]");
                    if (nums.Success)
                        points.Add((name, ParseDouble(nums.Groups[1].Value), ParseDouble(nums.Groups[2].Value)));
                }
            }
        }

        var chart = new QuadrantChart(title);
        if (x is not null && y is not null)
            chart.WithAxes(x, y);
        if (q1 is not null && q2 is not null && q3 is not null && q4 is not null)
            chart.WithQuadrants(q1, q2, q3, q4);
        foreach (var p in points)
            chart.AddPoint(p.Name, p.X, p.Y);
        return chart;
    }

    static RequirementDiagram ParseRequirement(IReadOnlyList<string> body)
    {
        var diagram = new RequirementDiagram();
        for (var i = 0; i < body.Count; i++)
        {
            var line = MermaidSourceReader.Meaningful(body[i]);
            if (line is null)
                continue;
            if (line.StartsWith("requirement ", StringComparison.Ordinal) && line.Contains('{', StringComparison.Ordinal))
            {
                var name = line["requirement ".Length..].Trim().TrimEnd('{').Trim();
                string id = name, text = name, risk = "", verify = "";
                i++;
                while (i < body.Count)
                {
                    var inner = body[i].Trim();
                    i++;
                    if (inner == "}")
                        break;
                    if (inner.StartsWith("id:", StringComparison.Ordinal))
                        id = inner[3..].Trim();
                    else if (inner.StartsWith("text:", StringComparison.Ordinal))
                        text = inner[5..].Trim();
                    else if (inner.StartsWith("risk:", StringComparison.Ordinal))
                        risk = inner[5..].Trim();
                    else if (inner.StartsWith("verifymethod:", StringComparison.Ordinal))
                        verify = inner["verifymethod:".Length..].Trim();
                }

                i--;
                diagram.AddRequirement(new RequirementNode(name, id, text, EmptyToNull(risk), EmptyToNull(verify)));
                continue;
            }

            if (line.StartsWith("element ", StringComparison.Ordinal))
            {
                var id = line["element ".Length..].Trim().TrimEnd('{').Trim();
                var type = "element";
                i++;
                while (i < body.Count)
                {
                    var inner = body[i].Trim();
                    i++;
                    if (inner == "}")
                        break;
                    if (inner.StartsWith("type:", StringComparison.Ordinal))
                        type = inner[5..].Trim();
                }

                i--;
                diagram.AddElement(new RequirementElement(id, type));
                continue;
            }

            var rel = Regex.Match(line, @"^(\S+)\s+-\s+(\w+)\s+->\s+(\S+)$");
            if (rel.Success && Enum.TryParse<RequirementRelationType>(rel.Groups[2].Value, ignoreCase: true, out var kind))
                diagram.AddRelation(new RequirementRelation(rel.Groups[1].Value, kind, rel.Groups[3].Value));
        }

        return diagram;
    }

    static GitGraph ParseGitGraph(IReadOnlyList<string> body)
    {
        var graph = new GitGraph();
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            graph.AddStatement(line);
        }

        return graph;
    }

    static Mindmap ParseMindmap(IReadOnlyList<string> body)
    {
        var rows = IndentedRows(body);
        if (rows.Count == 0)
            return new Mindmap("Root");
        var map = new Mindmap(UnwrapMindmap(rows[0].Text));
        var stack = new List<(int Indent, MindmapNode Node)> { (rows[0].Indent, map.Root) };
        for (var i = 1; i < rows.Count; i++)
        {
            var (indent, text) = rows[i];
            while (stack.Count > 1 && indent <= stack[^1].Indent)
                stack.RemoveAt(stack.Count - 1);
            var child = stack[^1].Node.AddChild(UnwrapMindmap(text));
            stack.Add((indent, child));
        }

        return map;
    }

    static Timeline ParseTimeline(IReadOnlyList<string> body)
    {
        var title = "Timeline";
        var timeline = new Timeline(title);
        Section? section = null;
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            if (line.StartsWith("title ", StringComparison.Ordinal))
            {
                title = line["title ".Length..].Trim();
                timeline = new Timeline(title);
                continue;
            }

            if (line.StartsWith("section ", StringComparison.Ordinal))
            {
                section = new Section(line["section ".Length..].Trim());
                timeline.AddSection(section);
                continue;
            }

            var colon = line.IndexOf(':');
            if (colon > 0)
            {
                var left = line[..colon].Trim();
                var right = line[(colon + 1)..].Trim();
                var ev = DateTime.TryParse(left, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dt)
                    ? new Event(right, dt)
                    : new Event(right, DateTime.UnixEpoch);
                if (section is not null)
                    section.AddEvent(ev);
                else
                    timeline.AddEvent(ev);
            }
        }

        return timeline;
    }

    static Sankey ParseSankey(IReadOnlyList<string> body)
    {
        var sankey = new Sankey();
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            var parts = line.Split(',');
            if (parts.Length >= 3 && double.TryParse(parts[^1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                sankey.AddLink(parts[0].Trim(), string.Join(',', parts[1..^1]).Trim(), value);
        }

        return sankey;
    }

    static XyChart ParseXy(string header, IReadOnlyList<string> body)
    {
        string? title = null;
        var chart = new XyChart();
        var horizontal = header.Contains("horizontal", StringComparison.OrdinalIgnoreCase);
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            if (line.StartsWith("title ", StringComparison.Ordinal))
            {
                title = Unquote(line["title ".Length..].Trim());
                continue;
            }

            if (line.StartsWith("x-axis ", StringComparison.Ordinal))
            {
                ParseXyAxis(line["x-axis ".Length..].Trim(), isX: true, out var axisTitle, out var cats, out var range);
                chart = new XyChart(title);
                if (horizontal)
                    chart.Horizontal();
                if (cats.Length > 0)
                    chart.WithXAxis(axisTitle, cats);
                else if (range is not null)
                {
                    var bounds = range.Split("-->", StringSplitOptions.TrimEntries);
                    if (bounds.Length == 2)
                        chart.WithXAxisRange(axisTitle, ParseDouble(bounds[0]), ParseDouble(bounds[1]));
                }
                else if (axisTitle is not null)
                    chart.WithXAxis(axisTitle);
                continue;
            }

            if (line.StartsWith("y-axis ", StringComparison.Ordinal))
            {
                ParseXyAxis(line["y-axis ".Length..].Trim(), isX: false, out var axisTitle, out _, out var range);
                double? min = null, max = null;
                if (range is not null)
                {
                    var bounds = range.Split("-->", StringSplitOptions.TrimEntries);
                    if (bounds.Length == 2)
                    {
                        min = ParseDouble(bounds[0]);
                        max = ParseDouble(bounds[1]);
                    }
                }

                if (chart.Title is null && title is not null)
                {
                    chart = new XyChart(title);
                    if (horizontal)
                        chart.Horizontal();
                }

                chart.WithYAxis(axisTitle, min, max);
                continue;
            }

            if (line.StartsWith("bar ", StringComparison.Ordinal) || line.StartsWith("line ", StringComparison.Ordinal))
            {
                var isBar = line.StartsWith("bar ", StringComparison.Ordinal);
                var rest = line[(isBar ? 4 : 5)..].Trim();
                string? name = null;
                if (rest.StartsWith('"'))
                {
                    var end = rest.IndexOf('"', 1);
                    if (end > 0)
                    {
                        name = rest[1..end];
                        rest = rest[(end + 1)..].Trim();
                    }
                }

                var nums = Regex.Matches(rest, @"-?\d+(?:\.\d+)?");
                var values = nums.Select(m => ParseDouble(m.Value)).ToArray();
                if (isBar)
                    chart.Bar(name, values);
                else
                    chart.Line(name, values);
            }
        }

        if (chart.Title is null && title is not null)
        {
            var named = new XyChart(title);
            if (horizontal)
                named.Horizontal();
            foreach (var s in chart.Series)
                named.AddSeries(s);
            return named;
        }

        return chart;
    }

    static BlockDiagram ParseBlock(IReadOnlyList<string> body)
    {
        var columns = 1;
        var block = new BlockDiagram();
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            if (line.StartsWith("columns ", StringComparison.Ordinal) && int.TryParse(line["columns ".Length..].Trim(), out var n))
            {
                block = new BlockDiagram(n);
                columns = n;
                continue;
            }

            _ = columns;
            block.AddStatement(line);
        }

        return block;
    }

    static ArchitectureDiagram ParseArchitecture(IReadOnlyList<string> body)
    {
        var diagram = new ArchitectureDiagram();
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            diagram.AddStatement(line);
        }

        return diagram;
    }

    static C4Diagram ParseC4(string header, IReadOnlyList<string> body)
    {
        var kind = header.Trim().Split(' ')[0].ToLowerInvariant() switch
        {
            "c4container" => C4Kind.Container,
            "c4component" => C4Kind.Component,
            "c4dynamic" => C4Kind.Dynamic,
            "c4deployment" => C4Kind.Deployment,
            _ => C4Kind.Context,
        };
        var title = "C4";
        var pending = new List<string>();
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            if (line.StartsWith("title ", StringComparison.Ordinal))
                title = line["title ".Length..].Trim();
            else
                pending.Add(line);
        }

        var diagram = new C4Diagram(kind, title);
        foreach (var line in pending)
            diagram.AddStatement(line);
        return diagram;
    }

    static PacketDiagram ParsePacket(IReadOnlyList<string> body)
    {
        string? title = null;
        var fields = new List<(int Start, int End, string Label)>();
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            if (line.StartsWith("title ", StringComparison.Ordinal))
            {
                title = line["title ".Length..].Trim();
                continue;
            }

            var m = Regex.Match(line, @"^(\d+)\s*-\s*(\d+)\s*:\s*""?(.*?)""?$");
            if (m.Success)
                fields.Add((int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture), int.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture), m.Groups[3].Value));
        }

        var packet = new PacketDiagram(title);
        foreach (var f in fields)
            packet.AddField(f.Start, f.End, f.Label);
        return packet;
    }

    static RadarChart ParseRadar(IReadOnlyList<string> body)
    {
        string? title = null;
        var chart = new RadarChart();
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            if (line.StartsWith("title ", StringComparison.Ordinal))
            {
                title = line["title ".Length..].Trim();
                chart = new RadarChart(title);
                continue;
            }

            if (line.StartsWith("axis ", StringComparison.Ordinal))
            {
                var axes = line["axis ".Length..].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                chart.WithAxes(axes);
                continue;
            }

            if (line.StartsWith("curve ", StringComparison.Ordinal))
            {
                var m = Regex.Match(line, @"^curve\s+(\S+)\[""([^""]+)""\]\{(.+)\}$");
                if (m.Success)
                {
                    var values = m.Groups[3].Value.Split(',', StringSplitOptions.TrimEntries).Select(ParseDouble).ToArray();
                    chart.AddCurve(m.Groups[1].Value, m.Groups[2].Value, values);
                }

                continue;
            }

            if (line.StartsWith("max ", StringComparison.Ordinal) && double.TryParse(line[4..].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var max))
                chart.WithMax(max);
            else if (line.StartsWith("graticule ", StringComparison.Ordinal))
                chart.WithGraticule(line["graticule ".Length..].Trim());
        }

        return chart;
    }

    static Treemap ParseTreemap(IReadOnlyList<string> body)
    {
        var map = new Treemap();
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            var parts = line.Split(':', 2);
            if (parts.Length == 2 && double.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                map.AddLeaf(Unquote(parts[0].Trim()), value);
        }

        return map;
    }

    static Kanban ParseKanban(IReadOnlyList<string> body)
    {
        var kanban = new Kanban();
        KanbanColumn? column = null;
        foreach (var raw in body)
        {
            if (MermaidSourceReader.IsComment(raw) || string.IsNullOrWhiteSpace(raw))
                continue;
            var indent = MermaidSourceReader.Indent(raw);
            var text = raw.Trim();
            if (indent == 0 || column is null)
            {
                var m = Regex.Match(text, @"^(\S+)\[(.+)\]$");
                if (m.Success)
                {
                    column = new KanbanColumn(m.Groups[1].Value, m.Groups[2].Value);
                    kanban.AddColumn(column);
                }
                else
                {
                    column = new KanbanColumn(text, text);
                    kanban.AddColumn(column);
                }

                continue;
            }

            column.AddTicket(text);
        }

        return kanban;
    }

    static VennDiagram ParseVenn(IReadOnlyList<string> body)
    {
        var venn = new VennDiagram();
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            var set = Regex.Match(line, @"^set\s+(\S+)\[""([^""]+)""\]$");
            if (set.Success)
            {
                venn.AddSet(set.Groups[1].Value, set.Groups[2].Value);
                continue;
            }

            var union = Regex.Match(line, @"^union\s+(\S+),\s*(\S+)\[""([^""]+)""\]$");
            if (union.Success)
                venn.AddUnion(union.Groups[1].Value, union.Groups[2].Value, union.Groups[3].Value);
        }

        return venn;
    }

    static TreeView ParseTreeView(IReadOnlyList<string> body)
    {
        var rows = IndentedRows(body);
        if (rows.Count == 0)
            return new TreeView("Root");
        var tree = new TreeView(rows[0].Text);
        var stack = new List<(int Indent, TreeViewNode Node)> { (rows[0].Indent, tree.Root) };
        for (var i = 1; i < rows.Count; i++)
        {
            var (indent, text) = rows[i];
            while (stack.Count > 1 && indent <= stack[^1].Indent)
                stack.RemoveAt(stack.Count - 1);
            var child = stack[^1].Node.AddChild(text);
            stack.Add((indent, child));
        }

        return tree;
    }

    static IshikawaDiagram ParseIshikawa(IReadOnlyList<string> body)
    {
        var rows = IndentedRows(body);
        if (rows.Count == 0)
            return new IshikawaDiagram("Problem");
        var diagram = new IshikawaDiagram(rows[0].Text);
        var stack = new List<(int Indent, IshikawaNode Node)> { (rows[0].Indent, diagram.Root) };
        for (var i = 1; i < rows.Count; i++)
        {
            var (indent, text) = rows[i];
            while (stack.Count > 1 && indent <= stack[^1].Indent)
                stack.RemoveAt(stack.Count - 1);
            var child = stack[^1].Node.AddCause(text);
            stack.Add((indent, child));
        }

        return diagram;
    }

    static UseCaseDiagram ParseUseCase(IReadOnlyList<string> body)
    {
        var diagram = new UseCaseDiagram();
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            if (line.StartsWith("direction ", StringComparison.Ordinal))
            {
                diagram.Direction(ParseDirection(line["direction ".Length..].Trim()));
                continue;
            }

            diagram.AddStatement(line);
        }

        return diagram;
    }

    static WardleyMap ParseWardley(IReadOnlyList<string> body)
    {
        var map = new WardleyMap();
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            if (line.StartsWith("title ", StringComparison.Ordinal))
            {
                map.Title(line["title ".Length..].Trim());
                continue;
            }

            if (line.StartsWith("size ", StringComparison.Ordinal))
            {
                var nums = Regex.Matches(line, @"\d+");
                if (nums.Count >= 2)
                    map.Size(int.Parse(nums[0].Value, CultureInfo.InvariantCulture), int.Parse(nums[1].Value, CultureInfo.InvariantCulture));
                continue;
            }

            map.AddStatement(line);
        }

        return map;
    }

    static CynefinDiagram ParseCynefin(IReadOnlyList<string> body)
    {
        string? title = null;
        CynefinDomain? current = null;
        var diagram = new CynefinDiagram();
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            if (line.StartsWith("title ", StringComparison.Ordinal))
            {
                title = line["title ".Length..].Trim();
                diagram = new CynefinDiagram(title);
                continue;
            }

            if (CynefinDiagram.TryParseDomain(line, out var domain))
            {
                current = domain;
                continue;
            }

            var trans = Regex.Match(line, @"^(\w+)\s+-->\s+(\w+)(?:\s*:\s*""?(.*?)""?)?$");
            if (trans.Success && CynefinDiagram.TryParseDomain(trans.Groups[1].Value, out var from) && CynefinDiagram.TryParseDomain(trans.Groups[2].Value, out var to))
            {
                diagram.Transition(from, to, string.IsNullOrWhiteSpace(trans.Groups[3].Value) ? null : trans.Groups[3].Value);
                current = null;
                continue;
            }

            if (current is { } d)
                diagram.AddItem(d, Unquote(line));
        }

        return diagram;
    }

    static RailroadDiagram ParseRailroad(string header, IReadOnlyList<string> body)
    {
        string? title = null;
        var rules = new List<string>();
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            if (line.StartsWith("title ", StringComparison.Ordinal))
                title = Unquote(line["title ".Length..].Trim());
            else
                rules.Add(line);
        }

        var diagram = new RailroadDiagram(RailroadDiagram.FromHeader(header), title);
        foreach (var rule in rules)
            diagram.Rule(rule);
        return diagram;
    }

    static EventModelingDiagram ParseEventModeling(IReadOnlyList<string> body)
    {
        var diagram = new EventModelingDiagram();
        foreach (var raw in body)
        {
            var line = MermaidSourceReader.Meaningful(raw);
            if (line is null)
                continue;
            diagram.AddStatement(line);
        }

        return diagram;
    }

    static List<(int Indent, string Text)> IndentedRows(IReadOnlyList<string> body)
    {
        var rows = new List<(int Indent, string Text)>();
        foreach (var raw in body)
        {
            if (string.IsNullOrWhiteSpace(raw) || MermaidSourceReader.IsComment(raw))
                continue;
            rows.Add((MermaidSourceReader.Indent(raw), raw.Trim()));
        }

        return rows;
    }

    static bool TryParseNamedShape(string line, out Node node)
    {
        node = null!;
        var m = Regex.Match(line, @"^(\S+)@\{\s*shape:\s*([^,]+),\s*label:\s*""([^""]*)""\s*\}$");
        if (!m.Success)
            return false;
        node = Node.NamedShape(m.Groups[1].Value, m.Groups[2].Value.Trim(), m.Groups[3].Value);
        return true;
    }

    static bool TryParseNode(string line, out Node node)
    {
        node = null!;
        var m = Regex.Match(line, @"^([A-Za-z0-9_]+)(\S.*)$");
        if (!m.Success)
            return false;
        var id = m.Groups[1].Value;
        var rest = m.Groups[2].Value.Trim();
        if (!ShapeExtensions.TryUnwrap(rest, out var label, out var shape))
            return false;
        node = Node.Named(id, Unquote(label), shape);
        return true;
    }

    static bool TryParseLink(string line, out Link link)
    {
        link = null!;
        var m = Regex.Match(line, @"^(\S+)\s+(--+>|=+>|-\.+>)\s*(?:\|([^|]+)\|)?\s*(\S+)$");
        if (!m.Success)
        {
            m = Regex.Match(line, @"^(\S+)\s+(--+>|=+>|-\.+>)\s+(\S+)$");
            if (!m.Success)
                return false;
            link = new Link(m.Groups[1].Value, m.Groups[3].Value);
            return true;
        }

        link = new Link(m.Groups[1].Value, m.Groups[4].Value, string.IsNullOrWhiteSpace(m.Groups[3].Value) ? null : m.Groups[3].Value);
        return true;
    }

    static bool TryParseClassRelation(string line, out ClassRelation relation)
    {
        relation = null!;
        var tokens = new (string Token, ClassRelationType Type)[]
        {
            ("<|--", ClassRelationType.Inheritance),
            ("*--", ClassRelationType.Composition),
            ("o--", ClassRelationType.Aggregation),
            ("-->", ClassRelationType.Association),
            ("..|>", ClassRelationType.Realization),
            ("..>", ClassRelationType.Dependency),
            ("--", ClassRelationType.Link),
        };
        foreach (var (token, type) in tokens)
        {
            var idx = line.IndexOf(token, StringComparison.Ordinal);
            if (idx <= 0)
                continue;
            var from = line[..idx].Trim();
            var rest = line[(idx + token.Length)..].Trim();
            string to;
            string? label = null;
            var colon = rest.IndexOf(" : ", StringComparison.Ordinal);
            if (colon > 0)
            {
                to = rest[..colon].Trim();
                label = rest[(colon + 3)..].Trim();
            }
            else
                to = rest;
            relation = new ClassRelation(from, to, type, label);
            return true;
        }

        return false;
    }

    static Direction ParseDirection(string token) => token.ToUpperInvariant() switch
    {
        "TD" => Direction.TopDown,
        "BT" => Direction.BottomToTop,
        "RL" => Direction.RightToLeft,
        "LR" => Direction.LeftToRight,
        _ => Direction.TopToBottom,
    };

    static void ParseXyAxis(string rest, bool isX, out string? title, out string[] categories, out string? range)
    {
        _ = isX;
        title = null;
        categories = [];
        range = null;
        var bracket = rest.IndexOf('[');
        if (bracket >= 0)
        {
            if (bracket > 0)
                title = Unquote(rest[..bracket].Trim());
            var inner = rest[(bracket + 1)..].Trim().TrimEnd(']');
            categories = inner.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(Unquote)
                .ToArray();
            return;
        }

        var arrow = rest.IndexOf("-->", StringComparison.Ordinal);
        if (arrow > 0)
        {
            var before = rest[..arrow].Trim();
            var after = rest[(arrow + 3)..].Trim();
            var bits = before.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (bits.Length >= 2 && double.TryParse(bits[^1], NumberStyles.Float, CultureInfo.InvariantCulture, out _))
            {
                title = Unquote(string.Join(' ', bits[..^1]));
                range = bits[^1] + " --> " + after;
            }
            else
            {
                title = Unquote(before);
                range = after.Contains("-->", StringComparison.Ordinal) ? after : before + " --> " + after;
            }

            return;
        }

        title = Unquote(rest);
    }

    static string UnwrapMindmap(string text)
    {
        if (ShapeExtensions.TryUnwrap(text, out var label, out _))
            return label;
        if (text.StartsWith(")", StringComparison.Ordinal) && text.EndsWith("(", StringComparison.Ordinal) && text.Length > 2)
            return text[1..^1];
        return text;
    }

    static string Unquote(string value)
    {
        value = value.Trim();
        if (value.Length >= 2 && value[0] == '"' && value[^1] == '"')
            return value[1..^1];
        return value;
    }

    static double ParseDouble(string value) => double.Parse(value, CultureInfo.InvariantCulture);

    static string? EmptyToNull(string value) => string.IsNullOrWhiteSpace(value) ? null : value;
}
