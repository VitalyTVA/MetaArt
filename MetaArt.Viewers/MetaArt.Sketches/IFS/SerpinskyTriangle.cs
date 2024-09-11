using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace IFS;
class SerpinskyTriangle {
    void setup() {
        size(800, 800);
        stroke(255);
        strokeWeight(1);
        var r = 0.5f;
        var ifs = new IFS(
            Transforms: new[] {
                   new Vector2(0, 0),
                   new Vector2(width, 0),
                   new Vector2(width / 2, height),
                }
                .Select(v => IFS.CreateLerpTransform(v, r))
                .ToArray(),
            Probabilities: [1 / 3f, 2 / 3f ]
        );
        pointsEnumerator = ifs.GetPointsIterator(new Vector2(0, 0), () => random(1)).GetEnumerator();
    }

    Color[] colors = [Colors.Red, Colors.Green, Colors.Blue];
    IEnumerator<(Vector2 point, int index)> pointsEnumerator = default!;


    void draw() {
        translate(0, height);
        scale(1, -1);

        var t = 0;
        while(t++ < 30) {
            pointsEnumerator.MoveNext();
            var (v, i) = pointsEnumerator.Current;
            stroke(colors[i]);
            point(round(v.X), round(v.Y));
        }
    }
}

record IFS(Matrix3x2[] Transforms, float[] Probabilities) { 
    public static Matrix3x2 CreateLerpTransform(Vector2 point, float r) {
        return new Matrix3x2(r, 0, 0, r, point.X * (1 - r), point.Y * (1 - r));
    }
    public (Vector2 point, int index) Transform(Vector2 point, float p) {
        var i = 0;
        for(i = 0; i < Probabilities.Length; i++) {
            if(p < Probabilities[i])
                break;
        }
        return (Vector2.Transform(point, Transforms[i]), i);
    }
    public IEnumerable<(Vector2 point, int index)> GetPointsIterator(Vector2 point, Func<float> getRandomValue) {
        while(true) {
            var next = Transform(point, getRandomValue());
            point = next.point;
            yield return next;
        }
    } 
}
