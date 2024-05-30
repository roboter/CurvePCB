using System;
using System.Collections.Generic;
using System.Drawing;
using PathfindingWithBezier;

public class PathfindingWBezier
{
    public static List<PointF> FindSmoothPath(int[,] grid, Node start, Node end)
    {
        var roughPath = AStar.FindPath(grid, start, end);
        if (roughPath.Count < 4)
            return new List<PointF>();

        var pointList = new List<Point>();
        foreach (var node in roughPath)
        {
            pointList.Add(new Point(node.X, node.Y));
        }

        return BezierCurve.GenerateBezierPath(pointList);
    }
}