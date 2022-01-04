using MetaArt.Internal;
using SkiaSharp;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;

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
        public static SKTextAlign ToSK(this TextAlign value) => value switch {
            TextAlign.CENTER => SKTextAlign.Center,
            TextAlign.LEFT => SKTextAlign.Left,
            TextAlign.RIGHT => SKTextAlign.Right,
            _ => throw new NotImplementedException(),
        }; //TODO test conversion
    }

    public sealed class SkiaGraphics : Graphics {
        public SKCanvas Canvas { get; private set; } = null!;
        SKSurface surface = null!;
        public SKSurface Surface {
            get => surface;
            set {
                surface = value;
                Canvas = surface.Canvas;
            }
        }

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

        TextVerticalAlign textVerticalAlign = TextVerticalAlign.BASELINE;
        public override void textAlign(TextAlign alignX, TextVerticalAlign alignY) {
            textVerticalAlign = alignY;
            fillPaint.TextAlign = alignX.ToSK();
        }

        SKPaint strokePaint = new SKPaint() {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
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
        public override void set(float x, float y, Color c) {
            Canvas.DrawPoint(x, y, c.Value);
        }
        public override void point(float x, float y) {
            if(strokePaint.StrokeCap == SKStrokeCap.Square)
                return;
            Canvas.DrawPoint(x, y, strokePaint);
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

        public override void quad(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4) {
            using var path = new SKPath { FillType = SKPathFillType.EvenOdd }; //TODO reuse triangle path??
            path.MoveTo(x1, y1);
            path.LineTo(x2, y2);
            path.LineTo(x3, y3);
            path.LineTo(x4, y4);
            path.Close();
            if(!_noFill)
                Canvas.DrawPath(path, fillPaint);
            if(!_noStroke)
                Canvas.DrawPath(path, strokePaint);
        }

        public override void text(string str, float x, float y) {
            if(textVerticalAlign == TextVerticalAlign.CENTER)
                y += textAscent() / 2;
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

        public override void popMatrix() {
            Canvas.Restore();
        }
        public override void pushMatrix() {
            Canvas.Save();
        }
        public override void pop() {
            //TODO save drawing style, not only matrix
            Canvas.Restore();
        }
        public override void push() {
            //TODO save drawing style, not only matrix
            Canvas.Save();
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

        public override void noSmooth() {
            fillPaint.IsAntialias = false;
            strokePaint.IsAntialias = false;
        }

        class SkiaPixels : Pixels {
            readonly SKCanvas canvas;
            readonly SKImage image;
            readonly SKBitmap bitmap;
            readonly SKColor[] skPixels;

            public SkiaPixels(SKCanvas canvas, SKImage image, SKBitmap bitmap, Color[] pixels, SKColor[] skPixels) 
                : base(pixels) {
                this.canvas = canvas;
                this.image = image;
                this.bitmap = bitmap;
                this.skPixels = skPixels;
            }

            public override void UpdatePixelsAndDispose() {
                var len = skPixels.Length;
                for(int i = 0; i < len; i++) {
                    uint value = PixelsArray[i].Value;
                    skPixels[i] = value;
                }
                bitmap.Pixels = skPixels;

                canvas.Save();
                canvas.SetMatrix(SKMatrix.Identity);
                canvas.DrawBitmap(bitmap, SKPoint.Empty);
                canvas.Restore();

                bitmap.Dispose();
                image.Dispose();
            }
        }
        public override Pixels loadPixels() {
            var shot = Surface.Snapshot();
            var bmp = SKBitmap.FromImage(shot);
            var skPixels = bmp.Pixels;
            var pixels = new Color[skPixels.Length];
            var len = skPixels.Length;
            for(int i = 0; i < len; i++) { //TODO copying pixels 1 by 1 is very slow
                uint value = (uint)skPixels[i];
                pixels[i] = new Color(value);
            }
            return new SkiaPixels(Surface.Canvas, shot, bmp, pixels, skPixels);
        }

        class SkiaImage : PImage {
            public readonly SKImage image;

            public SkiaImage(SKImage image) {
                this.image = image;
            }
        }
        public override PImage createImage(Stream stream) {
            return new SkiaImage(SKImage.FromEncodedData(stream)); //TODO dispose image
        }
        SKPaint imagePaint = new SKPaint() { Color = SKColors.Black };
        RectMode _imageMode = RectMode.CORNER;
        public override void imageOpacity(float opacity) {
            imagePaint.Color = imagePaint.Color.WithAlpha((byte)opacity);
        }
        public override void image(PImage image, float a, float b) {
            var skImage = ((SkiaImage)image).image;
            var (x, y) = _imageMode switch {
                RectMode.CORNER => (a, b),
                RectMode.CENTER => (a - skImage.Width / 2f, b - skImage.Height / 2f),
                _ => throw new InvalidOperationException()
            };
            Canvas.DrawImage(skImage, x, y, imagePaint);
        }
        public override void imageMode(RectMode mode) {
            _imageMode = mode;
        }

        class SkiaFont : PFont {
            public readonly SKTypeface typeface;
            public readonly float fontSize;
            public SkiaFont(SKTypeface typeface, float fontSize) {
                this.typeface = typeface;
                this.fontSize = fontSize;
            }
        }
        public override PFont createFont(Stream stream, float size) {
            var typeface = SKTypeface.FromStream(stream);
            return new SkiaFont(typeface, size);
        }
        public override void textFont(PFont font) {
            var skiaFont = (SkiaFont)font;
            fillPaint.TextSize = skiaFont.fontSize;
            fillPaint.Typeface = skiaFont.typeface;
        }
        public override float textAscent() => -fillPaint.FontMetrics.Ascent;
        public override float textDescent() => fillPaint.FontMetrics.Descent;
        public override float textWidth(string text) => fillPaint.MeasureText(text);
    }
}
