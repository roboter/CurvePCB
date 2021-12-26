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

namespace CurvePCB.Ui
{
    public class FancyCurve : Shape
    {

        public FancyCurve(Canvas canvas, List<FancyCurvePoint> Points)
        {
            ellipse = new EllipseGeometry();


            canvas.Children.Add(this);
            this.IsMouseDirectlyOverChanged += (o, e) =>
            {
                this.InvalidateVisual();
            };

            this.MouseMove += (o, e) =>
            {
                this.InvalidateVisual();
            };

            this.Points = Points;

            foreach (var point in Points)
            {
                canvas.Children.Add(point);
                point.Update += () =>
                {
                    this.InvalidateVisual();
                };
            }

        }

        private readonly EllipseGeometry ellipse;

        public bool Selected { get; set; }

        protected override Geometry DefiningGeometry => ellipse;

        public List<FancyCurvePoint> Points { get; set; } = new List<FancyCurvePoint>();

        public List<Point> GetPoints()
        {
            var list = new List<Point> { };
            foreach (var point in Points)
            {
                list.AddRange(point.Points());
            }

            return list;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            //foreach (var point in Points)
            //{
            //    point.OnRender(drawingContext);
            //}

            //var br = new SolidColorBrush();
            //br.Opacity = 1;
            //drawingContext.DrawRectangle(Brushes.OrangeRed, new Pen(Stroke, 0.1), new Rect(0, 0, Constants.IN * 2, Constants.IN * 2));
            var points = GetPoints();
            if (Points.Count > 1)
            {
                var start = Points.First();
                foreach (var point in Points.Skip(1))
                {
                    //   drawingContext.DrawLine(new Pen(Brushes.Aqua, 2), start.Point, point.Point);
                    start = point;
                }
                var path = new PathGeometry();

                var path_figure = new PathFigure();
                path.Figures.Add(path_figure);

                // Start at the first point.
                path_figure.StartPoint = points[0];

                // Create a PathSegmentCollection.
                var path_segment_collection = new PathSegmentCollection();
                path_figure.Segments = path_segment_collection;

                // Add the rest of the points to a PointCollection.
                var point_collection = new PointCollection(points.Count() - 1);
                for (int i = 1; i < points.Count(); i++)
                {
                    point_collection.Add(points[i]);
                }
                // Make a PolyBezierSegment from the points.
                var bezier_segment = new PolyBezierSegment
                {
                    Points = point_collection
                };

                // Add the PolyBezierSegment to othe segment collection.
                path_segment_collection.Add(bezier_segment);

                drawingContext.DrawGeometry(Brushes.Transparent, new Pen(Brushes.Yellow, IsMouseDirectlyOver ? 2 : 1), path);


            }
        }
    }

    public class FancyCurvePoint : Shape
    {
        //  private readonly Canvas canvas;
        public Point? dragStart { get; set; }

        public Action Update { get; internal set; }

        public FancyCurvePoint(Canvas canvas, Point center, Point? handleA = null, Point? handleB = null)
        {
            //  canvas.Children.Add(this);
            ellipse = new EllipseGeometry();
            Center = new FancyPoint(canvas, center);
            Center.brush = Brushes.DarkSlateBlue;
            canvas.Children.Add(Center);
            this.IsMouseDirectlyOverChanged += (o, e) =>
            {
                Update?.Invoke();
                this.InvalidateVisual();
            };

            Center.ChangePosition += (Point old, Point p) =>
            {
                this.InvalidateVisual();
                if (HandleA != null)
                {
                    HandleA.point = new Point(HandleA.point.X + (p.X - old.X), HandleA.point.Y + (p.Y - old.Y));
                    Update?.Invoke();
                    HandleA.InvalidateVisual();
                    this.InvalidateVisual();
                }

                if (HandleB != null)
                {
                    HandleB.point = new Point(HandleB.point.X + p.X - old.X, HandleB.point.Y + p.Y - old.Y);
                    Update?.Invoke();
                    //HandleB.InvalidateVisual();
                    this.InvalidateVisual();

                }
            };

            if (handleA.HasValue)
            {
                HandleA = new FancyPoint(canvas, handleA.Value);
                canvas.Children.Add(HandleA);
                HandleA.Update += () =>
                {
                    Update?.Invoke();
                    this.InvalidateVisual();
                };

            }

            if (handleB.HasValue)
            {
                HandleB = new FancyPoint(canvas, handleB.Value);
                canvas.Children.Add(HandleB);
                HandleB.Update += () =>
                {
                    Update?.Invoke();
                    this.InvalidateVisual();
                };
            }

            if (HandleA != null && HandleB != null)
            {
                HandleA.ChangePosition += (Point old, Point p) =>
                {
                    HandleB.point = new Point(Center.point.X + (Center.point.X - p.X), Center.point.Y + (Center.point.Y - p.Y));
                    HandleB.InvalidateVisual();
                };
                HandleB.ChangePosition += (Point old, Point p) =>
                {
                    HandleA.point = new Point(Center.point.X + (Center.point.X - p.X), Center.point.Y + (Center.point.Y - p.Y));
                    HandleA.InvalidateVisual();
                };
            }

        }

        private readonly EllipseGeometry ellipse;

        protected override Geometry DefiningGeometry => ellipse;
        public bool Linked { get; set; }

        public FancyPoint Center { get; set; }

        //   public Shape CenterPoint = new Rectangle { Width = 30, Height = 10, Fill = Brushes.Fuchsia };

        public FancyPoint? HandleA { get; set; }

        public FancyPoint? HandleB { get; set; }

        public List<Point> Points()
        {
            var list = new List<Point> { };
            if (HandleA != null && HandleB == null)
            {
                list.Add(Center.point);
            }

            if (HandleA != null)
            {
                list.Add(HandleA.point);
            }
            if (HandleA != null && HandleB != null)
            {
                list.Add(Center.point);
            }

            if (HandleB != null)
            {
                list.Add(HandleB.point);
            }
            if (HandleA == null && HandleB != null)
            {
                list.Add(Center.point);
            }

            return list;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Debug.WriteLine("FancyCurvePoint Render");
            base.OnRender(drawingContext);
            var br = new SolidColorBrush();
            br.Color = Colors.LightSeaGreen;
            var dotSize = new Size(Constants.IN * 4, Constants.IN * 4);
            br.Opacity = IsMouseDirectlyOver ? 1 : 0.1;

            var pen = IsMouseDirectlyOver ? new Pen(Brushes.White, 3) : new Pen(Brushes.White, 1);
            if (HandleA != null)
            {
                drawingContext.DrawLine(pen, Center.point, HandleA.point);
            }
            if (HandleB != null)
            {
                drawingContext.DrawLine(pen, Center.point, HandleB.point);
            }
//            drawingContext.DrawRectangle(br, new Pen(Stroke, 15), new Rect(Center.point, dotSize));
        }
    }

    public class FancyPoint : Shape
    {
        public Canvas canvas { get; set; }

        public Point? dragStart;

        public FancyPoint(Canvas canvas, Point point)
        {
            this.canvas = canvas;
            ellipse = new EllipseGeometry();
            this.point = point;

            this.IsMouseDirectlyOverChanged += (object sender, DependencyPropertyChangedEventArgs e) =>
            {
                this.InvalidateVisual();
            };

            this.MouseDown += (object sender, MouseButtonEventArgs e) =>
            {
                var element = (Shape)sender;
                dragStart = e.GetPosition(element);
                element.CaptureMouse();
                this.InvalidateVisual();
            };
            this.MouseMove += (object sender, MouseEventArgs e) =>
            {
                if (dragStart != null && e.LeftButton == MouseButtonState.Pressed)
                {
                    var element = (FancyPoint)sender;
                    var position = e.GetPosition(element);
                    ChangePosition?.Invoke(element.point, position);
                    element.point = position;
                    this.InvalidateVisual();
                    Update?.Invoke();

                }
            };

            this.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                var element = (Shape)sender;
                dragStart = null;
                element.ReleaseMouseCapture();
                this.InvalidateVisual();
            };
        }

        private readonly EllipseGeometry ellipse;

        public Point point { get; set; }

        public Brush brush { get; set; } = Brushes.OrangeRed;

        protected override Geometry DefiningGeometry => ellipse;

        public Action<Point, Point> ChangePosition { get; internal set; }
        public Action Update { get; internal set; }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var br = new SolidColorBrush();
            br.Opacity = IsMouseDirectlyOver ? 1 : 0.5;
            var dotSize = new Size(Constants.IN * 4, Constants.IN * 4);

            drawingContext.DrawRectangle(brush, new Pen(Stroke, 0.1), new Rect(point, dotSize));
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            new FancyCurve(canvas,
             new List<FancyCurvePoint> {
                    new FancyCurvePoint(canvas, center: new Point(Constants.IN, Constants.IN), handleA: new Point(Constants.IN * 20, Constants.IN)),
                    new FancyCurvePoint(canvas, center: new Point(Constants.IN * 100, Constants.IN * 30), handleA: new Point(Constants.IN * 20, Constants.IN * 20), handleB: new Point(Constants.IN * 21, Constants.IN * 25)),
                    new FancyCurvePoint(canvas, center: new Point(Constants.IN * 100, Constants.IN * 200), handleB: new Point(Constants.IN * 200, Constants.IN * 170))
                }
            );
        }
    }
}
