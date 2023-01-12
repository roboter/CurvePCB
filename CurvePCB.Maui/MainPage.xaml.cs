using System;
using System.Diagnostics;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace CurvePCB.Maui;

public partial class MainPage : ContentPage
{

    public MainPage()
    {
        InitializeComponent();
        new Timer((e) =>
        {

         //   this.MainGraphicsView.Invalidate();
        }, null, 0, 1000);
    }

    void MainGraphicsView_DragInteraction(System.Object sender, Microsoft.Maui.Controls.TouchEventArgs e)
    {
        this.drawable.Move(e.Touches.FirstOrDefault());
        this.MainGraphicsView.Invalidate();
    }

    void Slider_ValueChanged(System.Object sender, Microsoft.Maui.Controls.ValueChangedEventArgs e)
    {
        this.drawable.Angle = e.NewValue;
    }

    void MainGraphicsView_StartInteraction(System.Object sender, Microsoft.Maui.Controls.TouchEventArgs e)
    {
        Debug.WriteLine("Start");
        this.drawable.Start(e.Touches.FirstOrDefault());
    }

    void MainGraphicsView_EndInteraction(System.Object sender, Microsoft.Maui.Controls.TouchEventArgs e)
    {
        Debug.WriteLine("End");
        this.drawable.Stop();
    }
}

public class MyGraphicsDrawable : IDrawable
{
    //Point x;
    Boolean isSelected = false;
    private Element? selectedElement;
    private int rotate = 0;
    private List<Element> elements = new List<Element>();
    private Timer t;

    public MyGraphicsDrawable()
    {
        elements.AddRange(new[] {
            new Element(new Point { X = 10, Y = 10 }, new Point { X = 100, Y = 100 }, 45) ,
            new Element(new Point { X = 25, Y = 25 }, new Point { X = 40, Y = 50 },15)
        });
       t =  new Timer((e) =>
       {
           // if (selectedElement is not null)
           {
               rotate++;
               if (rotate > 360) rotate = 0;
               elements[0].Rotate(rotate);
               Debug.WriteLine($"Rotate: {rotate}");

           }

       }, null, 0, 1000);
    }

    public void Start(Point x)
    {
        selectedElement = elements.Find(e => e.Contains(x));
        if (selectedElement is not null)
        {
            isSelected = true;
        }
    }

    public void Stop()
    {
        //  isSelected = false;
    }

    public void Move(Point x)
    {
        //  this.x = x;
        if (selectedElement is not null)
        {
            selectedElement.MoveTo(x);
        }
    }

    public double Angle { get; set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.StrokeColor = Colors.Red;
        canvas.StrokeSize = 1;

        canvas.DrawRoundedRectangle(20, 20, 200, 200, 15);

        elements.ForEach(e => e.Draw(canvas));
        canvas.DrawRoundedRectangle(22, 22, 196, 196, 15);
    }
}

public class Element
{
    private PointF start;
    private PointF end;
    private PointF size;
    public double degree { get; set; } = 10;

    public Element(Point s, Point e, double d = 0)
    {
        start = s;
        size = e;
        end = new Point(s.X + size.X, s.Y + size.Y);
        degree = d;
    }

    public void MoveTo(Point x)
    {
        start = x;
        end = new Point(start.X + size.X, start.Y + size.Y);
    }

    public void Rotate(int angle)
    {
        degree = angle;
    }

    private Point TransformAngle(Point i)
    {
        double angle = degree * (Math.PI / 180); // convert to radians
        double x = i.X;
        double y = i.Y;

        double newX = x * Math.Cos(angle) - y * Math.Sin(angle);
        double newY = x * Math.Sin(angle) + y * Math.Cos(angle);

        return new Point(newX, newY);
    }

    public void Draw(ICanvas canvas)
    {
        canvas.StrokeColor = Colors.Red;
        canvas.StrokeSize = 1;
        canvas.SaveState();
        canvas.Rotate((float)degree, start.X + size.X/2, start.Y + size.Y/2);
        canvas.DrawRectangle(start.X, start.Y, size.X, size.Y);

        DrawPin(canvas);
        canvas.RestoreState();
        canvas.StrokeColor = Colors.Blue;
        PointF rotaredStart = TransformAngle(start);
        PointF rotaredSize = TransformAngle(size);


    }

    public void DrawPin(ICanvas canvas)
    {
        canvas.StrokeColor = Colors.DarkGray;
        canvas.StrokeSize = 1;

        canvas.DrawRectangle(start.X + size.X, start.Y +10, 10, 5);

    }

    public bool Contains(Point p) => p.X >= start.X && p.Y >= start.Y && p.X <= end.X && p.Y <= end.Y;
}