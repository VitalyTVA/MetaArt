﻿using MetaArt.Internal;
using System;
using System.Diagnostics;
using System.Linq;
namespace MetaArt {
    public enum StrokeJoin {
        Miter,
        Round,
        Bevel
    }

    public enum BlendMode {
        Blend,
        Difference,
    }

    public class SketchBase {
        internal int currentTime;
        protected internal int deltaTime { get; internal set; } //TODO should only be accessible from draw, not from mouse events
        protected int millis() => (int)currentTime; //TODO => stopwatch.ElapsedMillisecond
        protected internal int frameCount { get; internal set; }
        protected int second() => DateTime.Now.Second;
        protected int minute() => DateTime.Now.Minute;
        protected int hour() => DateTime.Now.Hour;

        PainterBase? painter;
        internal PainterBase Painter { get => painter!; set => painter = value; }
        Graphics Graphics => Painter.Graphics;


        protected float width => Painter.Width;
        protected float height => Painter.Height;

        protected internal float mouseX;
        protected internal float mouseY;
        protected internal float pmouseX;
        protected internal float pmouseY;
        protected internal char key { get; internal set; }

        protected static Color Black => new Color(0, 0, 0);
        protected static Color White => new Color(255, 255, 255);

        protected void size(int width, int height) {
            //TODO if called from setup in async mode or from draw in normal mode
            Painter.SetSize(width, height);
        }
        protected void noLoop() => Painter.NoLoop = true;

        protected void background(byte v1, byte v2, byte v3, byte a) {
            background(new Color(v1, v2, v3, a));
        }
        protected void background(Color color) => Graphics.background(color);
        protected void background(byte color) {
            background(new Color(color, color, color));
        }

        protected static StrokeJoin ROUND => StrokeJoin.Round;
        protected static StrokeJoin MITER => StrokeJoin.Miter;
        protected static StrokeJoin BEVEL => StrokeJoin.Bevel;
        protected void noStroke() => Graphics.noStroke();
        protected void stroke(byte color) {
            stroke(new Color(color, color, color));
        }
        protected void stroke(Color color) => Graphics.stroke(color);
        protected void strokeWeight(float weight) => Graphics.strokeWeight(weight);
        protected void strokeJoin(StrokeJoin join) => Graphics.strokeJoin(join);
        protected void strokeCap(StrokeCap cap) => Graphics.strokeCap(cap);

        protected void fill(byte gray, byte alpha = 255) {
            fill(new Color(gray, gray, gray, alpha));
        }
        protected void fill(Color color) {
            Graphics.fill(color);
        }
        protected void noFill() {
            Graphics.noFill();
        }
        protected void textSize(float size) => Graphics.textSize(size);

        protected static BlendMode BLEND => BlendMode.Blend;
        protected static BlendMode DIFFERENCE => BlendMode.Difference;
        protected void blendMode(BlendMode blendMode) => Graphics.blendMode(blendMode);

        protected void push() {
            Graphics.push();
        }
        protected void pop() {
            Graphics.pop();
        }
        protected void translate(float x, float y) {
            Graphics.translate(x, y);
        }
        protected void scale(float x, float y) {
            Graphics.scale(x, y);
        }
        protected void rotate(float angle) {
            Graphics.rotate(angle);
        }

        protected static RectMode CORNER = RectMode.CORNER;
        protected static RectMode CORNERS = RectMode.CORNERS;
        protected static RectMode RADIUS = RectMode.RADIUS;
        protected static RectMode CENTER = RectMode.CENTER;

        protected void rectMode(RectMode mode) => Graphics.rectMode(mode);
        protected void ellipseMode(RectMode mode) => Graphics.ellipseMode(mode);

        protected void line(float x0, float y0, float x1, float y1) => Graphics.line(x0, y0, x1, y1);

        protected void rect(float a, float b, float c, float d) => Graphics.rect(a, b, c, d);

        protected void circle(float x, float y, float extent) => Graphics.circle(x, y, extent);

        protected void ellipse(float x, float y, float width, float height) => Graphics.ellipse(x, y, width, height);

        protected void arc(float x, float y, float width, float height, float start, float stop) => Graphics.arc(x, y, width, height, start, stop);

        protected void triangle(float x1, float y1, float x2, float y2, float x3, float y3) => Graphics.triangle(x1, y1, x2, y2, x3, y3);

        protected void text(string str, float x, float y) => Graphics.text(str, x, y);

        protected void beginShape(BeginShapeMode mode = BeginShapeMode.LINES) => Graphics.beginShape(mode);
        protected static EndShapeMode CLOSE => EndShapeMode.CLOSE;
        protected static BeginShapeMode POINTS => BeginShapeMode.POINTS;
        protected void endShape(EndShapeMode mode = EndShapeMode.OPEN) => Graphics.endShape(mode);
        protected void vertex(float x, float y) => Graphics.vertex(x, y);

        protected static int floor(float d) => (int)Math.Floor(d);
        protected static float exp(float d) => (float)Math.Exp(d);
        protected static float pow(float n, float e) => (float)Math.Pow(n, e);
        protected static float sin(float angle) => (float)Math.Sin(angle);
        protected static float cos(float angle) => (float)Math.Cos(angle);
        protected const float PI = (float)Math.PI;
        protected const float TWO_PI = PI * 2;
        protected const float HALF_PI = PI  / 2;
        protected static float lerp(float start, float stop, float amt) => start * (1 - amt) + stop * amt;
        protected static float map(float value, float start1, float stop1, float start2, float stop2) => lerp(start2, stop2, (value - start1) / (stop1 - start1));
        protected static float norm(float value, float start, float stop) => lerp(0, 1, (value - start) / (stop - start));
        protected static float constrain(float amt, float low, float high) => min(max(amt, low), high);

        const float degreesToRadians = PI / 180;
        protected static float radians(float degrees) => degreesToRadians * degrees;

        Random rnd = new();
        protected float random(float low, float high) => lerp(low, high, (float)rnd.NextDouble());

        protected static float min(float value1, float value2) => Math.Min(value1, value2);
        protected static float max(float value1, float value2) => Math.Max(value1, value2);
        protected static float abs(float n) => Math.Abs(n);
    }
    //https://p5js.org/reference/#/p5/rectMode
    public enum RectMode {
        CORNER,
        CORNERS,
        CENTER,
        RADIUS
    }
    public enum BeginShapeMode { LINES, POINTS }
    public enum EndShapeMode { OPEN, CLOSE }
    public enum StrokeCap { PROJECT, ROUND, SQUARE }
}
