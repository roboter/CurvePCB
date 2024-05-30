using System;
using System.Collections.Generic;
using System.Drawing;

public class BezierCurve
{
    public static PointF CalculateBezierPoint(float t, PointF p0, PointF p1, PointF p2, PointF p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        PointF p = new PointF();
        p.X = uuu * p0.X;
        p.X += 3 * uu * t * p1.X;
        p.X += 3 * u * tt * p2.X;
        p.X += ttt * p3.X;

        p.Y = uuu * p0.Y;
        p.Y += 3 * uu * t * p1.Y;
        p.Y += 3 * u * tt * p2.Y;
        p.Y += ttt * p3.Y;

        return p;
    }

    public static List<PointF> GenerateBezierPath(List<Point> path)
    {
        var bezierPath = new List<PointF>();
        for (int i = 0; i < path.Count - 3; i += 3)
        {
            var p0 = new PointF(path[i].X, path[i].Y);
            var p1 = new PointF(path[i + 1].X, path[i + 1].Y);
            var p2 = new PointF(path[i + 2].X, path[i + 2].Y);
            var p3 = new PointF(path[i + 3].X, path[i + 3].Y);

            for (float t = 0; t <= 1; t += 0.01f)
            {
                bezierPath.Add(CalculateBezierPoint(t, p0, p1, p2, p3));
            }
        }
        return bezierPath;
    }
}