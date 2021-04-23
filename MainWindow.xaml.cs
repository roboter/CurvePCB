﻿using Svg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CurvePCB
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

    public class Pins : IElement
    {
        private bool selected = false;
        Dimention dimention;
        private Canvas canvas;
        private const int pinsOnSide = 16;
        private Point pos;
        private List<Shape> rectangles = new List<Shape>();
        private const double width = 1; // mm
        private const double height = 1; // mm
        private const double next = 2.54; // mm

        public Pins(Canvas canvas)
        {
           
            this.canvas = canvas;
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

        public Point[] Draw(Point offset)
        {
            pos = offset;
            dimention = new Dimention(pos.X, pos.Y, 12, 12);
            var points = new List<Point>();

            for (int i = 0; i != pinsOnSide; i++)
            {
                var rectangle1 = new Rectangle { RadiusX = 5, Width = width, Height = height };
                Canvas.SetLeft(rectangle1, offset.X -width/2);
                points.Add(new Point(offset.Y, offset.X));
                Canvas.SetTop(rectangle1, offset.Y + i * (next) - width / 2);
                rectangle1.Fill = Brushes.Fuchsia;
                rectangle1.Stroke = Brushes.Black;
                canvas.Children.Add(rectangle1);
                rectangles.Add(rectangle1);
            }
            return points.ToArray();
        }

        public void Move(Point point)
        {

        }
    }

    public interface IElement
    {
        void Move(Point point);
        void Clicked(Point position);
        Point[] Draw(Point point);
    }

    public class Element : IElement
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

        public Canvas canvas { get; set; }
        public Element(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public Point[] Draw(Point offset)
        {
            pos = offset;
            dimention = new Dimention(pos.X, pos.Y, 12, 12);
            var points = new List<Point>();
            for (int i = 0; i != pinsOnSide; i++)
            {
                var rectangle1 = new Rectangle { RadiusX = 5, Width = width, Height = height };
                Canvas.SetLeft(rectangle1, offset.X);
                points.Add(new Point(offset.Y, offset.X));
                Canvas.SetTop(rectangle1, offset.Y + i * (next) + start);
                rectangle1.Fill = Brushes.Fuchsia;
                rectangle1.Stroke = Brushes.Black;
                canvas.Children.Add(rectangle1);
                rectangles.Add(rectangle1);

                var rectangle2 = new Rectangle { RadiusX = 5, Width = width, Height = height };
                Canvas.SetLeft(rectangle2, offset.X + 10 + 0.8);
                points.Add(new Point(offset.Y, offset.X));
                Canvas.SetTop(rectangle2, offset.Y + i * (next) + start);
                rectangle2.Fill = Brushes.Fuchsia;
                rectangle2.Stroke = Brushes.Black;
                canvas.Children.Add(rectangle2);
                rectangles.Add(rectangle2);

                var rectangle3 = new Rectangle { RadiusX = 5, Width = height, Height = width };
                Canvas.SetLeft(rectangle3, offset.X + i * (next) + start);
                points.Add(new Point(offset.Y, offset.X));
                Canvas.SetTop(rectangle3, offset.Y + 10 + 0.8);
                rectangle3.Fill = Brushes.Fuchsia;
                rectangle3.Stroke = Brushes.Black;
                canvas.Children.Add(rectangle3);
                rectangles.Add(rectangle3);

                var rectangle4 = new Rectangle { RadiusX = 5, Width = height, Height = width };
                Canvas.SetLeft(rectangle4, offset.X + i * (next) + start);
                points.Add(new Point(offset.Y, offset.X));
                Canvas.SetTop(rectangle4, offset.Y);
                rectangle4.Fill = Brushes.Fuchsia;
                rectangle4.Stroke = Brushes.Black;
                canvas.Children.Add(rectangle4);
                rectangles.Add(rectangle4);
            }

            #region border
            border = new Rectangle { RadiusX = 0.5, Width = 12, Height = 12 }; //mm
            border.ForceCursor = true;
            border.Cursor = Cursors.Hand;
            border.IsMouseDirectlyOverChanged += Border_IsMouseDirectlyOverChanged;
            border.Stroke = Brushes.Pink;
            border.StrokeThickness = 0.1;
            border.StrokeDashArray = DoubleCollection.Parse("4 4");
            Canvas.SetLeft(border, offset.X);

            Canvas.SetTop(border, offset.Y);

            canvas.Children.Add(border);
            #endregion

            #region body
            body = new Rectangle { RadiusX = 2, Width = 10, Height = 10 }; // mm
            body.ForceCursor = true;
            body.Cursor = Cursors.Hand;
            body.IsMouseDirectlyOverChanged += Border_IsMouseDirectlyOverChanged;
            body.Stroke = Brushes.IndianRed;
            body.StrokeThickness = 0.2;
            body.Opacity = 0.5;
            //     body.StrokeDashArray = DoubleCollection.Parse("4 4");
            Canvas.SetLeft(body, offset.X + 1);

            Canvas.SetTop(body, offset.Y + 1);

            canvas.Children.Add(body);
            #endregion


            #region pins
            var pins = new Rectangle { RadiusX = 2, Width = 7.5, Height = 7.5 }; // mm
            pins.ForceCursor = true;
            pins.Cursor = Cursors.Hand;
            pins.IsMouseDirectlyOverChanged += Border_IsMouseDirectlyOverChanged;
            pins.Stroke = Brushes.IndianRed;
            pins.StrokeThickness = 0.01;
            pins.Opacity = 0.5;
            //     body.StrokeDashArray = DoubleCollection.Parse("4 4");
            Canvas.SetLeft(pins, offset.X + (10 - 7.5) / 2 + 1);

            Canvas.SetTop(pins, offset.Y + (10 - 7.5) / 2 + 1);

            canvas.Children.Add(pins);
            #endregion
            return points.ToArray();
        }

        private void Border_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Rectangle && e.NewValue is bool)
            {
                ((Rectangle)sender).Stroke = (bool)e.NewValue ? Brushes.Gray : Brushes.Pink;
            }
            Debug.WriteLine(e.NewValue is bool ? (bool)e.NewValue : null);
        }

        public void Move(Point brushPosition)
        {
            if (selected)
            {
                pos = brushPosition;
                dimention.X = brushPosition.X;
                dimention.Y = brushPosition.Y;

                Canvas.SetLeft(border, brushPosition.X);
                Canvas.SetTop(border, brushPosition.Y);

                int i = 0;
                foreach (var rectangle in rectangles)
                {
                    Canvas.SetLeft(rectangle, brushPosition.X);
                    Canvas.SetTop(rectangle, brushPosition.Y + i * next + start);
                    i++;
                }
            }
            else
            {
                if (dimention.Contains(brushPosition))
                {
                    foreach (var rectangle in rectangles)
                    {
                        rectangle.Fill = Brushes.Black;
                    }
                }
                else
                {
                    foreach (var rectangle in rectangles)
                    {
                        rectangle.Fill = Brushes.Fuchsia;
                    }
                }
            }
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

        Point[] points1 =
          {
                new Point(60, 30),
                new Point(200, 130),
                new Point(100, 150),
                new Point(200, 50),
              //    new Point(20, 20),
            };
        bool captured = false;
        double x_shape, x_canvas, y_shape, y_canvas;
        UIElement source = null;
        Path path;
        Line line1;
        Line line2;


        public MainWindow()
        {
            InitializeComponent();

            DrawGrid();

            var offset = new Point(25, 25);

            //var qfn48 = ;
            elements = new List<IElement> { new Element(canvas), new Pins(canvas) };
            //var qfnPoints = ((IElement)qfn48).Draw(new Point(3, 3));

            foreach (IElement el in elements)
            {
                el.Draw(new Point(IN, IN));
            }


            //var curves = new List<Tuple<Point, Point>>();
            //for (int i = 0; i != 1; i++)
            //{
            //    var p = new Point { X = i * 2.54  + offset.X, Y = 0 + offset.Y };
            //    //   addPoint(p, 10, new SolidColorBrush(Colors.GreenYellow));
            //    curves.Add(new Tuple<Point, Point>(qfnPoints[i], new Point { X = 0 + offset.Y, Y = i * 2.54 + offset.X }));
            //}

            foreach (Point point in points1)
            {
                Rectangle rect = new();
                rect.Width = 3;
                rect.Height = 3;
                Canvas.SetTop(rect, point.Y - 3);
                Canvas.SetLeft(rect, point.X - 3);
                rect.Fill = Brushes.White;
                rect.Stroke = Brushes.Black;
                rect.StrokeThickness = 1;
                canvas.Children.Add(rect);
                rect.MouseLeftButtonDown += Rect_MouseLeftButtonDown;
                rect.MouseMove += Rect_MouseMove;
                rect.DragEnter += Rect_DragEnter;
                rect.MouseLeftButtonUp += Rect_MouseLeftButtonUp;
                drPoints.Add(rect);
            }
            line1 = new Line { X1 = points1[0].X, X2 = points1[1].X, Y1 = points1[0].Y, Y2 = points1[1].Y };
            line1.Stroke = Brushes.Black;
            line1.StrokeThickness = 0.1;
            line1.StrokeDashArray = DoubleCollection.Parse("40 40");
            line2 = new Line { X1 = points1[2].X, X2 = points1[3].X, Y1 = points1[2].Y, Y2 = points1[3].Y };
            line2.Stroke = Brushes.Black;
            line2.StrokeDashArray = DoubleCollection.Parse("4 4");
            line2.StrokeThickness = 0.5;
            canvas.Children.Add(line1);
            canvas.Children.Add(line2);

            PathSegmentCollection path_segment_collection = new();
            // Create a Path to hold the geometry.
            path = new()
            {
                // Add a PathGeometry.
                Data = new PathGeometry
                {
                    Figures = { new PathFigure { Segments = path_segment_collection, StartPoint = points1[0] } }
                }
            };

            // Add the rest of the points to a PointCollection.
            PointCollection point_collection = new(points1.Length - 1);
            for (int i = 1; i < points1.Length; i++)
                point_collection.Add(points1[i]);

            // Make a PolyBezierSegment from the points.
            PolyBezierSegment bezier_segment = new();
            bezier_segment.Points = point_collection;

            // Add the PolyBezierSegment to othe segment collection.
            path_segment_collection.Add(bezier_segment);

            path.Stroke = Brushes.LightGreen;
            path.StrokeThickness = 1;
            canvas.Children.Add(path);

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

                var pathFigure = ((PathGeometry)path.Data).Figures.FirstOrDefault();
                if (pathFigure is not null)
                {
                    var segment = (PolyBezierSegment)pathFigure.Segments.FirstOrDefault();

                    Point[] newPointCollection = drPoints.Select(dp => new Point { X = Canvas.GetLeft(dp), Y = Canvas.GetTop(dp) }).ToArray();

                    pathFigure.StartPoint = newPointCollection[0];

                    line1.X1 = newPointCollection[0].X;
                    line1.X2 = newPointCollection[1].X;

                    line1.Y1 = newPointCollection[0].Y;
                    line1.Y2 = newPointCollection[1].Y;

                    line2.X1 = newPointCollection[2].X;
                    line2.X2 = newPointCollection[3].X;

                    line2.Y1 = newPointCollection[2].Y;
                    line2.Y2 = newPointCollection[3].Y;

                    PointCollection point_collection = new(newPointCollection.Length - 1);
                    for (int i = 1; i < newPointCollection.Length; i++)
                        point_collection.Add(newPointCollection[i]);
                    segment.Points = point_collection;
                }
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

        private void Rect_DragEnter(object sender, DragEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Save()
        {
            var document = new SvgDocument();
            document.Children.Add(new SvgText { Text = "test1" });
            document.Write("test1.svg");
        }

        const double IN = 2.54;
        private void DrawGrid()
        {
            var brush = new SolidColorBrush(Colors.DarkRed)
            {
                Opacity = 0.5,
            };

            for (double i = 0; i < 100; i += IN )
            {
                for (double j = 0; j < 100; j += IN )
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
                    {
                        element.Move(new Point(e.GetPosition(canvas).X, e.GetPosition(canvas).Y));
                    }
                }
            }
        }

        private void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            //  label1.Content = "X,Y:";
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
                {
                    element.Clicked(brushPosition);
                }
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