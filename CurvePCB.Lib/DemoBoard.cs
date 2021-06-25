using System.Collections.Generic;

namespace CurvePCB.Lib
{
    public static class DemoBoard
    {
        const double lqfpPinWidth = .2;
        const double lqfpPinHeight = 1;

        const double lqfpPinSpacing = .5;
        const double lqfnBorder = 12;

        const double lqfpChipSize = 10;

        const double lqfpChipAllPins = 7.5;
        const double lqfpChipFirstPin = (lqfnBorder - lqfpChipAllPins) / 2;

        public static Schematic Generate()
        {
            List<Element> connectorPins = new();

            for (int i = 1; i != (16 * 2) + 1; i++)
            {
                connectorPins.Add(new Element { Name = $"Pin{i}", Shape = new Shape { Name = $"PIN{i}", Size = new Size(Constants.IN / 2, Constants.IN / 2) }, Position = new Position(i * Constants.IN, Constants.IN) });
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
                lqfpPins.Add(new Element { Name = $"Pin{i}", Shape = new Shape { Name = $"PIN{i}", Size = new Size(lqfpPinHeight, lqfpPinWidth) }, Position = new Position(lqfpPinHeight / 2, lqfpChipFirstPin + (i - 1) * lqfpPinSpacing - lqfpPinWidth / 2) });
                //17 -32
                lqfpPins.Add(new Element { Name = $"Pin{i + 16}", Shape = new Shape { Name = $"PIN{i + 16}", Size = new Size(lqfpPinWidth, lqfpPinHeight) }, Position = new Position(lqfpChipFirstPin + (i - 1) * lqfpPinSpacing - lqfpPinWidth / 2 , lqfpChipSize + lqfpPinHeight / 2 + lqfpPinHeight) });
                //33 -48
                lqfpPins.Add(new Element { Name = $"Pin{i + 32}", Shape = new Shape { Name = $"PIN{i + 32}", Size = new Size(lqfpPinHeight, lqfpPinWidth) }, Position = new Position(lqfpChipSize + lqfpPinHeight + lqfpPinHeight / 2, lqfpChipFirstPin + (i - 1) * lqfpPinSpacing - lqfpPinWidth / 2) });
                //49 - 54
                lqfpPins.Add(new Element { Name = $"Pin{i + 48}", Shape = new Shape { Name = $"PIN{i + 48}", Size = new Size(lqfpPinWidth, lqfpPinHeight) }, Position = new Position(lqfpChipFirstPin + (i - 1) * lqfpPinSpacing - lqfpPinWidth / 2, lqfpPinHeight / 2) });
            }

            var u1 = new Element { Name = "U1", Shape = new Shape { Name = "LQFP", Pins = lqfpPins, Size = new Size(lqfnBorder, lqfnBorder) }, Position = new Position(Constants.IN * 16 + Constants.IN / 2, Constants.IN * 5) };
            var j1 = new Element { Name = "J1", Shape = connector, Position = new Position(Constants.IN * 2, Constants.IN * 1) };
            var j2 = new Element { Name = "J2", Shape = connector, Position = new Position(Constants.IN * 2, Constants.IN * 12) };

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
