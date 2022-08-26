using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CurvePCB.Ui
{
    public class FancyCurvePointPos
    {
        public Point Center { get; set; }

        public Point? HandleA { get; set; }

        public Point? HandleB { get; set; }
    }

    public class FancyCurve : Shape
    {
        private readonly EllipseGeometry ellipse;

        public bool Selected { get; set; }

        protected override Geometry DefiningGeometry => ellipse;

        public Brush LineColor { get; set; } = Brushes.Yellow;

        public Brush LineColorOver { get; set; } = Brushes.Honeydew;

        public List<FancyCurvePoint> Points { get; set; } = new List<FancyCurvePoint>();

        public FancyCurve(Canvas canvas, IEnumerable<FancyCurvePointPos> pointsPos)
        {
            ellipse = new EllipseGeometry();

            canvas.Children.Add(this);

            foreach (var pointPos in pointsPos)
            {
                var point = new FancyCurvePoint(canvas, pointPos.Center, pointPos.HandleA, pointPos.HandleB);
                point.Update += () => { this.InvalidateVisual(); };
                Points.Add(point);
            }

            this.IsMouseDirectlyOverChanged += (o, e) =>
            {
                Debug.WriteLine("FancyCurve IsMouseDirectlyOverChanged: ", IsMouseDirectlyOver);
                this.InvalidateVisual();
            };
        }

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
            var points = GetPoints();
            if (Points.Count > 1)
            {
                var start = Points.First();
                foreach (var point in Points.Skip(1))
                {
                    start = point;
                }
                var path = new PathGeometry();

                var path_figure = new PathFigure { IsFilled = false, };

                path.Figures.Add(path_figure);

                // Start at the first point.
                path_figure.StartPoint = points[0];

                // Create a PathSegmentCollection.
                var path_segment_collection = new PathSegmentCollection { };
                path_figure.Segments = path_segment_collection;

                // Add the rest of the points to a PointCollection.
                var point_collection = new PointCollection(points.Count() - 1);
                for (int i = 1; i < points.Count(); i++)
                {
                    point_collection.Add(points[i]);
                }
                // Make a PolyBezierSegment from the points.
                var bezier_segment = new PolyBezierSegment { Points = point_collection, IsSmoothJoin = true, IsStroked = true, };

                // Add the PolyBezierSegment to othe segment collection.
                path_segment_collection.Add(bezier_segment);

                //new Pen(IsMouseDirectlyOver ? Brushes.bl: Brushes.Yellow, IsMouseDirectlyOver ? 2 : 1)
                drawingContext.DrawGeometry(Brushes.Transparent, new Pen(IsMouseDirectlyOver ? LineColorOver : LineColor, 2), path);
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
            canvas.Children.Add(this);
            ellipse = new EllipseGeometry();
            Center = new FancyPoint(canvas, center)
            {
                brush = IsMouseOver ? Brushes.Red : Brushes.DarkSlateBlue
            };
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
                    HandleB.InvalidateVisual();
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
            // Debug.WriteLine("FancyCurvePoint Render");
            base.OnRender(drawingContext);

            var pen = new Pen(Brushes.White, 1);
            if (HandleA != null)
            {
                drawingContext.DrawLine(pen, Center.point, HandleA.point);
            }
            if (HandleB != null)
            {
                drawingContext.DrawLine(pen, Center.point, HandleB.point);
            }
        }
    }

    public class FancyPoint : Shape
    {
        public Canvas canvas { get; set; }

        public Point? dragStart;

        private readonly EllipseGeometry ellipse;

        public Point point { get; set; }

        public Brush brush { get; set; } = Brushes.OrangeRed;

        protected override Geometry DefiningGeometry => ellipse;

        public Action<Point, Point> ChangePosition { get; internal set; }

        public Action Update { get; internal set; }

        public FancyPoint(Canvas canvas, Point point)
        {
            this.canvas = canvas;
            ellipse = new EllipseGeometry();
            this.point = point;

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

            //Update?.Invoke();
            //this.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //    var br = new SolidColorBrush();
            //  //  br.Opacity = IsMouseDirectlyOver ? 1 : 0.5;
            var dotSize = new Size(Constants.IN * 4, Constants.IN * 4);
            var drawBrush = IsMouseDirectlyOver ? Brushes.Gray : brush;
            drawingContext.DrawRectangle(drawBrush, new Pen(IsMouseDirectlyOver ? Brushes.Green : Brushes.Red, 0.1), new Rect(new Point(point.X - dotSize.Width / 2, point.Y - dotSize.Height / 2), dotSize));
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

            new PCBGrid(canvas);

            new PCBElement(canvas, new Point(20, 100));

            new FancyCurve(canvas,
             new FancyCurvePointPos[] {
                    new FancyCurvePointPos{ Center =  new Point(Constants.IN, Constants.IN), HandleA = new Point(Constants.IN * 20, Constants.IN) },
                    //With 2 wings
                    new FancyCurvePointPos{ Center =  new Point(Constants.IN * 100, Constants.IN * 30), // Center 
                    HandleA = new Point(Constants.IN * 100 - Constants.IN * 20, Constants.IN * 30 -  Constants.IN * 20),
                    HandleB = new Point(Constants.IN * 20 + Constants.IN * 100, Constants.IN * 20 + Constants.IN * 30) },
                    new FancyCurvePointPos{Center = new Point(Constants.IN * 100, Constants.IN * 200), HandleB = new Point(Constants.IN * 200, Constants.IN * 170) }
                }
            );
        }
    }

    public class PCBGrid : Shape
    {
        private readonly EllipseGeometry ellipse;

        public PCBGrid(Canvas canvas)
        {
            ellipse = new EllipseGeometry();
            this.canvas = canvas;
            canvas.Children.Add(this);
        }

        public Canvas canvas { get; set; }

        protected override Geometry DefiningGeometry => ellipse;

        protected override void OnRender(DrawingContext drawingContext)
        {
            //    var br = new SolidColorBrush();
            //  //  br.Opacity = IsMouseDirectlyOver ? 1 : 0.5;
            var dotSize = new Size(1, 1);
            var drawBrush = IsMouseDirectlyOver ? Brushes.Gray : Brushes.RosyBrown;
            for (double i = 0; i < 1000 * Constants.IN; i = i + Constants.IN * 10)
                for (double j = 0; j < 1000 * Constants.IN; j = j + Constants.IN * 10)
                    drawingContext.DrawRectangle(drawBrush, new Pen(IsMouseDirectlyOver ? Brushes.Green : Brushes.Red, 0.1), new Rect(new Point(i, j), dotSize));
        }
    }

    public class PCBElement : MovableShape
    {
        public PCBElement(Canvas canvas, Point point) : base(canvas, point)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //    var br = new SolidColorBrush();
            //  //  br.Opacity = IsMouseDirectlyOver ? 1 : 0.5;
            var dotSize = new Size(Constants.IN * 8, Constants.IN * 8);
            var drawBrush = IsMouseDirectlyOver ? Brushes.Gray : Brushes.RosyBrown;
            for (double i = 0; i < 100 * Constants.IN; i = i + Constants.IN * 10)
                drawingContext.DrawRectangle(drawBrush, new Pen(IsMouseDirectlyOver ? Brushes.Green : Brushes.Red, 0.1), new Rect(new Point(point.X - dotSize.Width / 2, point.Y + i - dotSize.Height / 2), dotSize));
        }
    }

    public class Pin
    {
        public double Size { get; set; } = 0.8; // metal part

        //between pins 2.54

    }

    public class MovableShape : Shape
    {
        public Canvas Canvas { get; set; }

        protected override Geometry DefiningGeometry => ellipse;

        private readonly EllipseGeometry ellipse;

        public Point point { get; set; }

        public Point dragStart;

        public MovableShape(Canvas canvas, Point point)
        {
            this.Canvas = canvas;
            ellipse = new EllipseGeometry();
            this.point = point;
            canvas.Children.Add(this);

            this.MouseDown += (object sender, MouseButtonEventArgs e) =>
            {
                var element = (MovableShape)sender;
                Point c = e.GetPosition(element);
                Point elementAt = e.GetPosition(element);
                c.Offset(-element.point.X, -element.point.Y);

                dragStart = c;
                Debug.WriteLine($"Point:{element.point.X} {element.point.Y} elementAt: {elementAt.X} {elementAt.Y} ");
                element.CaptureMouse();
                this.InvalidateVisual();
            };

            this.MouseMove += (object sender, MouseEventArgs e) =>
            {
                if ( e.LeftButton == MouseButtonState.Pressed)
                {
                    var element = (MovableShape)sender;
                    var position =  e.GetPosition(element);

                    position.Offset(-dragStart.X, -dragStart.Y);
                    element.point = position;
                    this.InvalidateVisual();
                }
            };

            this.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                var element = (Shape)sender;
                element.ReleaseMouseCapture();
                this.InvalidateVisual();
            };
        }
    }
}
