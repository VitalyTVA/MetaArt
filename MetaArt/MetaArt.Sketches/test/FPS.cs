using System.Diagnostics;
namespace TestSketches;
class FPS {
    void setup() {
        size(200, 200);
        background(150);
    }


    Stopwatch Stopwatch = new Stopwatch();
    void draw() {
        int x = 30;
        textSize(20);
        fill(Black);
        background(150);
        text("SkiaSharp on Wpf!", x, 20);
        if(frameCount == 0)
            this.Stopwatch.Start();
        double fps = frameCount / ((this.Stopwatch.Elapsed.TotalSeconds != 0) ? this.Stopwatch.Elapsed.TotalSeconds : 1);
        text($"FPS: {fps:0}", x, 40);
        text($"Frames: {frameCount}", x, 60);

    }
}