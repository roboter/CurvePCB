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

        if (path.Count < 4)
            return bezierPath;

        for (int i = 0; i < path.Count - 1; i += 3)
        {
            PointF p0 = new PointF(path[i].X, path[i].Y);
            PointF p1 = new PointF(path[Math.Min(i + 1, path.Count - 1)].X, path[Math.Min(i + 1, path.Count - 1)].Y);
            PointF p2 = new PointF(path[Math.Min(i + 2, path.Count - 1)].X, path[Math.Min(i + 2, path.Count - 1)].Y);
            PointF p3 = new PointF(path[Math.Min(i + 3, path.Count - 1)].X, path[Math.Min(i + 3, path.Count - 1)].Y);

            for (float t = 0; t <= 1; t += 0.01f)
            {
                bezierPath.Add(CalculateBezierPoint(t, p0, p1, p2, p3));
            }
        }

        return bezierPath;
    }
}
