using CurvePCB.Lib;
using CurvePCB.Lib.JsonReferenceHandler;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Xunit;

namespace CurvePCB.Test
{
    public class SchematicTest
    {
        [Fact]
        public void SchematicJsonTest()
        {
            var u1 = new Element { Name = "U1" };
            var connectionPoint = new ConnectionPoint
            {
                Element = u1,
                ElementPin = 1
            };

            var net = new Net
            {
                Name = "Pin1",
                Connection = new List<ConnectionPoint> { connectionPoint }
            };
            var testSchematic = new Schematic
            {
                Name = "ConnectorsTest",
                Elements = new List<Element> { u1 },
                Nets = new List<Net> { net }
            };

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = new GuidReferenceHandler()
            };
            var json = JsonSerializer.Serialize(testSchematic, options);

            var deserializedSchematic = JsonSerializer.Deserialize<Schematic>(json, options);

            Assert.Equal(testSchematic.Name, deserializedSchematic.Name);
        }

        [Fact]
        public void SchematicNetTest()
        {
            var connector = new Shape { Name = "Connector", Pins = 16 * 2 };
            var u1 = new Element { Name = "U1", Shape = new Shape { Name = "LQFP", Pins = 16 * 4 } };
            var j1 = new Element { Name = "J1", Shape = connector };
            var j2 = new Element { Name = "J2", Shape = connector };

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

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = new GuidReferenceHandler()
            };

            File.WriteAllText("test.json", JsonSerializer.Serialize(new Schematic
            {
                Name = "ConnectorsTest",
                Elements = new List<Element> { u1, j1, j2 },
                Nets = nets
            }, options));
        }
    }
}
