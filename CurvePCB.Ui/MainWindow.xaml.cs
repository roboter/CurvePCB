﻿using System;
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
                point.Update += InvalidateVisual;
                Points.Add(point);
            }

            IsMouseDirectlyOverChanged += (o, e) =>
            {
                Debug.WriteLine("FancyCurve IsMouseDirectlyOverChanged: ", IsMouseDirectlyOver);
                InvalidateVisual();
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
                drawingContext.DrawGeometry(Brushes.Transparent, new Pen(IsMouseDirectlyOver ? LineColorOver : LineColor, 0.02), path);
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

            var pen = new Pen(Brushes.White, 0.1);
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
            var dotSize = new Size(0.24, 0.24);
            var drawBrush = IsMouseDirectlyOver ? Brushes.Gray : brush;
            drawingContext.DrawRectangle(drawBrush, new Pen(IsMouseDirectlyOver ? Brushes.Green : Brushes.Red, 0.001), new Rect(new Point(point.X - dotSize.Width / 2, point.Y - dotSize.Height / 2), dotSize));
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int Zoom { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            new PCBGrid(canvas);

            var soc = new PCBElement(canvas, new Point(0, 0), new Size { Width = 5, Height = 6 });

            var padWidth = 0.5;

            var padHeight = 1.0;

            var firstLegPos = 0.595 - 0.2;

            var nextLeg = 1.27;

            soc.pads.Add(new Pad { bounds = new Rect { X = firstLegPos, Y = 4.3, Width = padWidth, Height = padHeight } });

            soc.pads.Add(new Pad { bounds = new Rect { X = firstLegPos + nextLeg, Y = 4.3, Width = padWidth, Height = padHeight } });

            soc.pads.Add(new Pad { bounds = new Rect { X = firstLegPos + nextLeg * 2, Y = 4.3, Width = padWidth, Height = padHeight } });

            soc.pads.Add(new Pad { bounds = new Rect { X = firstLegPos + nextLeg * 3, Y = 4.3, Width = padWidth, Height = padHeight } });

            soc.pads.Add(new Pad { bounds = new Rect { X = firstLegPos, Y = 0, Width = padWidth, Height = padHeight } });

            soc.pads.Add(new Pad { bounds = new Rect { X = firstLegPos + nextLeg, Y = 0, Width = padWidth, Height = padHeight } });

            soc.pads.Add(new Pad { bounds = new Rect { X = firstLegPos + nextLeg * 2, Y = 0, Width = padWidth, Height = padHeight } });

            soc.pads.Add(new Pad { bounds = new Rect { X = firstLegPos + nextLeg * 3, Y = 0, Width = padWidth, Height = padHeight } });


            new PCBElement(canvas, new Point(5, 100));

            new FancyCurve(canvas,
             new FancyCurvePointPos[] {
                    new FancyCurvePointPos{ Center =  new Point(Constants.IN, Constants.IN), HandleA = new Point(Constants.IN , Constants.IN) },
                    //With 2 wings
                    new FancyCurvePointPos{ Center =  new Point(Constants.IN * 10, Constants.IN ), // Center 
                    HandleA = new Point(Constants.IN * 10 - Constants.IN * 2, Constants.IN  -  Constants.IN ),
                    HandleB = new Point(Constants.IN * 2 + Constants.IN * 10, Constants.IN * 2 + Constants.IN) },
                    new FancyCurvePointPos{Center = new Point(Constants.IN * 10, Constants.IN * 20), HandleB = new Point(Constants.IN * 20, Constants.IN * 17) }
                }
            );
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Zoom += e.Delta / 100;
            canvas.LayoutTransform = new ScaleTransform(Zoom, Zoom);
            Debug.WriteLine(e.Delta);
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
            var dotSize = new Size(0.01, 0.01);
            var drawBrush = IsMouseDirectlyOver ? Brushes.Gray : Brushes.RosyBrown;
            for (double i = 0; i < 1000 * Constants.IN*10; i += Constants.IN )
                for (double j = 0; j < 1000 * Constants.IN * 10; j += Constants.IN)
                   // drawingContext.draw
                    drawingContext.DrawRectangle(drawBrush, new Pen(IsMouseDirectlyOver ? Brushes.Green : Brushes.Red, 0.001), new Rect(new Point(i, j), dotSize));
        }
    }

    public class Pad
    {
        public Rect bounds { get; set; }
    }

    public class PCBElement : MovableShape
    {
        private Rect bounds;
        public IList<Pad> pads = new List<Pad>();

        public PCBElement(Canvas canvas, Point point) : base(canvas, point)
        {
        }

        public PCBElement(Canvas canvas, Point point, Size bounds) : base(canvas, point)
        {
            this.bounds = new Rect(bounds);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {

            var boundaBrush = new SolidColorBrush
            {
                Opacity = IsMouseDirectlyOver ? 1 : 0.5
            };
            Pen pen1 = new Pen(IsMouseDirectlyOver ? Brushes.Green : Brushes.Red, 0.01);
            pen1.DashStyle = DashStyles.DashDotDot;
            drawingContext.DrawRectangle(boundaBrush, pen1, new Rect { X = point.X, Y = point.Y, Height = bounds.Height, Width = bounds.Width });

            var drawBrush = IsMouseDirectlyOver ? Brushes.Gray : Brushes.RosyBrown;
            var pen = new Pen(IsMouseDirectlyOver ? Brushes.Yellow : Brushes.Salmon, 0.05);
            foreach (var pad in pads)
            {
                drawingContext.DrawRectangle(drawBrush, pen, new Rect { X = pad.bounds.X + point.X, Y = pad.bounds.Y + point.Y, Height = pad.bounds.Height, Width = pad.bounds.Width });
                //drawingContext.DrawRectangle(drawBrush, , new Rect(new Point(point.X + pad.bounds.X, point.Y + pad.bounds.Y), new Point { X = point.X + pad.bounds.Width, Y = point.Y + pad.bounds.Height }));
            }


            //var dotSize = new Size(Constants.IN * 8, Constants.IN * 8);
            //
            //for (double i = 0; i < 100 * Constants.IN; i += Constants.IN * 10)
            //    drawingContext.DrawRectangle(drawBrush, new Pen(IsMouseDirectlyOver ? Brushes.Green : Brushes.Red, 0.1), new Rect(new Point(point.X - dotSize.Width / 2, point.Y + i - dotSize.Height / 2), dotSize));
        }
    }

    //public class Pin
    //{
    //    public double Size { get; set; } = 0.8; // metal part

    //    //between pins 2.54

    //}

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
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    var element = (MovableShape)sender;
                    var position = e.GetPosition(element);

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
