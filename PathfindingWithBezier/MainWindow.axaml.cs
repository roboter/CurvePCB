using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System.Collections.Generic;
using System.Drawing;
using Brushes = Avalonia.Media.Brushes;
using Point = Avalonia.Point;

namespace PathfindingWithBezier;

public partial class MainWindow : Window
{
    private int[,] grid;
    private List<PointF> smoothPath;
    private const int cellSize = 40;

    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        InitializeGrid();
        FindPath();
        DrawGridAndPath();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void InitializeGrid()
    {
        grid = new int[10, 10];
        // Example obstacles
        grid[4, 4] = 1;
        grid[4, 5] = 1;
        grid[4, 6] = 1;
    }

    private void FindPath()
    {
        var start = new Node(0, 0);
        var end = new Node(9, 9);
        smoothPath = PathfindingWBezier.FindSmoothPath(grid, start, end);
    }

    private void DrawGridAndPath()
    {
        var canvas = this.FindControl<Canvas>("canvas");

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                var rect = new Avalonia.Controls.Shapes.Rectangle
                {
                    Width = cellSize,
                    Height = cellSize,
                    Fill = grid[x, y] == 1 ? Brushes.Black : Brushes.LightGray,
                    Stroke = Brushes.Gray,
                    StrokeThickness = 1,
                    [Canvas.LeftProperty] = x * cellSize,
                    [Canvas.TopProperty] = y * cellSize
                };
                canvas.Children.Add(rect);
            }
        }

        if (smoothPath != null && smoothPath.Count > 0)
        {
            var pathFigure = new PathFigure { StartPoint = new Point(smoothPath[0].X * cellSize + cellSize / 2, smoothPath[0].Y * cellSize + cellSize / 2) };

            for (int i = 1; i < smoothPath.Count; i += 3)
            {
                if (i + 2 < smoothPath.Count)
                {
                    var bezierSegment = new BezierSegment
                    {
                        Point1 = new Point(smoothPath[i].X * cellSize + cellSize / 2, smoothPath[i].Y * cellSize + cellSize / 2),
                        Point2 = new Point(smoothPath[i + 1].X * cellSize + cellSize / 2, smoothPath[i + 1].Y * cellSize + cellSize / 2),
                        Point3 = new Point(smoothPath[i + 2].X * cellSize + cellSize / 2, smoothPath[i + 2].Y * cellSize + cellSize / 2)
                    };

                    pathFigure.Segments.Add(bezierSegment);
                }
            }

            var pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);

            var path = new Avalonia.Controls.Shapes.Path
            {
                Data = pathGeometry,
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };

            canvas.Children.Add(path);
        }
    }

}


