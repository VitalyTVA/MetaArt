
using System.Diagnostics;
using MetaArt.Skia;
using SkiaSharp;

namespace MetaArt.Maui;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();

        var sketches = new[] {
                typeof(Sketches),
                typeof(SkiaSketch),
            }.SelectMany(x => SketchDisplayInfo.LoadSketches(x.Assembly)).ToList();

        this.list.ItemsSource = sketches;

        var painter = default(Painter);

        this.list.ItemSelected += (o, e) => {
            painter?.Dispose();
            var info = (SketchDisplayInfo)list.SelectedItem;
            painter = new Painter(
                info.Type,
                () =>
                {
                    this.Dispatcher.Dispatch(this.view.InvalidateSurface);
                    //this.view.InvalidateSurface();
                }, size => { }, feedback => { }
            );
            this.view.InvalidateSurface();
            count = 0;
        };


        this.view.PaintSurface += (o, e) => {
            painter?.PaintSurface(e.Surface);

            /*
            using (SKPaint paint = new SKPaint())
            {
                paint.Color = SKColors.Blue;
                paint.IsAntialias = true;
                paint.StrokeWidth = 15;
                paint.Style = SKPaintStyle.Stroke;
                e.Surface.Canvas.DrawCircle(200 + count++, 200, 100, paint); //arguments are x position, y position, radius, and paint
            }
            this.Dispatcher.Dispatch(this.view.InvalidateSurface);
            */

        };
        this.view.EnableTouchEvents = true;
        this.view.Touch += (o, e) => {
            if (e.ActionType == SkiaSharp.Views.Maui.SKTouchAction.Pressed) painter?.OnMouseDown(e.Location.X, e.Location.Y, isLeft: false);
            if (e.ActionType == SkiaSharp.Views.Maui.SKTouchAction.Moved) painter?.OnMouseOver(e.Location.X, e.Location.Y);
            if (e.ActionType == SkiaSharp.Views.Maui.SKTouchAction.Moved)
            {
                painter?.OnMouseUp(e.Location.X, e.Location.Y);
                painter?.OnMouseLeave();
            }
            e.Handled = true;
            
        };

    }

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;
	}
}


