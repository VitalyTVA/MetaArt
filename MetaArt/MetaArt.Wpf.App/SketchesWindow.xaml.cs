using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MetaArt.Wpf {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SketchesWindow : Window {
        public SketchesWindow() {
            InitializeComponent();

            var path = @"c:\Work\github\MetaArt\MetaArt\MetaArt.Sketches\";
            var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories)
                .Select(x => x.Replace(path, null))
                .ToArray();

            var types = Assembly.LoadFile(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "MetaArt.Sketches.dll"))
                .GetTypes()
                .Where(x => typeof(SketchBase).IsAssignableFrom(x))
                .Select(x => {
                    var path = files.FirstOrDefault(file => file.ToLower().EndsWith(x.Name.ToLower() + ".cs"));
                    var category = path != null 
                        ? path.Split(System.IO.Path.DirectorySeparatorChar)[0] 
                        : "Misc";
                    return new SketchInfo(x, category);
                })
                .ToArray();


            btn.Focus();
            Closed+= async (o, e) => await img.Stop();

            ICollectionView view = CollectionViewSource.GetDefaultView(types);
            view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(SketchInfo.Category)));

            list.ItemsSource = view;
            //list.SelectedIndex = 0;
            list.SelectionChanged += (o, e) => { img.Run(((SketchInfo)list.SelectedItem).Type); };
            IsVisibleChanged += SketchesWindow_IsVisibleChanged;

        }
        //protected override void OnActivated(EventArgs e) {
        //    base.OnActivated(e);
        //    img.Show_();
        //}
        //protected override void OnDeactivated(EventArgs e) {
        //    base.OnDeactivated(e);
        //    img.Hide_();
        //}
        private void SketchesWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            
        }

        void Button_Click(object sender, RoutedEventArgs e) {
            //img.Run(((SketchInfo)list.SelectedItem).Type);
        }
        protected override void OnLocationChanged(EventArgs e) {
            base.OnLocationChanged(e);
            img.UpdateLocation();
        }
    }
    record SketchInfo(Type Type, string Category);
    /*
     int count = 0;
        double lastTime = 0;
        SKPaint paint = new SKPaint() { Color = new SKColor(0, 0, 0), TextSize = 100 };
        SKPaint paint1 = new SKPaint(new SKFont(SKTypeface.FromFamilyName("Microsoft YaHei UI"))) {
            Color = new SKColor(0, 0, 0),
            TextSize = 20
        };
        void AUView_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e) {
            var sw = new Stopwatch();
            sw.Start();
            var surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(new SKColor(130, 130, 130));
            
            canvas.DrawText("SkiaSharp in Wpf! " + count++, 50, 200, paint);

            canvas.DrawText("Using SkiaSharp for making graphs in WPF " + lastTime, new SKPoint(50, 500), paint1);
            sw.Stop();
            lastTime = ((double)sw.ElapsedTicks/Stopwatch.Frequency) * 1000;
        }
     */
}
