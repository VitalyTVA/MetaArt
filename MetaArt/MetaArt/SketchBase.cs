using SkiaSharp;
using System;
using System.Linq;

namespace MetaArt {
    public class SketchBase {
        PainterBase? painter;
        internal PainterBase Painter { get => painter!; set => painter = value; }


        protected SKCanvas Canvas => Painter.Canvas;




        protected void size(int width, int height) {
            Painter.SetSize(width, height);
        }
        protected void background(byte color) {
            Canvas.Clear(new SKColor(color, color, color));
        }


        public virtual void setup() { }
        public virtual void draw() { }

    }
}
