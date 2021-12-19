using System.Diagnostics;

class FPS : SketchBase {




    public override void setup() {
        size(200, 200);
        background(150);
    }


    int Call = 0;
    Stopwatch Stopwatch = new Stopwatch();
    public override void draw() {
        int x = 30;
        var paint = new SKPaint() { Color = new SKColor(0, 0, 0), TextSize = 20 };
        background(150);
        Canvas.DrawText("SkiaSharp on Wpf!", x, 20, paint);
        if(this.Call == 0)
            this.Stopwatch.Start();
        double fps = this.Call / ((this.Stopwatch.Elapsed.TotalSeconds != 0) ? this.Stopwatch.Elapsed.TotalSeconds : 1);
        Canvas.DrawText($"FPS: {fps:0}", x, 40, paint);
        Canvas.DrawText($"Frames: {this.Call++}", x, 60, paint);

    }
}