using System.Numerics;
using MetaArt.D3;
using Vector = MetaArt.Vector;

namespace D3;
class ColorCube {
    Model<Color> cube = null!;
    void setup() {
        size(600, 400);

        cube = Extensions.CreateCube(100, (
            front: Colors.Red,
            back: Colors.Blue,
            left: Colors.Pink,
            right: Colors.Green,
            top: Colors.Yellow,
            bottom: Colors.Orange
        ));
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


        var lines = new[] {
            (0, 1), (1, 2), (2, 3), (3, 0),
            (4, 5), (5, 6), (6, 7), (7, 4),
            (0, 4), (1, 5), (2, 6), (3, 7),
        };
        foreach(var (from, to) in lines) {
            c.line3(
                cube.GetVertex(from),
                cube.GetVertex(to)
            );
        }

        foreach(var (v1, v2, v3, v4, color) in cube.Quads) {
            fill(color);
            c.quad3(
                cube.GetVertex(v1),
                cube.GetVertex(v2),
                cube.GetVertex(v3),
                cube.GetVertex(v4)
            );
        }
    }

    void keyPressed() {
        CameraExtensions.MoveCameraOnShepeOnKeyPressed(controller);
    }
}