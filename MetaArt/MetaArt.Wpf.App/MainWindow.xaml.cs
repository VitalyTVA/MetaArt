using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

namespace MetaArt.Wpf.App {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }
        int count = 0;
        double lastTime = 0;
        SKPaint paint = new SKPaint() { Color = new SKColor(0, 0, 0), TextSize = 100 };
        SKPaint paint1 = new SKPaint(new SKFont(SKTypeface.FromFamilyName("Microsoft YaHei UI"))) {
            Color = new SKColor(0, 0, 0),
            TextSize = 20
        };

        private void AUView_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e) {
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

        private void Button_Click(object sender, RoutedEventArgs e) {
            //count++;
            AUView.InvalidateVisual();
        }
    }
}
