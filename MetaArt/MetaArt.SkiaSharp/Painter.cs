using MetaArt.Skia;
using SkiaSharp;
using System;
using System.Linq;

namespace MetaArt.Skia {
    public sealed class Painter : PainterBase {
        SKSurface? sKSurface;
        public SKSurface SKSurface {
            get => sKSurface!; 
            set {
                sKSurface = value;
                ((SkiaGraphics)Graphics).Canvas = SKSurface.Canvas;
            }
        }

        public Painter(Type sketchType) 
            : base((SketchBase)Activator.CreateInstance(sketchType), new SkiaGraphics()) {
        }

        void ClearSurface() {
            sKSurface = null;
        }
        public void MousePressed(Point mouse) {
            MousePressedCore((float)mouse.X, (float)mouse.Y);
            //ClearSurface();
        }
        public void MouseMoved(Point mouse) {
            MouseMovedCore((float)mouse.X, (float)mouse.Y);
            //ClearSurface();
        }
        public void Setup() {
            SetupCore();
            ClearSurface();
        }
        public void Draw(Point? mouse) {
            DrawCore(mouse != null ? (float)mouse.Value.X : null, mouse != null ? (float)mouse.Value.Y : null);
            ClearSurface();
        }
    }
}
