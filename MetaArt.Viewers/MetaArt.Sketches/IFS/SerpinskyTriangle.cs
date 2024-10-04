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

        var n = 5;
        var r = GetOptimalRatio(n);
        var vertices = Enumerable.Range(0, n)
            .Select(i => new Vector2(cos(i * TWO_PI / n), sin(i * TWO_PI / n)))
            .ToArray();

        var ifs = new IFS(
            Transforms: vertices
                .Select(v => IFS.CreateLerpTransform(v, 1 - r))
                .ToArray(),
            Probabilities: Enumerable.Range(0, n - 1).Select(i => (i + 1) / (float)n).ToArray()
        );
        pointsEnumerator = ifs.GetPointsIterator(vertices[0], () => random(1)).GetEnumerator();

        choice([
            new ChoiceElement<int>("Triangle", 3),
            new ChoiceElement<int>("Pentagon", 5),
            new ChoiceElement<int>("Hexagon", 6)
        ], x => { });
    }

    static float GetOptimalRatio(int n) {
        //https://en.wikipedia.org/wiki/Chaos_game#Optimal_value_of_r_for_every_regular_polygon
        return (n % 4) switch {
            0 => 1 / (1 + tan(PI / n)),
            1 or 3 => 1 / (1 + 2 * sin(PI / (2 * n))),
            2 => 1 / (1 + sin(PI / n))
        };    
    }
   

    Color[] colors = [
        Colors.Red, 
        Colors.Green, 
        Colors.Blue, 
        Colors.Pink, 
        Colors.Green,
        Colors.Orange,
        Colors.Yellow,
        Colors.White,
    ];
    IEnumerator<(Vector2 point, int index)> pointsEnumerator = default!;


    void draw() {
        translate(width / 2, height / 2);
        scale(1, -1);

        var t = 0;
        while(t++ < 30) {
            pointsEnumerator.MoveNext();
            var (v, i) = pointsEnumerator.Current;
            stroke(colors[i]);
            point(round(v.X * width / 2), round(v.Y * height / 2));
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
