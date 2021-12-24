using SkiaSharp;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MetaArt.Wpf {
    sealed class Painter : PainterBase {
        SKSurface? sKSurface;
        public SKSurface SKSurface { get => sKSurface!; set => sKSurface = value; }

        public Painter(SketchBase sketch) 
            : base(sketch) {
        }

        void ClearSurface() {
            sKSurface = null;
        }

        public void Setup() {
            SetupCore();
            ClearSurface();
        }
        public void Draw(Point? mouse) {
            DrawCore(mouse != null ? (float)mouse.Value.X : null, mouse != null ? (float)mouse.Value.Y : null);
            ClearSurface();
        }

        public override SKCanvas Canvas => SKSurface.Canvas;
    }
}
