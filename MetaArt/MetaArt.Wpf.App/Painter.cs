using MetaArt.Skia;
using SkiaSharp;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MetaArt.Wpf {
    sealed class Painter : PainterBase {
        SKSurface? sKSurface;
        public SKSurface SKSurface {
            get => sKSurface!; 
            set {
                sKSurface = value;
                ((SkiaGraphics)Graphics).Canvas = SKSurface.Canvas;
            }
        }

        public Painter(SketchBase sketch) 
            : base(sketch, new SkiaGraphics()) {
        }

        void ClearSurface() {
            sKSurface = null;
        }
        public void MousePressed(System.Windows.Point mouse) {
            MousePressedCore((float)mouse.X, (float)mouse.Y);
            //ClearSurface();
        }
        public void MouseMoved(System.Windows.Point mouse) {
            MouseMovedCore((float)mouse.X, (float)mouse.Y);
            //ClearSurface();
        }
        public void Setup() {
            SetupCore();
            ClearSurface();
        }
        public void Draw(System.Windows.Point? mouse) {
            DrawCore(mouse != null ? (float)mouse.Value.X : null, mouse != null ? (float)mouse.Value.Y : null);
            ClearSurface();
        }
    }
}
