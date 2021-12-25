using MetaArt.Internal;
using SkiaSharp;
using System;
using System.Diagnostics;
using System.Linq;

namespace MetaArt.Internal {
    public abstract class Graphics {
        public abstract void fill(Color color);
        public abstract void noFill();
        public abstract void textSize(float size);

        public abstract void stroke(Color color);
        public abstract void strokeWeight(float weight);
        public abstract void strokeJoin(StrokeJoin join);
        public abstract void noStroke();
        public abstract void blendMode(BlendMode blendMode);
        public abstract void line(float x0, float y0, float x1, float y1);
        public abstract void rect(float a, float b, float c, float d);
        public abstract void circle(float x, float y, float extent);
        public abstract void ellipse(float x, float y, float width, float height);
        public abstract void triangle(float x1, float y1, float x2, float y2, float x3, float y3);
        public abstract void text(string str, float x, float y);
        public abstract void rectMode(RectMode mode);
        public abstract void background(Color color);
    }
}
namespace MetaArt {

	public readonly struct Color : IEquatable<Color> {
		public static readonly Color Empty;

		public readonly uint Value;

		public byte Alpha => (byte)((Value >> 24) & 0xFFu);

		public byte Red => (byte)((Value >> 16) & 0xFFu);

		public byte Green => (byte)((Value >> 8) & 0xFFu);

		public byte Blue => (byte)(Value & 0xFFu);

		public Color(uint value) {
			Value = value;
		}

		public Color(byte red, byte green, byte blue, byte alpha) {
			Value = (uint)((alpha << 24) | (red << 16) | (green << 8) | blue);
		}

		public Color(byte red, byte green, byte blue) {
			Value = 0xFF000000u | (uint)(red << 16) | (uint)(green << 8) | blue;
		}

		public Color WithRed(byte red) {
			return new Color(red, Green, Blue, Alpha);
		}

		public Color WithGreen(byte green) {
			return new Color(Red, green, Blue, Alpha);
		}

		public Color WithBlue(byte blue) {
			return new Color(Red, Green, blue, Alpha);
		}

		public Color WithAlpha(byte alpha) {
			return new Color(Red, Green, Blue, alpha);
		}

		public override string ToString() {
			return $"#{Alpha:x2}{Red:x2}{Green:x2}{Blue:x2}";
		}

		public bool Equals(Color obj) {
			return obj.Value == Value;
		}

		public override bool Equals(object other) {
			if(other is Color obj) {
				return Equals(obj);
			}
			return false;
		}

		public static bool operator ==(Color left, Color right) {
			return left.Equals(right);
		}

		public static bool operator !=(Color left, Color right) {
			return !left.Equals(right);
		}

		public override int GetHashCode() {
			uint num = Value;
			return num.GetHashCode();
		}
	}

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
        protected int millis() => (int)currentTime;

        PainterBase? painter;
        internal PainterBase Painter { get => painter!; set => painter = value; }
        Graphics Graphics => Painter.Graphics;


        SKCanvas Canvas => Painter.Canvas;

        protected float width => Painter.Width;
        protected float height => Painter.Height;

        protected internal float mouseX;
        protected internal float mouseY;
        protected internal float pmouseX;
        protected internal float pmouseY;

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
        protected float min(float value1, float value2) => Math.Min(value1, value2);


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
        protected void fill(byte color) {
            fill(new Color(color, color, color));
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

        protected static RectMode CORNER = RectMode.CORNER;
        protected static RectMode CORNERS = RectMode.CORNERS;
        protected static RectMode RADIUS = RectMode.RADIUS;
        protected static RectMode CENTER = RectMode.CENTER;

        protected void rectMode(RectMode mode) => Graphics.rectMode(mode);

        protected void line(float x0, float y0, float x1, float y1) => Graphics.line(x0, y0, x1, y1);

        protected void rect(float a, float b, float c, float d) => Graphics.rect(a, b, c, d);

        protected void circle(float x, float y, float extent) => Graphics.circle(x, y, extent);

        protected void ellipse(float x, float y, float width, float height) => Graphics.ellipse(x, y, width, height);

        protected void triangle(float x1, float y1, float x2, float y2, float x3, float y3) => Graphics.triangle(x1, y1, x2, y2, x3, y3);

        protected void text(string str, float x, float y) => Graphics.text(str, x, y);

        protected static float exp(float d) => (float)Math.Exp(d);
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
