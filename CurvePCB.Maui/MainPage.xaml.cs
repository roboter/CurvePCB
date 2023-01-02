using System;
namespace CurvePCB.Maui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        void MainGraphicsView_DragInteraction(System.Object sender, Microsoft.Maui.Controls.TouchEventArgs e)
        {
            this.drawable.Move(e.Touches.FirstOrDefault());
            this.MainGraphicsView.Invalidate();
        }
    }

    public class MyGraphicsDrawable : IDrawable

    {
        Point x;
        public void Move(Point x)
        {
            this.x = x;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeColor = Colors.Red;
            canvas.StrokeSize = 6;
            canvas.DrawLine(new Point(0, 0), x);
        }
    }
}

