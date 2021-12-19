using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        Type[] types = System.Reflection.Assembly.GetEntryAssembly()!.GetTypes().Where(x => typeof(SketchBase).IsAssignableFrom(x)).ToArray();
        public SketchesWindow() {
            InitializeComponent();
            btn.Focus();
            Closed+= (o, e) => img.Stop();
            list.ItemsSource = types;
            list.SelectedIndex = 0;
        }
        void Button_Click(object sender, RoutedEventArgs e) {
            img.Run((Type)list.SelectedItem);
        }
    }
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
