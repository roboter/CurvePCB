using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace CurvePCB.Lib
{
    public class Dimention
    {
        public Dimention(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public bool Contains(Point p) => (p.X > X && p.X < X + Width && p.Y > Y && p.Y < Y + Height);
    }

    public class HeaderPins : Shape
    {
        const double PinWidth = 1;

        private int Count;

        private double Pitch;

        public HeaderPins(int count, double pitch = Constants.IN)
        {
            Count = count;
            Pitch = pitch;
        }

        /// <summary>
        /// Get the geometry that defines this shape
        /// </summary>
        //protected override Geometry DefiningGeometry
        //{
        //    get
        //    {
        //        return _geometry;
        //    }
        //}

        //private EllipseGeometry _geometry = new(new Rect(100, 100, 100, 100));

        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    Debug.WriteLine("OnRender");
        //    for (int i = 0; i != Count; i++)
        //    {
        //        drawingContext.DrawRectangle(Brushes.DodgerBlue, null, new Rect { Height = PinWidth, Width = PinWidth, Location = new Point { X = -0.5, Y = -0.5 + Pitch * i } });
        //    }
        //}
    }

    public class Pins : IElement
    {
        private bool selected = false;
        Dimention dimention;
        //private Canvas canvas;
        private const int pinsOnSide = 16;
        private Point pos;
        //private List<System.Windows.Shapes.Shape> rectangles = new List<System.Windows.Shapes.Shape>();
        private const double width = 1; // mm
        private const double height = 1; // mm
        private const double next = 2.54; // mm
        public Guid Id { get; } = Guid.NewGuid();

        public Pins()
        {

            // this.canvas = canvas;
        }

        public void Clicked(Point brushPosition)
        {
            if (selected)
            {
                selected = false;
                return;
            }

            if (dimention.Contains(brushPosition))
            {
                selected = true;
            }
        }

        public ConnectionPoint[] Draw(Point offset)
        {
            pos = offset;
            dimention = new Dimention(pos.X, pos.Y, 12, 12);
            var points = new List<ConnectionPoint>();

            for (int i = 0; i != pinsOnSide; i++)
            {
                //var rectangle1 = new Rectangle { RadiusX = 5, Width = width, Height = height };
                //Canvas.SetLeft(rectangle1, offset.X - width / 2);
                //// points.Add(new ConnectionPoint(@"PIN_{i}"));
                //Canvas.SetTop(rectangle1, offset.Y + i * (next) - width / 2);
                //rectangle1.Fill = Brushes.Fuchsia;
                //rectangle1.Stroke = Brushes.Black;
                //canvas.Children.Add(rectangle1);
                //rectangles.Add(rectangle1);
            }
            return points.ToArray();
        }

        public void Move(Point point)
        {

        }
    }

    public class LQFP : IElement
    {
        const double IN = 2.54;
        private const double width = 1.2; // mm
        private const double height = 0.3; // mm
        private const double start = (10 - (7.5)) / 2 + 1 - 0.3 / 2;
        private const double next = 0.500; // mm
        private const int pinsOnSide = 16;
        private List<Rectangle> rectangles = new List<Rectangle>();
        private Point pos;
        private bool selected = false;
        Dimention dimention;
        private Rectangle border;
        private Rectangle body;

        //  public Canvas canvas { get; set; }
        public LQFP()
        {
            //    this.canvas = canvas;
        }

        public ConnectionPoint[] Draw(Point offset)
        {
            pos = offset;
            dimention = new Dimention(pos.X, pos.Y, 12, 12);
            var points = new List<ConnectionPoint>();
            //for (int i = 0; i != pinsOnSide; i++)
            //{
            //    var rectangle1 = new Rectangle { RadiusX = 5, Width = width, Height = height };
            //    Canvas.SetLeft(rectangle1, offset.X);
            //    //  points.Add(new ConnectionPoint($"PIN_{i}"));
            //    Canvas.SetTop(rectangle1, offset.Y + i * (next) + start);
            //    rectangle1.Fill = Brushes.Fuchsia;
            //    rectangle1.Stroke = Brushes.Black;

            //    //rectangle1.LayoutTransform = new RotateTransform(45);  // Rotate
            //    canvas.Children.Add(rectangle1);
            //    rectangles.Add(rectangle1);

            //    var rectangle2 = new Rectangle { RadiusX = 5, Width = width, Height = height };
            //    Canvas.SetLeft(rectangle2, offset.X + 10 + 0.8);
            //    // points.Add(new ConnectionPoint($"PIN_{pinsOnSide + i}"));
            //    Canvas.SetTop(rectangle2, offset.Y + i * (next) + start);
            //    rectangle2.Fill = Brushes.Fuchsia;
            //    rectangle2.Stroke = Brushes.Black;

            //    //rectangle2.LayoutTransform = new RotateTransform(45);  // Rotate
            //    canvas.Children.Add(rectangle2);
            //    rectangles.Add(rectangle2);

            //    var rectangle3 = new Rectangle { RadiusX = 5, Width = height, Height = width };
            //    Canvas.SetLeft(rectangle3, offset.X + i * (next) + start);
            //    //  points.Add(new ConnectionPoint($"PIN_{pinsOnSide * 2 + i}"));
            //    Canvas.SetTop(rectangle3, offset.Y + 10 + 0.8);
            //    rectangle3.Fill = Brushes.Fuchsia;
            //    rectangle3.Stroke = Brushes.Black;

            //    //rectangle3.LayoutTransform = new RotateTransform(45);  // Rotate
            //    canvas.Children.Add(rectangle3);
            //    rectangles.Add(rectangle3);

            //    var rectangle4 = new Rectangle { RadiusX = 5, Width = height, Height = width };
            //    Canvas.SetLeft(rectangle4, offset.X + i * (next) + start);
            //    //   points.Add(new ConnectionPoint($"PIN_{pinsOnSide * 3 + i}"));
            //    Canvas.SetTop(rectangle4, offset.Y);
            //    rectangle4.Fill = Brushes.Fuchsia;
            //    rectangle4.Stroke = Brushes.Black;

            //    //rectangle4.LayoutTransform = new RotateTransform(45);  // Rotate

            //    canvas.Children.Add(rectangle4);
            //    rectangles.Add(rectangle4);
            //}

            //#region border
            //border = new Rectangle { RadiusX = 0.5, Width = 12, Height = 12 }; //mm
            //border.ForceCursor = true;
            //border.Cursor = Cursors.Hand;
            //border.IsMouseDirectlyOverChanged += Border_IsMouseDirectlyOverChanged;
            //border.Stroke = Brushes.Pink;
            //border.StrokeThickness = 0.1;
            //border.StrokeDashArray = DoubleCollection.Parse("4 4");
            //Canvas.SetLeft(border, offset.X);

            //Canvas.SetTop(border, offset.Y);

            //canvas.Children.Add(border);
            //#endregion

            //#region body
            //body = new Rectangle { RadiusX = 2, Width = 10, Height = 10 }; // mm
            //body.ForceCursor = true;
            //body.Cursor = Cursors.Hand;
            //body.IsMouseDirectlyOverChanged += Border_IsMouseDirectlyOverChanged;
            //body.Stroke = Brushes.IndianRed;
            //body.StrokeThickness = 0.2;
            //body.Opacity = 0.5;
            ////     body.StrokeDashArray = DoubleCollection.Parse("4 4");
            //Canvas.SetLeft(body, offset.X + 1);

            //Canvas.SetTop(body, offset.Y + 1);

            //canvas.Children.Add(body);
            //#endregion

            //#region pins
            //var pins = new Rectangle { RadiusX = 2, Width = 7.5, Height = 7.5 }; // mm
            //pins.ForceCursor = true;
            //pins.Cursor = Cursors.Hand;
            //pins.IsMouseDirectlyOverChanged += Border_IsMouseDirectlyOverChanged;
            //pins.Stroke = Brushes.IndianRed;
            //pins.StrokeThickness = 0.01;
            //pins.Opacity = 0.5;
            ////     body.StrokeDashArray = DoubleCollection.Parse("4 4");
            //Canvas.SetLeft(pins, offset.X + (10 - 7.5) / 2 + 1);

            //Canvas.SetTop(pins, offset.Y + (10 - 7.5) / 2 + 1);

            //canvas.Children.Add(pins);
            //#endregion
            return points.ToArray();
        }

        //private void Border_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    if (sender is Rectangle && e.NewValue is bool)
        //    {
        //        ((Rectangle)sender).Stroke = (bool)e.NewValue ? Brushes.Gray : Brushes.Pink;
        //    }
        //    Debug.WriteLine(e.NewValue is bool ? (bool)e.NewValue : null);
        //}

        public void Move(Point brushPosition)
        {
            //if (selected)
            //{
            //    pos = brushPosition;
            //    dimention.X = brushPosition.X;
            //    dimention.Y = brushPosition.Y;

            //    foreach (var rectangle in rectangles)
            //    {
            //        Canvas.SetLeft(rectangle, Canvas.GetLeft(border) - Canvas.GetLeft(rectangle) + brushPosition.X);
            //        Canvas.SetTop(rectangle, Canvas.GetTop(border) - Canvas.GetTop(rectangle) + brushPosition.Y);
            //    }

            //    Canvas.SetLeft(border, brushPosition.X);
            //    Canvas.SetTop(border, brushPosition.Y);

            //    Canvas.SetLeft(body, brushPosition.X + 1);
            //    Canvas.SetTop(body, brushPosition.Y + 1);
            //}
            //else
            //{
            //    if (dimention.Contains(brushPosition))
            //    {
            //        foreach (var rectangle in rectangles)
            //        {
            //            rectangle.Fill = Brushes.Black;
            //        }
            //    }
            //    else
            //    {
            //        foreach (var rectangle in rectangles)
            //        {
            //            rectangle.Fill = Brushes.Fuchsia;
            //        }
            //    }
            //}
        }

        public void Clicked(Point brushPosition)
        {
            if (selected)
            {
                selected = false;
                return;
            }

            if (dimention.Contains(brushPosition))
            {
                selected = true;
            }
        }
    }

    public interface IElement
    {
        void Move(Point point);

        void Clicked(Point position);

        ConnectionPoint[] Draw(Point point);
    }

    public class ConnectionPoint
    {
        public Element Element { get; set; }

        public Element ElementPin { get; set; }
    }

    public class Net
    {
        public string Name { get; set; }

        public List<ConnectionPoint> Connection { get; set; }
    }

    public class Size
    {
        public Size(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }

        public double Width { get; set; }

        public double Height { get; set; }
    }

    public class Schematic
    {
        public Size Size { get; set; }

        public string Name { get; set; }

        public List<Element> Elements { get; set; }

        public List<Net> Nets { get; set; }
    }

    public class Position
    {
        public Position(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double X { get; set; }

        public double Y { get; set; }
    }

    public class Element
    {
        public string Name { get; set; }

        public Shape Shape { get; set; }

        public Position Position { get; set; }
        public double Transform { get; set; }
        public double CenterY { get; set; }
        public double CenterX { get; set; }
    }

    public class Shape
    {
        public Size Size { get; set; }

        public string Name { get; set; }

        public int PinsCount { get; set; }

        public IList<Element> Pins { get; set; }
    }
}
