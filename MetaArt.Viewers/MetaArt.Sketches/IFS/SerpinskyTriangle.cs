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
        ifs = new(
            Transforms: new[] {
                   new Vector2(0, 0),
                   new Vector2(width, 0),
                   new Vector2(width / 2, height),
                }
                .Select(v => IFS.CreateLerpTransform(v, r))
                .ToArray(),
            Probabilities: [1 / 3f, 2 / 3f ]

        );
    }

    IFS ifs = default!;
    Color[] colors = [Colors.Red, Colors.Green, Colors.Blue];

    float x = 0;
    float y = 0;
    void draw() {
        translate(0, height);
        scale(1, -1);

        int maximum_iterations = 30;
        float t = 0;
        while(t < maximum_iterations) {
            float xn = 0;
            float yn = 0;
            float r = random(1);
            var (v, i) = ifs.Transform(new Vector2(x, y), r);
            xn = v.X;
            yn = v.Y;

            stroke(colors[i]);
            point(round(xn), round(yn));
            x = xn;
            y = yn;
            t++;
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
}
