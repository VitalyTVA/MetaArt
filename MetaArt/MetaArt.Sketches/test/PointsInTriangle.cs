using MetaArt;
using System.Diagnostics;

namespace TestSketches;
class PointsInTriangle {
    void setup() {
        size(400, 400);
        stroke(White);
        noFill();
        strokeWeight(2);
        seed = (int)random(0, 100);
    }
    int seed = 0;
    void draw() {
        background(0);
        randomSeed(seed);

        var o = new Vector(50, 200);
        var a = new Vector(200, -150);
        var b = new Vector(150, 100);
        triangle(o, o + a, o + b);
        for(int i = 0; i < 200; i++) {
            var u1 = random(1);
            var u2 = random(1);
            if(u1 + u2 > 1) {
                u1 = 1 - u1;
                u2 = 1 - u2;
            }
            var (x, y) = o + a.Mult(u1) + b.Mult(u2);
            point(x, y);
        }
    }
}
