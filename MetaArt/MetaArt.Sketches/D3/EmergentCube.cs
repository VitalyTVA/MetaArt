using System.Numerics;
using MetaArt.D3;
using Vector = MetaArt.Vector;

namespace D3;
class EmergentCube {
    Model cube = null!;
    void setup() {
        size(600, 400);

        cube = Extensions.CreateCube(100);
        pointPlains = cube.Quads.Select(x => {
            var v1 = cube.GetVertex(x.Item1);
            var v2 = cube.GetVertex(x.Item2);
            var v3 = cube.GetVertex(x.Item3);
            var v4 = cube.GetVertex(x.Item4);

            var (x1, y1, z1) = v1;
            var (x2, y2, z2) = v3;
            var points = Enumerable
                .Range(0, 30)
                .Select(_ => new Vector3(random(x1, x2), random(y1, y2), random(z1, z2)))
                .ToArray();
            return (points, (v1, v2, v3, v4));
        }).ToArray();

    }

    SphereCameraContoller controller = new();
    (Vector3[] points, (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4))[]? pointPlains;
    void draw() {
        noSmooth();
        stroke(White);
        strokeWeight(1);
        noFill();
        background(0);
        CameraExtensions.InitCoords();

        var c = controller.CreateCamera();

        var dx = mouseX - pmouseX;
        var dy = mouseY - pmouseY;
        if(isLeftMousePressed) {
            const float scale = 200;
            controller.Pitch(-dy / scale);
            controller.Yaw(-dx / scale);
        }
        if(isRightMousePressed) {
            cube.Rotate(c, dx, -dy);
        }

        var pointSize = 2;

        randomSeed(0);
        var backgroundPoints = Enumerable.Range(0, 400).Select(_ => (random(-width / 2, width / 2), random(-height / 2, height / 2)));
        stroke(White);
        strokeWeight(pointSize);
        foreach(var (x, y) in backgroundPoints) {
            point(x, y);
        }

        fill(Black);
        foreach(var (points, v) in pointPlains!) {
            var v1 = Vector3.Transform(v.v1, cube.Rotation);
            var v2 = Vector3.Transform(v.v2, cube.Rotation);
            var v3 = Vector3.Transform(v.v3, cube.Rotation);
            var v4 = Vector3.Transform(v.v4, cube.Rotation);
            var n = Extensions.GetNormal(v1, v2, v3);
            if(!c.IsVisible(v1, n)) continue;
            noStroke();
            c.quad3(v1, v2, v3, v4);
            stroke(White);
            strokeWeight(pointSize);
            foreach(var p in points) {
                var (x, y) = c.ProjectPoint(Vector3.Transform(p, cube.Rotation));
                point(x, y);
            }
        }
    }

    void keyPressed() {
        CameraExtensions.MoveCameraOnShepeOnKeyPressed(controller);
    }
}
