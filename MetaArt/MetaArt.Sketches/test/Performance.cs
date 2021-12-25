using System.Diagnostics;

class Performance : SketchBase {




    void setup() {
        size(600, 600);
        background(BlanchedAlmond);
        //background(150);
    }


    void draw() {
        Stopwatch Stopwatch = new Stopwatch();
        Stopwatch.Start();
        background(BlanchedAlmond);
        var rand = new Random();
        for(int i = 0; i < 100_000; i++) {
            stroke(new SKColor(
                    red: (byte)rand.Next(255),
                    green: (byte)rand.Next(255),
                    blue: (byte)rand.Next(255),
                    alpha: (byte)rand.Next(255)
                )
            );
            strokeWeight(rand.Next(1, 10));
            line(
                x0: rand.Next((int)width),
                y0: rand.Next((int)height),
                x1: rand.Next((int)width),
                y1: rand.Next((int)height)
            );
        }

        textSize(100);
        fill(Black);
        text(Stopwatch.ElapsedMilliseconds.ToString(), 30, 200);
    }
}