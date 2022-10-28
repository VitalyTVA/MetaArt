using MetaArt;
using SkiaSharp;
using System.Diagnostics;

class ArcVertex {
    void setup() {
        size(400, 400);
    }

    void draw() {
        background(255);

		strokeWeight(5);
        fill(color(150));
		strokeCap(StrokeCap.ROUND);
        stroke(0);

		beginShape();
		vertex(100, 100);
		vertex(100, 150);
		vertex(150, 150);
		endShape(CLOSE);


		beginShape();
		vertex(300, 200);
		arcVertex(200, 100, 100, 100, 0, PI / 2);
		arcVertex(200, 200, 100, 100, 3 * PI / 2, PI);
		vertex(300, 200);

		endShape(CLOSE);

		using var path1 = new SKPath {  };
		path1.MoveTo(new SKPoint(300, 300));
        path1.ArcTo(new SKRect(150, 150, 250, 250), 0, 90, false);
        path1.ArcTo(new SKRect(150, 250, 250, 350), 270, -90, false);
		path1.Close();
		Canvas.DrawPath(path1, Graphics.StrokePaint);
		Canvas.DrawPath(path1, Graphics.FillPaint);
    }
}
