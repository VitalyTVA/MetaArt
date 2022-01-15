using MetaArt;
using SkiaSharp;
using System.Diagnostics;

class BlurredLine {
	SKPoint[] p = null!;
    void setup() {
        size(400, 400);
        background(0);

        p = Enumerable.Range(0, 20).Select(x => new SKPoint(x * 20, height / 2)).ToArray();
	}

    void draw() {
		

		noFill();
        stroke(255, 5);
        for(int j = 0; j < 1000; j++) {
            using var path = Curve.CreateSpline(p);
            Canvas.DrawPath(path, Graphics.StrokePaint);

            for(int i = 0; i < p.Length; i++) {
                float step = 2f * pow(i / (p.Length - 1f), 3);

                //var mid = (p.Length - 1) / 2f;
                //float step = 1f * ((float)i / p.Length);

                //float step = 0.75f * (-abs(i - mid) + mid) / mid;

                var (dx, dy) = (random(-step, step), random(-step, step));
                p[i].Offset(dx, dy);
            }
        }
        noLoop();
    }
}
