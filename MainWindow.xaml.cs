using System;
using System.Collections.Generic;
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

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point currentPoint = new Point();
        private bool IsMouseDown = false;
        private int childcounter = 0;
        public MainWindow()
        {
            InitializeComponent();

            DrawGrid();

            //for (int i = 0; i != 10; i++)
            //{
            //    addPoint(new Point { X = i * 2.45, Y = 0 });
            //}

        }
        const double IN = 2.45 * 5;
        private void DrawGrid()
        {
            for (double i = 0; i < (int)Height; i += IN)
            {
                for (double j = 0; j < (int)Width; j += IN)
                {
                    addPoint(new Point { X = i, Y = j }, 2);
                }
            }
        }

        private void addPoint(Point p, int w)
        {
            var elipse = new Ellipse
            {
                Width = w,
                Height = w
            };
            Canvas.SetTop(elipse, p.X - w / 2);
            Canvas.SetLeft(elipse, p.Y - w / 2);
            elipse.Fill = new SolidColorBrush(Colors.DarkRed);

            canvas.Children.Add(elipse);
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Ellipse mybrush = new Ellipse();
            //   label1.Content = "X,Y:"+e.GetPosition(canvas1).X.ToString() + “,” +e.GetPosition(canvas1).Y.ToString();
            mybrush.Width = 10;
            mybrush.Height = 10;

            if (IsMouseDown)
            {
                Point brushPosition = new Point(e.GetPosition(canvas).X, e.GetPosition(canvas).Y);
                //  if (erasorON.IsChecked == true)
                mybrush.Fill = new SolidColorBrush(Colors.Bisque);
                //else
                //    mybrush.Fill = new SolidColorBrush(Color.FromArgb(Convert.ToByte(sliderOp.Value), Convert.ToByte(sliderRed.Value), Convert.ToByte(sliderGreen.Value), Convert.ToByte(sliderBlue.Value)));

                Canvas.SetTop(mybrush, brushPosition.Y);
                Canvas.SetLeft(mybrush, brushPosition.X);
                canvas.Children.Add(mybrush);
                childcounter++;
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


        //private void SizeGroupBox_Checked(object sender, RoutedEventArgs e)
        //{
        //    //if (radioSmall.IsChecked == true)
        //    //    diameter = Convert.ToInt32(2 + sliderSize.Value);
        //    //if (radioMedium.IsChecked == true)
        //    //    diameter = Convert.ToInt32(10 + sliderSize.Value);
        //    //if (radioLarge.IsChecked == true)
        //    //    diameter = Convert.ToInt32(20 + sliderSize.Value);
        //}

        //private void buttonUndo_Click(object sender, RoutedEventArgs e)
        //{
        //    //int count = canvas1.Children.Count;
        //    //canvas1.Children.RemoveAt(count – 1);
        //}

        //private void buttonClear_Click(object sender, RoutedEventArgs e)
        //{
        // //   canvas1.Children.Clear();
        //}

        //private void erasorON_Unchecked(object sender, RoutedEventArgs e)
        //{
        //  //  erasorON.IsChecked = false;
        //}

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = true;
        }

        private void DrawCurve(double tension)
        {
            // Remove any previous curves.
            canvas.Children.Clear();

            // Make a path.
            Point[] points1 =
            {
                new Point(60, 30),
                new Point(200, 130),
                new Point(100, 150),
                new Point(200, 50),
            };
            Path path1 = MakeCurve(points1, tension);
            path1.Stroke = Brushes.LightGreen;
            path1.StrokeThickness = 5;
            canvas.Children.Add(path1);

            foreach (Point point in points1)
            {
                Rectangle rect = new Rectangle();
                rect.Width = 6;
                rect.Height = 6;
                Canvas.SetLeft(rect, point.X - 3);
                Canvas.SetTop(rect, point.Y - 3);
                rect.Fill = Brushes.White;
                rect.Stroke = Brushes.Black;
                rect.StrokeThickness = 1;
                canvas.Children.Add(rect);
            }
        }

        // Make a Path holding a series of Bezier curves.
        // The points parameter includes the points to visit
        // and the control points.
        private Path MakeBezierPath(Point[] points)
        {
            // Create a Path to hold the geometry.
            Path path = new Path();

            // Add a PathGeometry.
            PathGeometry path_geometry = new PathGeometry();
            path.Data = path_geometry;

            // Create a PathFigure.
            PathFigure path_figure = new PathFigure();
            path_geometry.Figures.Add(path_figure);

            // Start at the first point.
            path_figure.StartPoint = points[0];

            // Create a PathSegmentCollection.
            PathSegmentCollection path_segment_collection =
                new PathSegmentCollection();
            path_figure.Segments = path_segment_collection;

            // Add the rest of the points to a PointCollection.
            PointCollection point_collection =
                new PointCollection(points.Length - 1);
            for (int i = 1; i < points.Length; i++)
                point_collection.Add(points[i]);

            // Make a PolyBezierSegment from the points.
            PolyBezierSegment bezier_segment = new PolyBezierSegment();
            bezier_segment.Points = point_collection;

            // Add the PolyBezierSegment to othe segment collection.
            path_segment_collection.Add(bezier_segment);

            return path;
        }

        // Make an array containing Bezier curve points and control points.
        private Point[] MakeCurvePoints(Point[] points, double tension)
        {
            if (points.Length < 2) return null;
            double control_scale = tension / 0.5 * 0.175;

            // Make a list containing the points and
            // appropriate control points.
            List<Point> result_points = new List<Point>();
            result_points.Add(points[0]);

            for (int i = 0; i < points.Length - 1; i++)
            {
                // Get the point and its neighbors.
                Point pt_before = points[Math.Max(i - 1, 0)];
                Point pt = points[i];
                Point pt_after = points[i + 1];
                Point pt_after2 = points[Math.Min(i + 2, points.Length - 1)];

                double dx1 = pt_after.X - pt_before.X;
                double dy1 = pt_after.Y - pt_before.Y;

                Point p1 = points[i];
                Point p4 = pt_after;

                double dx = pt_after.X - pt_before.X;
                double dy = pt_after.Y - pt_before.Y;
                Point p2 = new Point(
                    pt.X + control_scale * dx,
                    pt.Y + control_scale * dy);

                dx = pt_after2.X - pt.X;
                dy = pt_after2.Y - pt.Y;
                Point p3 = new Point(
                    pt_after.X - control_scale * dx,
                    pt_after.Y - control_scale * dy);

                // Save points p2, p3, and p4.
                result_points.Add(p2);
                result_points.Add(p3);
                result_points.Add(p4);
            }

            // Return the points.
            return result_points.ToArray();
        }

        // Make a Bezier curve connecting these points.
        private Path MakeCurve(Point[] points, double tension)
        {
            if (points.Length < 2) return null;
            Point[] result_points = MakeCurvePoints(points, tension);

            // Use the points to create the path.
            return MakeBezierPath(result_points.ToArray());
        }
    }
}
