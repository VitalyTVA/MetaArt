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

        this.view.PaintSurface += (o, e) => {
            painter?.PaintSurface(e.Surface);
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

        var (info, _) = Sketch!;
        ShowSketch(info);

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
                this.view.InvalidateSurface();
#else
                this.Dispatcher.Dispatch(this.view.InvalidateSurface);
#endif
            },
            size => {
            },
            feedback => {
                fpsLabel.Text = feedback.DrawTime.TotalMilliseconds.ToString();
            },
            displayDensity: (float)DeviceDisplay.MainDisplayInfo.Density
        );

        painter.SetSize((int)view.CanvasSize.Width, (int)view.CanvasSize.Height);

        this.view.InvalidateSurface();
        this.title.Text = info.Name;
        currentSketch = info;
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        painter?.Dispose();
        painter = null;
    }

    void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        var (_, list) = Sketch!;
        var index = list.IndexOf(currentSketch!) - 1;
        if (index < 0) index = list.Count - 1;
        ShowSketch(list[index]);
    }

    void Button_Clicked_1(System.Object sender, System.EventArgs e)
    {
        var (_, list) = Sketch!;
        var index = list.IndexOf(currentSketch!) + 1;
        if (index >= list.Count) index = list.Count;
        ShowSketch(list[index]);
    }
}

