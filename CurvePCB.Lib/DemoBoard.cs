using System.Collections.Generic;
using System.Linq;

namespace CurvePCB.Lib
{
    public static class DemoBoard
    {
        const double qfp64PinWidth = .2;
        const double qfp64PinHeight = 1;

        const double qfp64PinSpacing = .5;
        const double qfp64Border = 12;

        const double qfp64ChipSize = 10;

        const double qfp64ChipAllPins = 7.5;
        const double qfp64ChipFirstPin = (qfp64Border - qfp64ChipAllPins) / 2;

        public static Schematic Generate()
        {
            List<Element> connectorPins = new();

            for (int i = 1; i != (16 * 2) + 1; i++)
            {
                connectorPins.Add(new Element { Name = $"PIN{i}", Shape = new Shape { Name = $"PIN{i}", Size = new Size(Constants.IN / 2, Constants.IN / 2) }, Position = new Position(i * Constants.IN, Constants.IN) });
            }

            var connector = new Shape
            {
                Name = "Connector",
                Pins = connectorPins,
                Size = new Size(Constants.IN * 33, Constants.IN * 2)
            };

            List<Element> lqfpPins = new();
            for (int i = 1; i != 17; i++)
            {
                //1 - 16
                lqfpPins.Add(new Element { Name = $"PIN{i}", Shape = new Shape { Name = $"PIN{i}", Size = new Size(qfp64PinHeight, qfp64PinWidth) }, Position = new Position(qfp64PinHeight / 2, qfp64ChipFirstPin + (i - 1) * qfp64PinSpacing - qfp64PinWidth / 2) });
                //17 -32
                lqfpPins.Add(new Element { Name = $"PIN{i + 16}", Shape = new Shape { Name = $"PIN{i + 16}", Size = new Size(qfp64PinWidth, qfp64PinHeight) }, Position = new Position(qfp64ChipFirstPin + (i - 1) * qfp64PinSpacing - qfp64PinWidth / 2, qfp64ChipSize + qfp64PinHeight / 2 + qfp64PinHeight) });
                //33 -48
                lqfpPins.Add(new Element { Name = $"PIN{i + 32}", Shape = new Shape { Name = $"PIN{i + 32}", Size = new Size(qfp64PinHeight, qfp64PinWidth) }, Position = new Position(qfp64ChipSize + qfp64PinHeight + qfp64PinHeight / 2, qfp64ChipFirstPin + (i - 1) * qfp64PinSpacing - qfp64PinWidth / 2) });
                //49 - 64
                lqfpPins.Add(new Element { Name = $"PIN{i + 48}", Shape = new Shape { Name = $"PIN{i + 48}", Size = new Size(qfp64PinWidth, qfp64PinHeight) }, Position = new Position(qfp64ChipFirstPin + (i - 1) * qfp64PinSpacing - qfp64PinWidth / 2, qfp64PinHeight / 2) });
            }

            var u1 = new Element
            {
                Name = "U1",
                Shape = new Shape { Name = "QFP64", Pins = lqfpPins, Size = new Size(qfp64Border, qfp64Border) },
                Position = new Position(Constants.IN * 16 + Constants.IN / 2, Constants.IN * 5),
                //Transform = 45,
                //CenterX = qfp64Border / 2,
                //CenterY = qfp64Border / 2,
            };
            var j1 = new Element { Name = "J1", Shape = connector, Position = new Position(Constants.IN * 2, Constants.IN * 1) };
            var j2 = new Element { Name = "J2", Shape = connector, Position = new Position(Constants.IN * 2, Constants.IN * 12) };

            var nets = new List<Net>();
            for (int i = 1; i != 17; i++)
            {
                nets.Add(new Net
                {
                    Name = $"U1_{i} to J2_{i}",
                    Connection = new List<ConnectionPoint> { new ConnectionPoint { Element = u1, ElementPin = lqfpPins.FirstOrDefault(x => x.Name == $"PIN{i}") }, new ConnectionPoint { Element = j2, ElementPin = connectorPins.FirstOrDefault(x => x.Name == $"PIN{i}") } }
                });

                nets.Add(new Net
                {
                    Name = $"U1_{i + 16} to J2_{i + 16}",
                    Connection = new List<ConnectionPoint> { new ConnectionPoint { Element = u1, ElementPin = lqfpPins.FirstOrDefault(x => x.Name == $"PIN{ i + 16}") }, new ConnectionPoint { Element = j2, ElementPin = connectorPins.FirstOrDefault(x => x.Name == $"PIN{i + 16}") } }
                });

                nets.Add(new Net
                {
                    Name = $"U1_{i + 16 * 2} to J1_{i + 16}",
                    Connection = new List<ConnectionPoint> { new ConnectionPoint { Element = u1, ElementPin = lqfpPins.FirstOrDefault(x => x.Name == $"PIN{ i + 16 * 2}") }, new ConnectionPoint { Element = j1, ElementPin = connectorPins.FirstOrDefault(x => x.Name == $"PIN{i + 16 }") } }
                });

                nets.Add(new Net
                {
                    Name = $"U1_{i + 16 * 3} to J1_{i }",
                    Connection = new List<ConnectionPoint> { new ConnectionPoint { Element = u1, ElementPin = lqfpPins.FirstOrDefault(x => x.Name == $"PIN{ i + 16 * 3}") }, new ConnectionPoint { Element = j1, ElementPin = connectorPins.FirstOrDefault(x => x.Name == $"PIN{i }") } }
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
