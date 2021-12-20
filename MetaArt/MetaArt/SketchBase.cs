using SkiaSharp;
using System;
using System.Linq;

namespace MetaArt {
    public class SketchBase {
        PainterBase? painter;
        internal PainterBase Painter { get => painter!; set => painter = value; }


        protected SKCanvas Canvas => Painter.Canvas;

        protected int width => Painter.Width;
        protected int height => Painter.Height;


        protected void size(int width, int height) {
            Painter.SetSize(width, height);
        }
        protected void background(byte color) {
            background(new SKColor(color, color, color));
        }
        protected void background(SKColor color) {
            Canvas.Clear(color);
        }
        protected int min(int valu1, int value2) => Math.Min(valu1, value2);

        SKPaint fillPaint = new SKPaint() { Style = SKPaintStyle.Fill, IsAntialias = true };
        SKPaint strokePaint = new SKPaint() { Style = SKPaintStyle.Stroke, IsAntialias = true };

        protected void fill(SKColor color) {
            fillPaint.Color = color;
        }

        protected void noStroke() {
            strokePaint.StrokeWidth = 0;
        }

        protected void circle(float x, float y, float extent) {
            Canvas.DrawCircle(new SKPoint(x, y), extent / 2, fillPaint);
            Canvas.DrawCircle(new SKPoint(x, y), extent / 2, strokePaint);
        }
    }
}
