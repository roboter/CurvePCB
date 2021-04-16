using Svg;
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

    public class QFN
    {
        private const double width = 3.25; // 0.010 in
        private const double height = 0.3;
        private const double start = 2.125;
        private const double next = 0.500;
        private const int pinsOnSide = 12;
        private List<Rectangle> rectangles = new List<Rectangle>();
        private Point pos;
        private bool selected = false;
        Dimention dimention;
        private Rectangle border;

        public Canvas canvas { get; set; }
        public QFN(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public Point[] Draw(Point offset)
        {
            pos = offset;
            dimention = new Dimention(pos.X, pos.Y, 10 * 10, 10 * 10);
            var points = new List<Point>();
            for (int i = 0; i != pinsOnSide; i++)
            {
                var rectangle = new Rectangle { RadiusX = 5, Width = width * 10, Height = height * 10 };
                Canvas.SetLeft(rectangle, offset.X);
                points.Add(new Point(offset.Y, offset.X));
                Canvas.SetTop(rectangle, offset.Y + i * 10);
                rectangle.Fill = Brushes.Fuchsia;
                canvas.Children.Add(rectangle);
                rectangles.Add(rectangle);
            }


            border = new Rectangle { RadiusX = 5, Width = 10 * 10, Height = 10 * 10 };
            border.ForceCursor = true;
            border.Cursor = Cursors.Hand;
            border.IsMouseDirectlyOverChanged += Border_IsMouseDirectlyOverChanged;
            border.Stroke = Brushes.Pink;
            border.StrokeThickness = 4;
            border.StrokeDashArray = DoubleCollection.Parse("4 4");
            Canvas.SetLeft(border, offset.X);

            Canvas.SetTop(border, offset.Y);


            canvas.Children.Add(border);
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

        internal void Move(Point brushPosition)
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
                    Canvas.SetTop(rectangle, brushPosition.Y + i * 10);
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

        internal void Clicked(Point brushPosition)
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

    public class DragabblePoint
    {
        public Point Point { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point currentPoint = new Point();
        private bool IsMouseDown = false;
        private int childcounter = 0;
        private double zoom = 1;
        private List<Point> curvePoints = new List<Point>();
        private List<QFN> qfns = new List<QFN>();
        Point[] points1 =
          {
                new Point(60, 30),
                new Point(200, 130),
                new Point(100, 150),
                new Point(200, 50),
            };
        bool captured = false;
        double x_shape, x_canvas, y_shape, y_canvas;
        UIElement source = null;
        Path path;


        public MainWindow()
        {
            InitializeComponent();

            DrawGrid();

            var offset = new Point(25, 25);

            var qfn48 = new QFN(canvas);
            qfns.Add(qfn48);
            var qfnPoints = qfn48.Draw(new Point(100, 200));

            var curves = new List<Tuple<Point, Point>>();
            for (int i = 0; i != 1; i++)
            {
                var p = new Point { X = i * 2.45 * 10 + offset.X, Y = 0 + offset.Y };
                //   addPoint(p, 10, new SolidColorBrush(Colors.GreenYellow));
                curves.Add(new Tuple<Point, Point>(qfnPoints[i], new Point { X = 0 + offset.Y, Y = i * 2.45 * 10 + offset.X }));
            }

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
                //rect.Name = $"Point{point.X}_{point.Y}";
            }


            PathSegmentCollection path_segment_collection = new();
            // Create a Path to hold the geometry.
            path = new()
            {

                // Add a PathGeometry.
                Data = new PathGeometry
                {
                    Figures = { new PathFigure {
                    Segments = path_segment_collection,
            StartPoint = points1[0] } }
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
            path.StrokeThickness = 5;
            canvas.Children.Add(path);
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
                    var point = points1[1];
                    //point = new Point(x, y);
                    point.X = x;
                    point.Y = y;

                    Point[] newPointCollection =
                    {
                        new Point(),
                        point,
                        point,
                        points1[3]
                    };
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

        const double IN = 2.45 * 5;
        private void DrawGrid()
        {
            var brush = new SolidColorBrush(Colors.DarkRed)
            {
                Opacity = 0.5
            };

            for (double i = 0; i < (int)Height; i += IN)
            {
                for (double j = 0; j < (int)Width; j += IN)
                {
                    addPoint(new Point { X = i, Y = j }, 2, brush);
                }
            }
        }

        private void addPoint(Point p, int w, Brush brush)
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
            Ellipse mybrush = new Ellipse();
            //   label1.Content = "X,Y:"+e.GetPosition(canvas1).X.ToString() + “,” +e.GetPosition(canvas1).Y.ToString();
            mybrush.Width = 10;
            mybrush.Height = 10;
            Point brushPosition = new Point(e.GetPosition(canvas).X, e.GetPosition(canvas).Y);
            if (IsMouseDown)
            {

                //  if (erasorON.IsChecked == true)
                mybrush.Fill = new SolidColorBrush(Colors.Bisque);
                //else
                //    mybrush.Fill = new SolidColorBrush(Color.FromArgb(Convert.ToByte(sliderOp.Value), Convert.ToByte(sliderRed.Value), Convert.ToByte(sliderGreen.Value), Convert.ToByte(sliderBlue.Value)));

                Canvas.SetTop(mybrush, brushPosition.Y);
                Canvas.SetLeft(mybrush, brushPosition.X);
                //        canvas.Children.Add(mybrush);
                childcounter++;
            }
            else
            {
                foreach (var element in qfns)
                {
                    {
                        element.Move(brushPosition);
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

            foreach (var element in qfns)
            {
                {
                    element.Clicked(brushPosition);
                }
            }
        }

        private void canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Debug.WriteLine(e.Delta);
            zoom = e.Delta > 0 ? zoom + 0.1 : zoom - 0.1;

            ScaleTransform zoomTransform = new();
            zoomTransform.ScaleX = zoomTransform.ScaleY = zoom;
            canvas.RenderTransform = zoomTransform;
        }
    }
}
