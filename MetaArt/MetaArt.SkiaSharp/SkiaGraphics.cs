using MetaArt.Internal;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace MetaArt.Skia {
    static class Extensions {
        public static SKColor ToSK(this Color value) => new SKColor(value.Value);
        public static SKStrokeJoin ToSK(this StrokeJoin value) => (SKStrokeJoin)value; //TODO test conversion
        public static SKStrokeCap ToSK(this StrokeCap value) => (SKStrokeCap)value; //TODO test conversion
        public static SKBlendMode ToSK(this BlendMode value) => value switch { 
            BlendMode.Blend => SKBlendMode.SrcOver, 
            BlendMode.Difference => SKBlendMode.Difference, 
            _ => throw new NotImplementedException(), 
        }; //TODO test conversion
        public static float ConvertRadiansToDegrees(float radians) => (float)(180 / Math.PI) * radians;
    }

    public sealed class SkiaGraphics : Graphics {

        private SKCanvas? canvas;
        public SKCanvas Canvas { get => canvas!; set => canvas = value; }


        SKPaint fillPaint = new SKPaint() { 
            Style = SKPaintStyle.Fill, 
            IsAntialias = true, 
            TextSize = 12,
            Color = SKColors.White,
        };
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
            StrokeWidth = 2,
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Round,
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
        public override void strokeCap(StrokeCap cap) {
            strokePaint.StrokeCap = cap.ToSK();
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
                RectMode.CORNERS => new SKRect(a, b, c, d),
                RectMode.CORNER => new SKRect(a, b, a + c, b + d),
                RectMode.RADIUS => new SKRect(a - c, b - d, a + c, b + d),
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

        public override void ellipse(float a, float b, float c, float d) {
            var (point, size) = _ellipseMode switch {
                RectMode.RADIUS => (new SKPoint(a, b), new SKSize(c, d)),
                RectMode.CENTER => (new SKPoint(a, b), new SKSize(c / 2, d / 2)),
                _ => throw new InvalidOperationException()
            };
            if(!_noFill)
                Canvas.DrawOval(point, size, fillPaint);
            if(!_noStroke)
                Canvas.DrawOval(point, size, strokePaint);
        }
        public override void arc(float x, float y, float width, float height, float start, float stop) {
            var rect = new SKRect(x - width / 2, y - height / 2, x + width / 2, x + height / 2);
            if(!_noFill)
                Canvas.DrawArc(rect, Extensions.ConvertRadiansToDegrees(start), Extensions.ConvertRadiansToDegrees(stop - start), false, fillPaint);
            if(!_noStroke)
                Canvas.DrawArc(rect, Extensions.ConvertRadiansToDegrees(start), Extensions.ConvertRadiansToDegrees(stop - start), false, strokePaint);
        }

        public override void triangle(float x1, float y1, float x2, float y2, float x3, float y3) {
            using var path = new SKPath { FillType = SKPathFillType.EvenOdd }; //TODO reuse triangle path??
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

        RectMode _ellipseMode = RectMode.CENTER;
        public override void ellipseMode(RectMode mode) {
            _ellipseMode = mode;
        }


        public override void background(Color color) {
            Canvas.DrawColor(color.ToSK(), SKBlendMode.SrcOver);
        }

        public override void push() {
            Canvas.Save();
        }
        public override void pop() {
            Canvas.Restore();
        }
        public override void translate(float x, float y) {
            Canvas.Translate(x, y);
        }
        public override void scale(float x, float y) {
            Canvas.Scale(x, y);
        }
        public override void rotate(float angle) {
            Canvas.RotateRadians(angle);
        }

        SKPath? vertices;
        List<SKPoint> points = new(); //TODO use LargeArrayBuilder??
        public override void beginShape(BeginShapeMode mode) {
            if(mode == BeginShapeMode.LINES)
                vertices = new SKPath { FillType = SKPathFillType.EvenOdd };
        }
        public override void vertex(float x, float y) {
            if(vertices != null) {
                if(vertices!.IsEmpty)
                    vertices.MoveTo(new SKPoint(x, y));
                else
                    vertices.LineTo(new SKPoint(x, y));
            } else {
                points.Add(new SKPoint(x, y));
            }
        }
        public override void endShape(EndShapeMode mode) {
            if(vertices != null) {
                if(mode == EndShapeMode.CLOSE)
                    vertices.Close();
                if(!_noFill && mode == EndShapeMode.CLOSE) {
                    Canvas.DrawPath(vertices, fillPaint);
                }
                if(!_noStroke) {
                    Canvas.DrawPath(vertices, strokePaint);
                }
                vertices.Dispose();
                vertices = null;
            } else {
                Canvas.DrawPoints(SKPointMode.Points, points.ToArray(), strokePaint);
                points.Clear();
            }
        }
    }
}
