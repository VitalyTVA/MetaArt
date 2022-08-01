
using System.Diagnostics;
using MetaArt.Skia;
using SkiaSharp;

namespace MetaArt.Maui;

public partial class MainPage : ContentPage
{
    List<SketchDisplayInfo> sketches;
	public MainPage()
	{
		InitializeComponent();

        sketches = new[] {
                typeof(ThatButtonAgain.Level1),
                typeof(Sketches),
                typeof(SkiaSketch),
            }.SelectMany(x => SketchDisplayInfo.LoadSketches(x.Assembly)).ToList();

        var groups = sketches.GroupBy(x => x.Category, (key, values) => new SketchGroup(key, values)).ToArray();

        this.list.ItemsSource = groups;
        this.list.SelectionMode = SelectionMode.Single;

        this.list.SelectionChanged += (o, e) => {
            Shell.Current.GoToAsync(new ShellNavigationState("sketch"),
                new Dictionary<string, object>() { { nameof(SkecthPage.Sketch), new SketchNavInfo((SketchDisplayInfo)list.SelectedItem, sketches) } });

        }; 

    }
    bool loaded = false;

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        if (loaded) return;
        loaded = true;
        base.OnNavigatedTo(args);
        var name = Preferences.Get("SketchName", default(string));
        Preferences.Remove("SketchName");
        if (name != null)
        {
            var info = sketches.FirstOrDefault(x => x.Name == name);
            if (info != null)
                list.SelectedItem = info;
        }

    }
}
public record SketchNavInfo(SketchDisplayInfo Current,  List<SketchDisplayInfo> Lists);
public class SketchGroup : List<SketchDisplayInfo>
{
    public string Name { get; private set; }

    public SketchGroup(string name, IEnumerable<SketchDisplayInfo> sketches)
        : base(sketches)
    {
        Name = name;
    }
}
