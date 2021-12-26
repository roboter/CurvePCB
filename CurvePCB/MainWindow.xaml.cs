using CurvePCB.Lib;
using Svg;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CurvePCB
{
    public class DrawingElement : System.Windows.Shapes.Shape
    {
        public decimal RadiusX { get; set; }
        public decimal RadiusY { get; set; }

        public Element _element;

        public DrawingElement(Element element)
        {
            _element = element;
            ellipse = new EllipseGeometry();
        }

        EllipseGeometry ellipse;
        protected override Geometry DefiningGeometry
        {
            get
            {
                return ellipse;
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {

            var br = new SolidColorBrush();
            br.Opacity = 1;
            drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Stroke, 0.1), new Rect(0, 0, _element.Shape.Size.Width, _element.Shape.Size.Height));

            drawingContext.DrawText(new FormattedText(_element.Name, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("Comic Sans"), 3, Brushes.Coral), new Point(0, 0));

            #region DrawingPins
            if (_element.Shape.Pins != null)
            {
                foreach (var pin in _element.Shape.Pins)
                {

                    //  drawingContext.DrawRectangle(Brushes.Transparent, new(Brushes.Fuchsia, 0.1), new Rect(pin.Position.X - pin.Shape.Size.Width - _element.Shape.Size.Width / 2, pin.Position.Y - pin.Shape.Size.Height - _element.Shape.Size.Height / 2, pin.Shape.Size.Width * 2, Height = pin.Shape.Size.Height * 2));

                    drawingContext.DrawRectangle(Brushes.Red, new(Brushes.Fuchsia, 0.1), new Rect(pin.Position.X - pin.Shape.Size.Width / 2, pin.Position.Y - pin.Shape.Size.Height / 2, pin.Shape.Size.Width, Height = pin.Shape.Size.Height));
                }
            }
            #endregion
        }
    }

    public class DrawingNet : System.Windows.Shapes.Shape
    {

        Net _net;
        LineGeometry line;

        public DrawingNet(Net net)
        {
            _net = net;
            line = new LineGeometry();
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                return line;
            }
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
          //  if (_net.Connection.Count < 2) return;

            var beginConnection = _net.Connection.First();

           // drawingContext.DrawRectangle(Brushes.Red, new Pen(Stroke, 0.1), new Rect(beginConnection.Element.Position.X, beginConnection.Element.Position.Y,1,1));

            //   drawingContext.DrawRectangle(Brushes.Red, new Pen(Stroke, 0.1), new Rect(-0.1, -0.1, 2, 2));

            if (_net.Connection.Count > 1)
            {
                ConnectionPoint begin = _net.Connection[0];
                foreach (var con in _net.Connection.Skip(1))
                {
                    //var len = 0.00001;
                    //drawingContext.DrawRectangle(Brushes.Red, new(Brushes.Fuchsia, 0.1), new Rect(con.Element.Position.X - len, con.Element.Position.X + len, con.Element.Position.Y - len, con.Element.Position.Y + len));
                    var start = new Point(begin.Element.Position.X + begin.ElementPin.Position.X, begin.Element.Position.Y + begin.ElementPin.Position.Y);
                    var end = new Point(con.Element.Position.X + con.ElementPin.Position.X, con.Element.Position.Y + con.ElementPin.Position.Y);
                    drawingContext.DrawLine(new Pen(Stroke, .2), start, end);
                    if (Stroke == Brushes.OrangeRed)
                        drawingContext.DrawText(new FormattedText(_net.Name, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("Comic Sans"), 3, Brushes.Coral), end);
                    begin = con;
                }
            }
        }
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point currentPoint = new Point();
        private bool IsMouseDown = false;
        private int childcounter = 0;
        private double zoom = 10;
        private List<Point> curvePoints = new();
        private List<IElement> elements = new();
        private List<Rectangle> drPoints = new();
        private Schematic schematic;
        const double padRounding = .1;

        Point[] points1 =
        {
            new Point(Constants.IN, Constants.IN),
            new Point(Constants.IN * 5, Constants.IN),
            new Point(Constants.IN * 8, Constants.IN * 4),
            new Point(Constants.IN * 10, Constants.IN * 5)
        };

        bool captured = false;
        double x_shape, x_canvas, y_shape, y_canvas;
        UIElement source = null;

        #region Element
        private void DrawElement(Element element)
        {

            var drawingElement = new DrawingElement(element);
            Canvas.SetLeft(drawingElement, element.Position.X);
            Canvas.SetTop(drawingElement, element.Position.Y);
            if(element.Transform > 0)
                drawingElement.RenderTransform = new RotateTransform(element.Transform, element.CenterX, element.CenterY);
            //Canvas.LayoutTransformProperty(drawingElement, RotateTransform)
            drawingElement.MouseEnter += (object sender, MouseEventArgs e) =>
            {
                ((DrawingElement)sender).Stroke = Brushes.OrangeRed;
            };
            drawingElement.MouseLeave += (object sender, MouseEventArgs e) =>
            {
                ((DrawingElement)sender).Stroke = Brushes.Black;
            };
            drawingElement.MouseLeftButtonDown += Rect_MouseLeftButtonDown;
            drawingElement.MouseMove += Rect_MouseMove;
            drawingElement.MouseLeftButtonUp += Rect_MouseLeftButtonUp;
            drawingElement.Stroke = Brushes.Black;
            drawingElement.StrokeThickness = 0.1;

            canvas.Children.Add(drawingElement);
        }
        #endregion

        #region Net
        private void DrawNet(Net net)
        {
            var drawingNet = new DrawingNet(net);
            Canvas.SetLeft(drawingNet, 0);
            Canvas.SetTop(drawingNet, 0);
            drawingNet.Stroke = Brushes.Red;
            drawingNet.MouseEnter += (object sender, MouseEventArgs e) =>
            {
                ((DrawingNet)sender).Stroke = Brushes.OrangeRed;
            };
            drawingNet.MouseLeave += (object sender, MouseEventArgs e) =>
            {
                ((DrawingNet)sender).Stroke = Brushes.Black;
            };
            canvas.Children.Add(drawingNet);
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            DrawGrid();

            schematic = DemoBoard.Generate();

            schematic.Elements.ForEach(DrawElement);
            schematic.Nets.ForEach(DrawNet);

            //// Canvas.LayoutTransformProperty

            //var el1 = new HeaderPins(24);
            //canvas.Children.Add(el1);

            //Canvas.SetLeft(el1, Constants.IN);
            //Canvas.SetTop(el1, Constants.IN);

            //var el2 = new HeaderPins(24);
            //canvas.Children.Add(el2);

            //Canvas.SetLeft(el2, Constants.IN * 10);
            //Canvas.SetTop(el2, Constants.IN);

            //// el1.LayoutTransform = new RotateTransform(45);  // Rotate

            ////var curves = new List<Tuple<Point, Point>>();
            ////for (int i = 0; i != 1; i++)
            ////{
            ////    var p = new Point { X = i * 2.54  + offset.X, Y = 0 + offset.Y };
            ////    //   addPoint(p, 10, new SolidColorBrush(Colors.GreenYellow));
            ////    curves.Add(new Tuple<Point, Point>(qfnPoints[i], new Point { X = 0 + offset.Y, Y = i * 2.54 + offset.X }));
            ////}
            //line1 = new Line { X1 = points1[0].X - 0.5 / 2, X2 = points1[1].X - 0.5 / 2, Y1 = points1[0].Y - 0.5 / 2, Y2 = points1[1].Y - 0.5 / 2 };
            //line1.Stroke = Brushes.Black;
            //line1.StrokeThickness = 0.2;
            //line1.StrokeDashArray = DoubleCollection.Parse("5 5");
            //line2 = new Line { X1 = points1[2].X - 0.5 / 2, X2 = points1[3].X - 0.5 / 2, Y1 = points1[2].Y - 0.5 / 2, Y2 = points1[3].Y - 0.5 / 2 };
            //line2.Stroke = Brushes.Black;
            //line2.StrokeDashArray = DoubleCollection.Parse("5 5");
            //line2.StrokeThickness = 0.2;
            //canvas.Children.Add(line1);
            //canvas.Children.Add(line2);

            //foreach (Point point in points1)
            //{
            //    Rectangle rect = new();
            //    rect.Width = Constants.PointWidth;
            //    rect.Height = Constants.PointWidth;
            //    Canvas.SetTop(rect, point.Y - Constants.PointWidth / 2);
            //    Canvas.SetLeft(rect, point.X - Constants.PointWidth / 2);
            //    rect.Fill = Brushes.White;
            //    rect.Stroke = Brushes.Black;
            //    rect.StrokeThickness = 0;
            //    canvas.Children.Add(rect);
            //    rect.MouseEnter += (object sender, MouseEventArgs e) =>
            //    {
            //        ((Rectangle)sender).Fill = Brushes.OrangeRed;
            //    };
            //    rect.MouseLeave += (object sender, MouseEventArgs e) =>
            //    {
            //        ((Rectangle)sender).Fill = Brushes.White;
            //    };
            //    rect.MouseLeftButtonDown += Rect_MouseLeftButtonDown;
            //    rect.MouseMove += Rect_MouseMove;
            //    rect.DragEnter += Rect_DragEnter;
            //    rect.MouseLeftButtonUp += Rect_MouseLeftButtonUp;
            //    drPoints.Add(rect);
            //}

            PathSegmentCollection path_segment_collection = new();
            // Create a Path to hold the geometry.
            //path = new()
            //{
            //    // Add a PathGeometry.
            //    Data = new PathGeometry
            //    {
            //        Figures = { new PathFigure { Segments = path_segment_collection, StartPoint = points1[0] } }
            //    }
            //};

            //// Add the rest of the points to a PointCollection.
            //PointCollection point_collection = new(points1.Length - 1);
            //for (int i = 1; i < points1.Length; i++)
            //    point_collection.Add(points1[i]);

            //// Make a PolyBezierSegment from the points.
            //PolyBezierSegment bezier_segment = new();
            //bezier_segment.Points = point_collection;

            //// Add the PolyBezierSegment to othe segment collection.
            //path_segment_collection.Add(bezier_segment);

            //path.Stroke = Brushes.LightGreen;
            //path.StrokeThickness = .3;
            //canvas.Children.Add(path);

            ScaleTransform zoomTransform = new();
            zoomTransform.ScaleX = zoomTransform.ScaleY = zoom;
            canvas.RenderTransform = zoomTransform;
        }

        private void Rect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            captured = false;
        }

        private void Rect_MouseMove(object sender, MouseEventArgs e)
        {
            if (captured)
            {
                double x = e.GetPosition(canvas).X;
                double y = e.GetPosition(canvas).Y;
                x_shape += x - x_canvas;
                Canvas.SetLeft(source, x_shape);
                x_canvas = x;
                y_shape += y - y_canvas;
                Canvas.SetTop(source, y_shape);
                y_canvas = y;
                ((DrawingElement)sender)._element.Position.X = x_shape;

                ((DrawingElement)sender)._element.Position.Y = y_shape;
                canvas.UpdateLayout();
                foreach (UIElement child in canvas.Children)
                {
                    //  child.UpdateLayout();
                }
                //var pathFigure = ((PathGeometry)path.Data).Figures.FirstOrDefault();
                //if (pathFigure is not null)
                //{
                //    var segment = (PolyBezierSegment)pathFigure.Segments.FirstOrDefault();

                //    Point[] newPointCollection = drPoints.Select(dp => new Point { X = Canvas.GetLeft(dp) + Constants.PointWidth / 2, Y = Canvas.GetTop(dp) + Constants.PointWidth / 2 }).ToArray();

                //    pathFigure.StartPoint = newPointCollection[0];

                //    line1.X1 = newPointCollection[0].X;
                //    line1.X2 = newPointCollection[1].X;

                //    line1.Y1 = newPointCollection[0].Y;
                //    line1.Y2 = newPointCollection[1].Y;

                //    line2.X1 = newPointCollection[2].X;
                //    line2.X2 = newPointCollection[3].X;

                //    line2.Y1 = newPointCollection[2].Y;
                //    line2.Y2 = newPointCollection[3].Y;

                //    PointCollection point_collection = new(newPointCollection.Length - 1);
                //    for (int i = 1; i < newPointCollection.Length; i++)
                //        point_collection.Add(newPointCollection[i]);
                //    segment.Points = point_collection;
                //}
            }
        }

        private void Rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            source = (UIElement)sender;
            Mouse.Capture(source);
            captured = true;
            x_shape = Canvas.GetLeft(source);
            x_canvas = e.GetPosition(canvas).X;
            y_shape = Canvas.GetTop(source);
            y_canvas = e.GetPosition(canvas).Y;
        }


        private void DrawGrid()
        {
            var brush = new SolidColorBrush(Colors.DarkRed)
            {
                Opacity = 0.5,
            };

            for (double i = 0; i < Constants.IN * 50; i += Constants.IN)
            {
                for (double j = 0; j < Constants.IN * 50; j += Constants.IN)
                {
                    addPoint(new Point { X = i, Y = j }, 0.25, brush);
                }
            }
        }

        private void addPoint(Point p, double w, Brush brush)
        {
            var elipse = new Ellipse
            {
                Width = w,
                Height = w
            };
            Canvas.SetTop(elipse, p.X - w / 2);
            Canvas.SetLeft(elipse, p.Y - w / 2);
            elipse.Fill = brush;

            canvas.Children.Add(elipse);
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsMouseDown)
            {
                foreach (var element in elements)
                {
                    // element.Move(new Point(e.GetPosition(canvas).X, e.GetPosition(canvas).Y));
                }
            }
        }

        private void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            // label1.Content = "X,Y:";
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            schematic = JsonSerializer.Deserialize<Schematic>(File.ReadAllText("schematic.json"));
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //schematic = JsonSerializer.Deserialize<Schematic>(File.ReadAllText("schematic.json"));
            if (schematic is null)
            {
                var lqfpId = Guid.NewGuid();
                var pins = new Pins()
                {

                };
                schematic = new Schematic
                {
                    Elements = new List<Element>
                    {

                    },
                    Nets = new List<Net>
                    {
                        new Net
                        {
                             Name = "Pin1",
                             Connection = new List<ConnectionPoint>{

                             }
                        }
                    }
                };
            }

            File.WriteAllText("schematic.json", JsonSerializer.Serialize(schematic));
        }


        private void Export_Click(object sender, RoutedEventArgs e)
        {
            var document = new SvgDocument();
            document.Children.Add(new SvgText { Text = "test1" });
            document.Write("test1.svg");
        }
        private void CommonCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = false;
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point brushPosition = new Point(e.GetPosition(canvas).X, e.GetPosition(canvas).Y);
            IsMouseDown = true;

            foreach (var element in elements)
            {
                // element.Clicked(brushPosition);
            }
        }

        private void canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoom = e.Delta > 0 ? zoom + 0.1 : zoom - 0.1;
            ScaleTransform zoomTransform = new();
            zoomTransform.ScaleX = zoomTransform.ScaleY = zoom;
            canvas.RenderTransform = zoomTransform;
        }
    }
}
