using System.Collections.Generic;

namespace CurvePCB.Lib
{
    public class ConnectionPoint
    {
        public Element Element { get; set; }

        public int ElementPin { get; set; }
    }

    public class Net
    {
        public string Name { get; set; }

        public List<ConnectionPoint> Connection { get; set; }
    }

    public class Schematic
    {
        public string Name { get; set; }

        public List<Element> Elements { get; set; }

        public List<Net> Nets { get; set; }
    }

    public class Element
    {
        public string Name { get; set; }

        public Shape Shape { get; set; }
    }

    public class Shape
    {
        public string Name { get; set; }

        public int Pins { get; set; }
    }
}
