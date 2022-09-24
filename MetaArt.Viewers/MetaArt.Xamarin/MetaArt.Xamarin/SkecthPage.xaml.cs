using MetaArt.Skia;
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
            if(Sketch != null)
                ShowSketch(Sketch!.Current);
        }
    }


    public SkecthPage()
	{
		InitializeComponent();
        view.Feedback += (o, feedback) => fpsLabel.Text = ((int)feedback.DrawTime.TotalMilliseconds).ToString();
    }

    void ShowSketch(SketchDisplayInfo info)
    {
        Preferences.Set("SketchName", info.Name);
        view.SetSketch(info.CreateSketch());
        title.Text = info.Name;
    }

    //protected override void OnNavigatedFrom(NavigationEventArgs e) {
    //    base.OnNavigatedFrom(args);

    //}
    protected override bool OnBackButtonPressed() {
        view.ClearSkecth();
        return base.OnBackButtonPressed();
    }



    void Button_Clicked(System.Object sender, System.EventArgs e) {
        var list = Sketch!.Lists;
        var index = list.IndexOf(Sketch.Current!) - 1;
        if (index < 0) index = list.Count - 1;
        view.ClearSkecth();
        this.Sketch = new SketchNavInfo(list[index], this.Sketch!.Lists);
        ShowSketch(Sketch.Current);
    }

    void Button_Clicked_1(System.Object sender, System.EventArgs e)
    {
        var list = Sketch!.Lists;
        var index = list.IndexOf(Sketch.Current!) + 1;
        if (index >= list.Count) index = list.Count;
        view.ClearSkecth();
        this.Sketch = new SketchNavInfo(list[index], this.Sketch!.Lists);
        ShowSketch(Sketch.Current);
    }
}

