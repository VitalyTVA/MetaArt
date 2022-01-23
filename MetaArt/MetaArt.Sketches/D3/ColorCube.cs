using System.Numerics;
using MetaArt.D3;
using Vector = MetaArt.Vector;

namespace D3;
class ColorCube {
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
        //vertices.RotateY(deltaTime / 1000f);

        var lines = new[] {
            (0, 1), (1, 2), (2, 3), (3, 0),
            (4, 5), (5, 6), (6, 7), (7, 4),
            (0, 4), (1, 5), (2, 6), (3, 7),
        };
        foreach(var (from, to) in lines) {
            c.line3(cube.Vertices[from], cube.Vertices[to]);
        }

        var colors = new[] {
            Colors.Red,
            Colors.Blue,
            Colors.Pink,
            Colors.Green,
            Colors.Yellow,
            Colors.Orange,
        };


        foreach(var ((v1, v2, v3, v4), color) in cube.Quads.Zip(colors, (quad, color) => (quad, color))) {
            fill(color);
            c.quad3(cube.Vertices[v1], cube.Vertices[v2], cube.Vertices[v3], cube.Vertices[v4]);
        }
    }

    void keyPressed() {
        CameraExtensions.MoveCameraOnShepeOnKeyPressed(controller);
    }
}