using MetaArt.Skia;
using Plugin.SimpleAudioPlayer;
using SkiaSharp;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MetaArt.Xamarin;

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
        if (Device.RuntimePlatform == Device.iOS) {
            this.view.HasRenderLoop = true;
        }
        this.view.PaintSurface += (o, e) => {
            if (Sketch == null) return;
            if (painter == null) {
                var info = Sketch!.Current;
                ShowSketch(info);
                e.Surface.Canvas.Clear(SKColors.Black);
                return;
            }

            painter?.PaintSurface(e.Surface);
        };
        
        this.view.EnableTouchEvents = true;
        this.view.Touch += (o, e) => {
            if (e.ActionType == SkiaSharp.Views.Forms.SKTouchAction.Pressed) {
                //painter?.OnMouseOver(e.Location.X, e.Location.Y);
                painter?.OnMouseDown(e.Location.X, e.Location.Y, isLeft: false);
            }
            if (e.ActionType == SkiaSharp.Views.Forms.SKTouchAction.Moved) painter?.OnMouseOver(e.Location.X, e.Location.Y);
            if (e.ActionType == SkiaSharp.Views.Forms.SKTouchAction.Released) {
                painter?.OnMouseUp(e.Location.X, e.Location.Y);
                painter?.OnMouseLeave();
            }
            e.Handled = true;

        };
    }

    SketchDisplayInfo? currentSketch;
    private void ShowSketch(SketchDisplayInfo info)
    {
        Preferences.Set("SketchName", info.Name);
        painter?.Dispose();

        painter = new Painter(
            info.Type,
            info.Parameters,
            () =>
            {
                if (Device.RuntimePlatform == Device.Android) {
                    //this.view.InvalidateSurface();
                    this.Dispatcher.BeginInvokeOnMainThread(this.view.InvalidateSurface);
                } else {
                    //this.view.InvalidateSurface();
                }
            },
            feedback => {
                if (Device.RuntimePlatform == Device.iOS) {
                    fpsLabel.Text = ((int)feedback.DrawTime.TotalMilliseconds).ToString();
                }
            },
            displayDensity: (float)DeviceDisplay.MainDisplayInfo.Density,
            deviceType: DeviceType.Mobile,
            createSoundFile: CreateSoundFile
        );
        painter.SetSize((int)view.CanvasSize.Width, (int)view.CanvasSize.Height);
        painter.Setup();

        this.view.InvalidateSurface();
        this.Dispatcher.BeginInvokeOnMainThread(() => {
            this.title.Text = info.Name;
        });
        currentSketch = info;
    }
    class XamarinSoundFile : SoundFile {
        readonly ISimpleAudioPlayer player;

        public XamarinSoundFile(ISimpleAudioPlayer player) {
            this.player = player;
        }
        public override void play() {
            player.Play();
        }
    }
    static SoundFile CreateSoundFile(Stream stream) {
        ISimpleAudioPlayer player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
        player.Load(stream);
        return new XamarinSoundFile(player);
    } 
    //protected override void OnNavigatedFrom(NavigationEventArgs e) {
    //    base.OnNavigatedFrom(args);

    //}
    protected override bool OnBackButtonPressed() {
        DisposePainter();
        this.Sketch = null;
        return base.OnBackButtonPressed();
    }

    private void DisposePainter() {
        painter?.Dispose();
        painter = null;
    }

    void Button_Clicked(System.Object sender, System.EventArgs e) {
        var list = Sketch!.Lists;
        var index = list.IndexOf(currentSketch!) - 1;
        if (index < 0) index = list.Count - 1;
        DisposePainter();
        this.Sketch = new SketchNavInfo(list[index], this.Sketch!.Lists);
        view.InvalidateSurface();
        //ShowSketch(list[index]);
    }

    void Button_Clicked_1(System.Object sender, System.EventArgs e)
    {
        var list = Sketch!.Lists;
        var index = list.IndexOf(currentSketch!) + 1;
        if (index >= list.Count) index = list.Count;
        DisposePainter();
        this.Sketch = new SketchNavInfo(list[index], this.Sketch!.Lists);
        view.InvalidateSurface();
        //ShowSketch(list[index]);
    }
}

