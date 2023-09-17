using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using System;

namespace CurvePCB.Avalonia.UI.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    public void PointerPressedEvent(object o, PointerPressedEventArgs args)
    {
        var point = args.GetCurrentPoint(canvas);
        var x = point.Position.X;
        var y = point.Position.Y;
        if (point.Properties.IsLeftButtonPressed)
        {
            // left button pressed
        //    canvas.
        }
        if (point.Properties.IsRightButtonPressed)
        {
            // right button pressed
        }
        args.Handled = true;
    }
}

public class LineBoundsDemoControl : Control
{
    static LineBoundsDemoControl()
    {
        AffectsRender<LineBoundsDemoControl>(AngleProperty);
    }

    public LineBoundsDemoControl()
    {
        var timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1 / 60.0);
        timer.Tick += (sender, e) => Angle += Math.PI / 360;
        timer.Start();
    }

    public static readonly StyledProperty<double> AngleProperty =
        AvaloniaProperty.Register<LineBoundsDemoControl, double>(nameof(Angle));

    public double Angle
    {
        get => GetValue(AngleProperty);
        set => SetValue(AngleProperty, value);
    }

    public override void Render(DrawingContext drawingContext)
    {
        var lineLength = Math.Sqrt((100 * 100) + (100 * 100));

        //var diffX = LineBoundsHelper.CalculateAdjSide(Angle, lineLength);
        //var diffY = LineBoundsHelper.CalculateOppSide(Angle, lineLength);


        var p1 = new Point(200, 200);
        var p2 = new Point(p1.X, p1.Y );

        var pen = new Pen(Brushes.Green, 20, lineCap: PenLineCap.Square);
        var boundPen = new Pen(Brushes.Black);

        drawingContext.DrawLine(pen, p1, p2);

    //    drawingContext.DrawRectangle(boundPen, LineBoundsHelper.CalculateBounds(p1, p2, pen));
    }
}