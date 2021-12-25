using MetaArt.Internal;
using SkiaSharp;
using System;

namespace MetaArt.Skia {
    static class Extensions {
        public static SKColor ToSK(this Color value) => new SKColor(value.Value);
        public static SKStrokeJoin ToSK(this StrokeJoin value) => (SKStrokeJoin)value; //TODO test conversion
        public static SKBlendMode ToSK(this BlendMode value) => value switch { 
            BlendMode.Blend => SKBlendMode.SrcOver, 
            BlendMode.Difference => SKBlendMode.Difference, 
            _ => throw new NotImplementedException(), 
        }; //TODO test conversion
    }

    public sealed class SkiaGraphics : Graphics {

        private SKCanvas? canvas;
        public SKCanvas Canvas { get => canvas!; set => canvas = value; }


        SKPaint fillPaint = new SKPaint() { Style = SKPaintStyle.Fill, IsAntialias = true, TextSize = 12 };
        bool _noFill = false;

        public override void fill(Color color) {
            _noFill = false;
            fillPaint.Color = color.ToSK();
        }
        public override void noFill() {
            _noFill = true;
        }
        public override void textSize(float size) => fillPaint.TextSize = size;

        SKPaint strokePaint = new SKPaint() {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 4,
            IsAntialias = true,
            //Color = SKColors.White 
        };
        bool _noStroke = false;


        public override void stroke(Color color) {
            _noStroke = false;
            strokePaint.Color = color.ToSK();
        }
        public override void strokeWeight(float weight) {
            _noStroke = false;
            strokePaint.StrokeWidth = weight;
        }
        public override void strokeJoin(StrokeJoin join) {
            strokePaint.StrokeJoin = join.ToSK();
        }
        public override void noStroke() {
            _noStroke = true;
        }

        public override void blendMode(BlendMode blendMode) {
            fillPaint.BlendMode = blendMode.ToSK();
            strokePaint.BlendMode = blendMode.ToSK();
        }

        public override void line(float x0, float y0, float x1, float y1) {
            Canvas.DrawLine(x0, y0, x1, y1, strokePaint);
        }

        public override void rect(float a, float b, float c, float d) {
            var rect = _rectMode switch {
                RectMode.CENTER => new SKRect(a - c / 2, b - d / 2, a + c / 2, b + d / 2),
                _ => throw new InvalidOperationException()
            };
            if(!_noFill)
                Canvas.DrawRect(rect, fillPaint);
            if(!_noStroke)
                Canvas.DrawRect(rect, strokePaint);
        }

        public override void circle(float x, float y, float extent) {
            var point = new SKPoint(x, y);
            if(!_noFill)
                Canvas.DrawCircle(point, extent / 2, fillPaint);
            if(!_noStroke)
                Canvas.DrawCircle(point, extent / 2, strokePaint);
        }

        public override void ellipse(float x, float y, float width, float height) {
            var point = new SKPoint(x, y);
            var size = new SKSize(width / 2, height / 2);
            if(!_noFill)
                Canvas.DrawOval(point, size, fillPaint);
            if(!_noStroke)
                Canvas.DrawOval(point, size, strokePaint);
        }

        public override void triangle(float x1, float y1, float x2, float y2, float x3, float y3) {
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

        public override void text(string str, float x, float y) {
            Canvas.DrawText(str, x, y, fillPaint);
        }

        RectMode _rectMode = RectMode.CORNER;
        public override void rectMode(RectMode mode) {
            _rectMode = mode;
        }


        public override void background(Color color) {
            Canvas.DrawColor(color.ToSK(), SKBlendMode.SrcOver);
        }
    }
}
