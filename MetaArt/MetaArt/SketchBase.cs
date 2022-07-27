using MetaArt.Internal;
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

    public static class Sketch {
        public static int deltaTime => Painter.DeltaTime; //TODO should only be accessible from draw, not from mouse events
        public static int millis() => Painter.Millis();
        public static int frameCount => Painter.FrameCount;
        public static int second() => DateTime.Now.Second;
        public static int minute() => DateTime.Now.Minute;
        public static int hour() => DateTime.Now.Hour;

        [ThreadStatic]
        static PainterBase? painter;
        internal static PainterBase Painter { get => painter!; set => painter = value; }
        internal static void ClearPainter() => painter = null;
        internal static Graphics Graphics => Painter.Graphics;


        public static int width => Painter.Width;
        public static int height => Painter.Height;

        public static float mouseX { get => Painter.mouseX; set => Painter.mouseX = value; }
        public static float mouseY { get => Painter.mouseY; set => Painter.mouseY = value; }
        public static float pmouseX => Painter.pmouseX;
        public static float pmouseY => Painter.pmouseY;
        public static float mouseXOffset => mouseX - pmouseX;
        public static float mouseYOffset => mouseY - pmouseY;
        public static char key => Painter.key;
        public static bool isMousePressed => Painter.isMousePressed;
        public static bool isLeftMousePressed => Painter.isLeftMousePressed;
        public static bool isRightMousePressed => Painter.isRightMousePressed;

        public static Color Black => new Color(0, 0, 0);
        public static Color White => new Color(255, 255, 255);

        public static void size(int width, int height) {
            //TODO if called from setup in async mode or from draw in normal mode
            Painter.SetSize(width, height);
        }
        public static void noLoop() => Painter.NoLoop = true;
        public static void noSmooth() => Graphics.noSmooth();

        public static void background(float v1, float v2, float v3, float a) {
            background(color(v1, v2, v3, a));
        }
        public static void background(Color color) => Graphics.background(color);
        public static void background(float color) {
            background(Painter.color(color));
        }

        public static StrokeJoin ROUND => StrokeJoin.Round;
        public static StrokeJoin MITER => StrokeJoin.Miter;
        public static StrokeJoin BEVEL => StrokeJoin.Bevel;
        public static void noStroke() => Graphics.noStroke();
        public static void stroke(float color) {
            stroke(Painter.color(color, color, color));
        }
        public static void stroke(float color, float a) {
            stroke(Painter.color(color, a));
        }
        public static void stroke(float v1, float v2, float v3) {
            stroke(color(v1, v2, v3));
        }
        public static void stroke(Color color) => Graphics.stroke(color);
        public static void strokeWeight(float weight) => Graphics.strokeWeight(weight);
        public static void strokeJoin(StrokeJoin join) => Graphics.strokeJoin(join);
        public static void strokeCap(StrokeCap cap) => Graphics.strokeCap(cap);

        public static void fill(float gray, float alpha) {
            fill(color(gray, gray, gray, alpha));
        }
        public static void fill(float v1, float v2, float v3) {
            fill(color(v1, v2, v3));
        }
        public static void fill(float gray) {
            fill(color(gray));
        }
        public static void fill(Color color) {
            Graphics.fill(color);
        }
        public static void noFill() {
            Graphics.noFill();
        }
        public static void textSize(float size) => Graphics.textSize(size);
        public static void textAlign(TextAlign alignX, TextVerticalAlign alignY = TextVerticalAlign.BASELINE) => Graphics.textAlign(alignX, alignY);

        public static BlendMode BLEND => BlendMode.Blend;
        public static BlendMode DIFFERENCE => BlendMode.Difference;
        public static void blendMode(BlendMode blendMode) => Graphics.blendMode(blendMode);

        public static float displayDensity() => Painter.displayDensity;

        public static void pushMatrix() => Graphics.pushMatrix();
        public static void popMatrix() => Graphics.popMatrix();
        public static void push() => Graphics.push();
        public static void pop() => Graphics.pop();
        public static void translate(float x, float y) => Graphics.translate(x, y);
        public static void scale(float x, float y) => Graphics.scale(x, y);
        public static void rotate(float angle) => Graphics.rotate(angle);

        public static RectMode CORNER = RectMode.CORNER;
        public static RectMode CORNERS = RectMode.CORNERS;
        public static RectMode RADIUS = RectMode.RADIUS;
        public static RectMode CENTER = RectMode.CENTER;

        public static void rectMode(RectMode mode) => Graphics.rectMode(mode);
        public static void ellipseMode(RectMode mode) => Graphics.ellipseMode(mode);

        public static void point(float x, float y) => Graphics.point(x, y);
        public static void set(float x, float y, Color c) => Graphics.set(x, y, c);

        public static void line(float x0, float y0, float x1, float y1) => Graphics.line(x0, y0, x1, y1);

        public static void rect(float a, float b, float c, float d) => Graphics.rect(a, b, c, d);

        public static void circle(float x, float y, float extent) => Graphics.circle(x, y, extent);

        public static void ellipse(float x, float y, float width, float height) => Graphics.ellipse(x, y, width, height);

        public static void arc(float x, float y, float width, float height, float start, float stop) => Graphics.arc(x, y, width, height, start, stop);

        public static void triangle(float x1, float y1, float x2, float y2, float x3, float y3) => Graphics.triangle(x1, y1, x2, y2, x3, y3);
        public static void triangle(Vector v1, Vector v2, Vector v3) => triangle(v1.X, v1.Y, v2.X, v2.Y, v3.X, v3.Y);
        public static void quad(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4) => Graphics.quad(x1, y1, x2, y2, x3, y3, x4, y4);
        public static void quad(Vector v1, Vector v2, Vector v3, Vector v4) => quad(v1.X, v1.Y, v2.X, v2.Y, v3.X, v3.Y, v4.X, v4.Y);

        public static void text(string str, float x, float y) => Graphics.text(str, x, y);

        public static void beginShape(BeginShapeMode mode = BeginShapeMode.LINES) => Graphics.beginShape(mode);
        public static EndShapeMode CLOSE => EndShapeMode.CLOSE;
        public static BeginShapeMode POINTS => BeginShapeMode.POINTS;
        public static void endShape(EndShapeMode mode = EndShapeMode.OPEN) => Graphics.endShape(mode);
        public static void vertex(float x, float y) => Graphics.vertex(x, y);

        public static int floor(float d) => (int)Math.Floor(d);
        public static int ceil(float d) => (int)Math.Ceiling(d);
        public static float exp(float d) => (float)Math.Exp(d);
        public static float sqrt(float d) => (float)Math.Sqrt(d);
        public static float log(float d) => (float)Math.Log(d);
        public static float pow(float n, float e) => (float)Math.Pow(n, e);
        public static float sin(float angle) => (float)Math.Sin(angle);
        public static float cos(float angle) => (float)Math.Cos(angle);
        public static float atan2(float y, float x) => (float)Math.Atan2(y, x);
        public static float dist(float x1, float y1, float x2, float y2) {
            var x = x1 - x2;
            var y = y1 - y2;
            return sqrt(x * x + y * y);
        }

        public const float PI = (float)Math.PI;
        public const float TWO_PI = PI * 2;
        public const float HALF_PI = PI  / 2;
        public static float lerp(float start, float stop, float amt) => start * (1 - amt) + stop * amt;
        public static float map(float value, float start1, float stop1, float start2, float stop2) => lerp(start2, stop2, (value - start1) / (stop1 - start1));
        public static float norm(float value, float start, float stop) => lerp(0, 1, (value - start) / (stop - start));
        public static float constrain(float amt, float low, float high) => min(max(amt, low), high);

        const float degreesToRadians = PI / 180;
        public static float radians(float degrees) => degreesToRadians * degrees;

        public static float random(float low, float high) => lerp(low, high, (float)Painter.NextDouble());
        public static float random(float high) => random(0, high);
        public static void randomSeed(int seed) => Painter.RandomSeed(seed);

        public static float randomGaussian() {
            double mu = 0;
            double sigma = 1;
            var u1 = Painter.NextDouble();
            var u2 = Painter.NextDouble();
            var rand_std_normal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                Math.Sin(2.0 * Math.PI * u2);
            var rand_normal = mu + sigma * rand_std_normal;

            return (float)rand_normal;
        }

        public static float min(float value1, float value2) => Math.Min(value1, value2);
        public static float max(float value1, float value2) => Math.Max(value1, value2);
        public static float abs(float n) => Math.Abs(n);

        public static readonly ColorMode RGB = ColorMode.RGB;
        public static readonly ColorMode HSB = ColorMode.HSB;

        public static void colorMode(ColorMode mode, float max) => Painter.colorMode(mode, max);
        public static void colorMode(ColorMode mode, float max1, float max2, float max3) => Painter.colorMode(mode, max1, max2, max3);

        public static Color color(float v1, float v2, float v3, float? a = null) => Painter.color(v1, v2, v3, a);
        public static Color color(float v)  => Painter.color(v);

        public static Color[] pixels => Painter.pixels;
        public static void loadPixels() => Painter.loadPixels();
        public static void updatePixels() => Painter.updatePixels();

        public static void frameRate(float fps) => Painter.SetFPS(fps);

        public static PImage loadImage(string filename) {
            var stream = GetStream(filename);
            return Graphics.createImage(stream);
        }
        static System.IO.Stream GetStream(string filename) {
            return Painter.Assembly.GetManifestResourceStream(Painter.Assembly.GetName().Name + ".Assets." + filename);
        }

        public static void image(PImage image, float a, float b) => Graphics.image(image, a, b);
        public static void tint(int rgb, float alpha) {
            if(rgb != 255)
                throw new InvalidOperationException(); //TODO tint
            Graphics.imageOpacity(alpha);

        }
        public static void imageMode(RectMode mode) => Graphics.imageMode(mode);

        public static PFont createFont(string filename, float size) {
            var stream = GetStream(filename);
            return Graphics.createFont(stream, size);
        }

        public static void textFont(PFont font) => Graphics.textFont(font);
        public static float textAscent() => Graphics.textAscent();
        public static float textDescent() => Graphics.textDescent();
        public static float textWidth(string text) => Graphics.textWidth(text);
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
    public enum ColorMode { RGB, HSB }
    public enum TextAlign { LEFT, CENTER, RIGHT }
    public enum TextVerticalAlign { BASELINE, CENTER }
}
