using SkiaSharp;
using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.model.simulation;

namespace TDT4900_MasterThesis.view;

public class GraphView : IDrawable, IUpdatable
{
    private readonly Graph _graph;

    private readonly int[] _activeNodes;
    private readonly int _nodeActivationDuration;

    public GraphView(Graph graph, AppSettings appSettings)
    {
        _activeNodes = new int[graph.Nodes.Count];

        // Nodes will be activated for one second
        _nodeActivationDuration = appSettings.TargetTPS / 4;

        _graph = graph;
    }

    public void Draw(SKCanvas canvas)
    {
        foreach (Edge e in _graph.Edges)
        {
            DrawEdge(e, canvas);
        }

        foreach (Node n in _graph.Nodes)
        {
            DrawNode(n, canvas);
        }
    }

    private void DrawEdge(Edge e, SKCanvas canvas)
    {
        canvas.DrawLine(
            e.Source.X,
            e.Source.Y,
            e.Target.X,
            e.Target.Y,
            new SKPaint { Color = SKColors.Black }
        );
    }

    private void DrawNode(Node n, SKCanvas canvas)
    {
        int radius = 20;

        if (_activeNodes[n.Id] > 0)
        {
            canvas.DrawCircle(n.X, n.Y, radius, new SKPaint { Color = SKColors.Green });
        }
        else
        {
            canvas.DrawCircle(n.X, n.Y, radius, new SKPaint { Color = SKColors.Coral });
        }

        canvas.DrawText(
            $"{n.Id}",
            n.X,
            n.Y,
            SKTextAlign.Center,
            new SKFont(SKTypeface.Default, 14),
            new SKPaint { Color = SKColors.Black }
        );
    }

    public void ActivateNode(Node n)
    {
        _activeNodes[n.Id] = _nodeActivationDuration;
    }

    public void Update(long currentTick)
    {
        for (int i = 0; i < _activeNodes.Length; i++)
        {
            if (_activeNodes[i] > 0)
            {
                _activeNodes[i]--;
            }
        }
    }
}
