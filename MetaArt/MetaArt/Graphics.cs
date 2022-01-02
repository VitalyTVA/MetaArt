using System;
using System.Linq;

namespace MetaArt.Internal {
    public abstract class Graphics {
        public static Graphics GraphicsInstance => Sketch.Graphics;

        public abstract void fill(Color color);
        public abstract void noFill();
        public abstract void textSize(float size);

        public abstract void stroke(Color color);
        public abstract void strokeWeight(float weight);
        public abstract void strokeJoin(StrokeJoin join);
        public abstract void strokeCap(StrokeCap cap);
        public abstract void noStroke();
        public abstract void blendMode(BlendMode blendMode);
        public abstract void point(float x, float y);
        public abstract void set(float x, float y, Color c);
        public abstract void line(float x0, float y0, float x1, float y1);
        public abstract void rect(float a, float b, float c, float d);
        public abstract void circle(float x, float y, float extent);
        public abstract void ellipse(float x, float y, float width, float height);
        public abstract void arc(float x, float y, float width, float height, float start, float stop);
        public abstract void triangle(float x1, float y1, float x2, float y2, float x3, float y3);
        public abstract void text(string str, float x, float y);
        public abstract void rectMode(RectMode mode);
        public abstract void ellipseMode(RectMode mode);
        public abstract void background(Color color);

        public abstract void pushMatrix();
        public abstract void popMatrix();
        public abstract void push();
        public abstract void pop();
        public abstract void translate(float x, float y);
        public abstract void scale(float x, float y);
        public abstract void rotate(float angle);
        public abstract void beginShape(BeginShapeMode mode);
        public abstract void endShape(EndShapeMode mode);
        public abstract void vertex(float x, float y);

        public abstract void noSmooth();

        public abstract Pixels loadPixels();
    }

    public abstract class Pixels {
        public readonly Color[] PixelsArray;
        public abstract void UpdatePixelsAndDispose();

        protected Pixels(Color[] pixels) {
            PixelsArray = pixels;
        }
    }
}
