using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace IFS; 
class BarnsleyFern {
    void setup() {
        size(800, 800);
        stroke(255);
        strokeWeight(1);

        var ifs = new IFS(
            Transforms: [
                new Matrix3x2(0, 0, 0, 0.16f, 0, 0),
                new Matrix3x2(0.85f, -0.04f, 0.04f, 0.85f, 0, 1.6f),
                new Matrix3x2(0.2f, 0.23f, -0.26f, 0.22f, 0, 1.6f),
                new Matrix3x2(-0.15f, 0.26f, 0.28f, 0.24f, 0, 0.44f),
            ],
            Probabilities: [0.01f, 0.86f, 0.93f]
        );
        pointsEnumerator = ifs.GetPointsIterator(new Vector2(0, 0), () => random(1)).GetEnumerator();
    }

    Color[] colors = [Colors.Red, Colors.Green, Colors.Blue, Colors.Pink];
    IEnumerator<(Vector2 point, int index)> pointsEnumerator = default!;

    void draw() {
        translate(width / 2, height);
        scale(1, -1);
        
        var t = 0;
        while(t++ < 30) {
            pointsEnumerator.MoveNext();
            var (v, i) = pointsEnumerator.Current;
            stroke(colors[i]);
            point(round(v.X * width / 10), round(v.Y * height / 10));
        }
    }
}
