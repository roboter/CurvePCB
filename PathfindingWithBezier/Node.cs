namespace PathfindingWithBezier;

public class Node
{
    public int X { get; set; }
    public int Y { get; set; }
    public Node Parent { get; set; }
    public float G { get; set; }
    public float H { get; set; }
    public float F => G + H;

    public Node(int x, int y)
    {
        X = x;
        Y = y;
    }
}


