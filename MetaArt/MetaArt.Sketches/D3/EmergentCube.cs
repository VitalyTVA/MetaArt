using System.Numerics;
using MetaArt.D3;
using Vector = MetaArt.Vector;

namespace D3;
class EmergentCube {
    Model cube = null!;
    void setup() {
        size(600, 400);

        cube = Extensions.CreateCube(100);
    }

    SphereCameraContoller controller = new();

    void draw() {
        noSmooth();
        stroke(White);
        strokeWeight(1);
        noFill();
        background(0);
        CameraExtensions.InitCoords();

        var c = controller.CreateCamera();

        var pointPlains = cube.Quads.Select(x => {
            var v1 = cube.Vertices[x.Item1];
            var v2 = cube.Vertices[x.Item2];
            var v3 = cube.Vertices[x.Item3];
            var v4 = cube.Vertices[x.Item4];

            var (x1, y1, z1) = v1;
            var (x2, y2, z2) = v3;
            var points = Enumerable
                .Range(0, 30)
                .Select(_ => new Vector3(random(x1, x2), random(y1, y2), random(z1, z2)))
                .ToArray();
            return (points, (v1, v2, v3, v4));
        }).ToArray();

        var pointSize = 2;

        randomSeed(0);
        var backgroundPoints = Enumerable.Range(0, 400).Select(_ => (random(-width / 2, width / 2), random(-height / 2, height / 2)));
        stroke(White);
        strokeWeight(pointSize);
        foreach(var (x, y) in backgroundPoints) {
            point(x, y);
        }

        fill(Black);
        foreach(var (points, (v1, v2, v3, v4)) in pointPlains) {
            var n = Extensions.GetNormal(v1, v2, v3);
            if(!c.IsVisible(v1, n)) continue;
            noStroke();
            c.quad3(v1, v2, v3, v4);
            stroke(White);
            strokeWeight(pointSize);
            foreach(var p in points) {
                var (x, y) = c.ProjectPoint(p);
                point(x, y);
            }
        }
    }

    void keyPressed() {
        CameraExtensions.MoveCameraOnShepeOnKeyPressed(controller);
    }
}
