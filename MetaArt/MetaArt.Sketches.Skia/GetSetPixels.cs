using MetaArt;
using SkiaSharp;
using System.Diagnostics;

class GetSetPixels {
    void setup() {
        size(640, 360);
        noLoop();
        background(0);
    }

    void draw() {
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        using var shot = Surface.Snapshot();
        using var bmp = SKBitmap.FromImage(shot);
        var pixels = bmp.Pixels;
        for(int i = 0; i < bmp.Width * bmp.Height; i++) {
            var color = new SKColor((byte)(i % 255), 0, 0);
            pixels[i] = color;
        }
        bmp.Pixels = pixels;

        Canvas.Save();
        Canvas.SetMatrix(SKMatrix.Identity);
        Canvas.DrawBitmap(bmp, SKPoint.Empty);
        Canvas.Restore();

        var elapsed = stopwatch.ElapsedMilliseconds.ToString();
        textSize(40);
        text(elapsed, 200, 100);
    }
}
