using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaArt.Skia;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MetaArt.Xamarin {
    public partial class MainPage : ContentPage {
        List<SketchDisplayInfo> sketches;

        public MainPage() {
            InitializeComponent();

            sketches = new[] {
                typeof(ThatButtonAgain.LevelBase),
                typeof(Sketches),
                typeof(SkiaSketch),
            }.SelectMany(x => SketchDisplayInfo.LoadSketches(x.Assembly)).ToList();

            var groups = sketches.GroupBy(x => x.Category, (key, values) => new SketchGroup(key, values)).ToArray();

            if (Device.RuntimePlatform == Device.iOS) {
                this.list.ItemSizingStrategy = ItemSizingStrategy.MeasureFirstItem;
            }
            this.list.ItemsSource = groups;
            this.list.SelectionMode = SelectionMode.Single;

            this.list.SelectionChanged += (o, e) => {

                var page = new SkecthPage() { Sketch = new SketchNavInfo((SketchDisplayInfo)list.SelectedItem, sketches) };
                Navigation.PushAsync(page);
                //Shell.Current.GoToAsync(new ShellNavigationState("sketch"),
                //    new Dictionary<string, object>() { { nameof(SkecthPage.Sketch), new SketchNavInfo((SketchDisplayInfo)list.SelectedItem, sketches) } });

            };
        }

        bool loaded = false;
        protected override void OnAppearing() {
            base.OnAppearing();
            if (loaded) return;
            loaded = true;
            var name = Preferences.Get("SketchName", default(string));
            Preferences.Remove("SketchName");
            if (name != null) {
                var info = sketches.FirstOrDefault(x => x.Name == name);
                if (info != null)
                    Dispatcher.BeginInvokeOnMainThread(() => { list.SelectedItem = info; });
            }
        }
    }
    public class SketchNavInfo {
        public SketchNavInfo(SketchDisplayInfo current, List<SketchDisplayInfo> lists) {
            Current = current;
            Lists = lists;
        }

        public SketchDisplayInfo Current { get; }
        public List<SketchDisplayInfo> Lists { get; }
    }
    public class SketchGroup : List<SketchDisplayInfo> {
        public string Name { get; private set; }

        public SketchGroup(string name, IEnumerable<SketchDisplayInfo> sketches)
            : base(sketches) {
            Name = name;
        }
    }
}

