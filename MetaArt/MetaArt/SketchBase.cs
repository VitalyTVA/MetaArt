using SkiaSharp;
using System;
using System.Diagnostics;
using System.Linq;

namespace MetaArt {
    //TODO make sketch interface skia-independent
    public class SketchBase {
        Stopwatch stopwatch = new();
        internal void StartStopwatch() => stopwatch.Start();
        protected int millis() => (int)stopwatch.ElapsedMilliseconds;

        PainterBase? painter;
        internal PainterBase Painter { get => painter!; set => painter = value; }


        protected SKCanvas Canvas => Painter.Canvas;

        protected float width => Painter.Width;
        protected float height => Painter.Height;


        protected void size(int width, int height) {
            //TODO if called from setup in async mode or from draw in normal mode
            Painter.SetSize(width, height);
        }
        protected void noLoop() => Painter.NoLoop = true;

        protected void background(byte v1, byte v2, byte v3, byte a) {
            background(new SKColor(v1, v2, v3, a));
        }
        protected void background(SKColor color) {
            Canvas.DrawColor(color, SKBlendMode.SrcOver);
        }
        protected void background(byte color) {
            background(new SKColor(color, color, color));
        }
        protected float min(float value1, float value2) => Math.Min(value1, value2);


        protected static SKStrokeJoin ROUND => SKStrokeJoin.Round;
        protected static SKStrokeJoin MITER => SKStrokeJoin.Miter;
        protected static SKStrokeJoin BEVEL => SKStrokeJoin.Bevel;
        SKPaint strokePaint = new SKPaint() { 
            Style = SKPaintStyle.Stroke, 
            StrokeWidth = 4, 
            IsAntialias = true, 
            //Color = SKColors.White 
        };
        bool _noStroke = false;
        protected void noStroke() {
            _noStroke = true;
        }
        protected void stroke(byte color) {
            stroke(new SKColor(color, color, color));
        }
        protected void stroke(SKColor color) {
            _noStroke = false;
            strokePaint.Color = color;
        }
        protected void strokeWeight(float weight) {
            _noStroke = false;
            strokePaint.StrokeWidth = weight;
        }
        protected void strokeJoin(SKStrokeJoin join) {
            strokePaint.StrokeJoin = join;
        }

        SKPaint fillPaint = new SKPaint() { Style = SKPaintStyle.Fill, IsAntialias = true };
        bool _noFill = false;
        protected void fill(byte color) {
            fill(new SKColor(color, color, color));
        }
        protected void fill(SKColor color) {
            _noFill = false;
            fillPaint.Color = color;
        }
        protected void noFill() {
            _noFill = true;
        }

        protected static SKBlendMode BLEND => SKBlendMode.SrcOver;
        protected static SKBlendMode DIFFERENCE => SKBlendMode.Difference;
        protected void blendMode(SKBlendMode blendMode) {
            fillPaint.BlendMode = blendMode;
            strokePaint.BlendMode = blendMode;
        }

        protected void push() {
            Canvas.Save();
        }
        protected void pop() {
            Canvas.Restore();
        }
        protected void translate(float x, float y) { 
            Canvas.Translate(x, y);
        }
        protected void rotate(float angle) {
            Canvas.RotateRadians(angle);
        }

        RectMode _rectMode = CORNER;
        protected static RectMode CORNER = RectMode.CORNER;
        protected static RectMode CORNERS = RectMode.CORNERS;
        protected static RectMode RADIUS = RectMode.RADIUS;
        protected static RectMode CENTER = RectMode.CENTER;
        protected void rectMode(RectMode mode) {
            _rectMode = mode;
        }
        protected void rect(float a, float b, float c, float d) {
            var rect = _rectMode switch {
                RectMode.CENTER => new SKRect(a - c / 2, b - d / 2, a + c / 2, b + d / 2),
                _ => throw new InvalidOperationException()
            };
            if(!_noFill)
                Canvas.DrawRect(rect, fillPaint);
            if(!_noStroke)
                Canvas.DrawRect(rect, strokePaint);
        }


        protected void circle(float x, float y, float extent) {
            var point = new SKPoint(x, y);
            if(!_noFill)
                Canvas.DrawCircle(point, extent / 2, fillPaint);
            if(!_noStroke)
                Canvas.DrawCircle(point, extent / 2, strokePaint);
        }

        protected void ellipse(float x, float y, float width, float height) {
            var point = new SKPoint(x, y);
            var size = new SKSize(width / 2, height / 2);
            if(!_noFill)
                Canvas.DrawOval(point, size, fillPaint);
            if(!_noStroke)
                Canvas.DrawOval(point, size, strokePaint);
        }

        protected void triangle(float x1, float y1, float x2, float y2, float x3, float y3) {
            var path = new SKPath { FillType = SKPathFillType.EvenOdd }; //TODO reuse triangle path??
            path.MoveTo(x1, y1);
            path.LineTo(x2, y2);
            path.LineTo(x3, y3);
            path.Close();
            if(!_noFill)
                Canvas.DrawPath(path, fillPaint);
            if(!_noStroke)
                Canvas.DrawPath(path, strokePaint);
        }

        protected static float sin(float angle) => (float)Math.Sin(angle);
        protected static float cos(float angle) => (float)Math.Cos(angle);
        protected static float PI => (float)Math.PI;
        protected static float lerp(float start, float stop, float amt) => start * (1 - amt) + stop * amt;
    }
    //https://p5js.org/reference/#/p5/rectMode
    public enum RectMode {
        CORNER,
        CORNERS,
        CENTER,
        RADIUS
    }
}
