using System;
using System.Collections.Generic;
using PathfindingWithBezier;

public class AStar
{
    private static readonly int[] Deltas = { 0, 1, 0, -1, 1, 0, -1, 0 };

    public static List<Node> FindPath(int[,] grid, Node start, Node end)
    {
        var openList = new List<Node> { start };
        var closedList = new HashSet<Node>();

        start.G = 0;
        start.H = Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y);

        while (openList.Count > 0)
        {
            openList.Sort((a, b) => a.F.CompareTo(b.F));
            var current = openList[0];
            openList.RemoveAt(0);

            if (current.X == end.X && current.Y == end.Y)
            {
                return ReconstructPath(current);
            }

            closedList.Add(current);

            for (int i = 0; i < Deltas.Length; i += 2)
            {
                var neighborX = current.X + Deltas[i];
                var neighborY = current.Y + Deltas[i + 1];

                if (neighborX < 0 || neighborX >= grid.GetLength(0) || neighborY < 0 || neighborY >= grid.GetLength(1))
                    continue;

                if (grid[neighborX, neighborY] == 1)
                    continue;

                var neighbor = new Node(neighborX, neighborY) { Parent = current, G = current.G + 1 };

                if (closedList.Contains(neighbor))
                    continue;

                neighbor.H = Math.Abs(neighbor.X - end.X) + Math.Abs(neighbor.Y - end.Y);

                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
                else
                {
                    var existingNeighbor = openList.Find(n => n.X == neighborX && n.Y == neighborY);
                    if (existingNeighbor != null && existingNeighbor.G > neighbor.G)
                    {
                        existingNeighbor.Parent = current;
                        existingNeighbor.G = neighbor.G;
                    }
                }
            }
        }

        return new List<Node>();
    }

    private static List<Node> ReconstructPath(Node node)
    {
        var path = new List<Node>();
        while (node != null)
        {
            path.Add(node);
            node = node.Parent;
        }
        path.Reverse();
        return path;
    }
}