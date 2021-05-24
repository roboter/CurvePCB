using System.Collections.Generic;

namespace CurvePCB.Lib
{
    public static class DemoBoard
    {
        public static Schematic Generate()
        {
            var connector = new Shape { Name = "Connector", Pins = 16 * 2, Size = new Size(100, 10) };
            var u1 = new Element { Name = "U1", Shape = new Shape { Name = "LQFP", Pins = 16 * 4, Size = new Size(100, 100) }, Position = new Position(10, 10) };
            var j1 = new Element { Name = "J1", Shape = connector, Position = new Position(20, 20) };
            var j2 = new Element { Name = "J2", Shape = connector, Position = new Position(30, 30) };

            var nets = new List<Net>();
            for (int i = 1; i != 17; i++)
            {
                nets.Add(new Net
                {
                    Name = $"U1_Pin{i}",
                    Connection = new List<ConnectionPoint> { new ConnectionPoint { Element = u1, ElementPin = i }, new ConnectionPoint { Element = j1, ElementPin = i } }
                });

                nets.Add(new Net
                {
                    Name = $"U1_Pin{i + 16}",
                    Connection = new List<ConnectionPoint> { new ConnectionPoint { Element = u1, ElementPin = i + 16 }, new ConnectionPoint { Element = j1, ElementPin = i + 16 } }
                });

                nets.Add(new Net
                {
                    Name = $"U1_Pin{i + 16 * 2}",
                    Connection = new List<ConnectionPoint> { new ConnectionPoint { Element = u1, ElementPin = i + 16 * 2 }, new ConnectionPoint { Element = j2, ElementPin = i } }
                });

                nets.Add(new Net
                {
                    Name = $"U1_Pin{i + 16 * 3}",
                    Connection = new List<ConnectionPoint> { new ConnectionPoint { Element = u1, ElementPin = i + 16 * 3 }, new ConnectionPoint { Element = j2, ElementPin = i + 16 } }
                });
            }

            return new Schematic
            {
                Name = "ConnectorsTest",
                Elements = new List<Element> { u1, j1, j2 },
                Nets = nets
            };
        }
    }
}
