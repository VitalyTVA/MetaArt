using MetaArt.Skia;

namespace MetaArt.Maui;

[QueryProperty(nameof(Sketch), nameof(Sketch))]
public partial class SkecthPage : ContentPage
{

    SketchNavInfo? sketch;
    public SketchNavInfo? Sketch
    {
        get => sketch;
        set
        {
            sketch = value;
            OnPropertyChanged();
        }
    }

    Painter? painter;

    public SkecthPage()
	{
		InitializeComponent();
#if IOS
        view.HasRenderLoop = true;
#endif
        this.view.PaintSurface += (o, e) => {
#if IOS
            if (view.CanvasSize.IsEmpty)
                return;
#endif
            if (Sketch == null) return;
            if (painter == null) {
                var (info, _) = Sketch!;
                ShowSketch(info);
                e.Surface.Canvas.Clear(SkiaSharp.SKColors.Black);
                return;
            }

            painter?.PaintSurface(e.Surface);
            //e.Surface.Canvas.();
        };
        this.view.EnableTouchEvents = true;
        this.view.Touch += (o, e) => {
            if (e.ActionType == SkiaSharp.Views.Maui.SKTouchAction.Pressed)
            {
                //painter?.OnMouseOver(e.Location.X, e.Location.Y);
                painter?.OnMouseDown(e.Location.X, e.Location.Y, isLeft: false);
            }
            if (e.ActionType == SkiaSharp.Views.Maui.SKTouchAction.Moved) painter?.OnMouseOver(e.Location.X, e.Location.Y);
            if (e.ActionType == SkiaSharp.Views.Maui.SKTouchAction.Released)
            {
                painter?.OnMouseUp(e.Location.X, e.Location.Y);
                painter?.OnMouseLeave();
            }
            e.Handled = true;

        };
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        //        painter?.Dispose();

        //var (info, _) = Sketch!;
        //ShowSketch(info);

    }

    private void DisposePainter() {
        painter?.Dispose();
        painter = null;
    }

    SketchDisplayInfo? currentSketch;
    private void ShowSketch(SketchDisplayInfo info)
    {
        Preferences.Default.Set("SketchName", info.Name);
        painter?.Dispose();

        painter = new Painter(
            info.Type,
            () =>
            {
#if ANDROID
                this.Dispatcher.Dispatch(this.view.InvalidateSurface);
#elif IOS
                //this.view.InvalidateSurface();
#endif
            },
            feedback => {
#if IOS
                fpsLabel.Text = ((int)feedback.DrawTime.TotalMilliseconds).ToString();
#endif
            },
            displayDensity: (float)DeviceDisplay.MainDisplayInfo.Density,
            deviceType: DeviceType.Mobile
        );

        painter.SetSize((int)view.CanvasSize.Width, (int)view.CanvasSize.Height);
        painter.Setup();

        this.view.InvalidateSurface();
        this.Dispatcher.Dispatch(() => {
            this.title.Text = info.Name;
        });
        currentSketch = info;
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        DisposePainter();
        this.Sketch = null;
    }

    void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        var (_, list) = Sketch!;
        var index = list.IndexOf(currentSketch!) - 1;
        if (index < 0) index = list.Count - 1;
        DisposePainter();
        this.Sketch = new SketchNavInfo(list[index], this.Sketch!.Lists);
        view.InvalidateSurface();
        //ShowSketch(list[index]);
    }

    void Button_Clicked_1(System.Object sender, System.EventArgs e)
    {
        var (_, list) = Sketch!;
        var index = list.IndexOf(currentSketch!) + 1;
        if (index >= list.Count) index = list.Count;
        DisposePainter();
        this.Sketch = new SketchNavInfo(list[index], this.Sketch!.Lists);
        view.InvalidateSurface();
        //ShowSketch(list[index]);
    }
}

