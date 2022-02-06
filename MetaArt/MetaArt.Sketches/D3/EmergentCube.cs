using System.Numerics;
using MetaArt.D3;
using Vector = MetaArt.Vector;

namespace D3;
class EmergentCube {
    Model<int[]> cube = null!;
    void setup() {
        size(600, 400);

        var tempCube = Extensions.CreateCube<object?>(100, (null, null, null, null, null, null));

        var vertices = new List<Vector3>(tempCube.Vertices);
        List<(int, int, int, int, int[])> planes = new();

        foreach(var (v1, v2, v3, v4, _) in tempCube.Quads) {
            var (x1, y1, z1) = tempCube.Vertices[v1];
            var (x2, y2, z2) = tempCube.Vertices[v3];
            var points = Enumerable
                .Range(0, 30)
                .Select(_ => new Vector3(random(x1, x2), random(y1, y2), random(z1, z2)))
                .ToArray();
            var pointIndices = Enumerable.Range(vertices.Count, points.Length).ToArray();
            vertices.AddRange(points);
            planes.Add((v1, v2, v3, v4, pointIndices));
        }


        cube = new Model<int[]>(vertices.ToArray(), planes.ToArray());
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
        foreach(var (i1, i2, i3, i4, points) in cube.Quads) {
            var v1 = cube.GetVertex(i1);
            var v2 = cube.GetVertex(i2);
            var v3 = cube.GetVertex(i3);
            var v4 = cube.GetVertex(i4);
            var n = Extensions.GetNormal(v1, v2, v3);
            if(!c.IsVisible(v1, n)) continue;
            noStroke();
            c.quad3(v1, v2, v3, v4);
            stroke(White);
            strokeWeight(pointSize);
            foreach(var p in points) {
                var (x, y) = c.ProjectPoint(cube.GetVertex(p));
                point(x, y);
            }
        }
    }

    void keyPressed() {
        CameraExtensions.MoveCameraOnShepeOnKeyPressed(controller);
    }
}
