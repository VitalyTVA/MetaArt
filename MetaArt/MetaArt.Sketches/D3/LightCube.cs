using System.Numerics;
using MetaArt.D3;
using Vector = MetaArt.Vector;

namespace D3;
class LightCube {
    Model<Color> cube = null!;
    void setup() {
        size(600, 400);

        cube = Extensions.CreateCube(100, (
            front: Colors.White,
            back: Colors.White,
            left: Colors.White,
            right: Colors.White,
            top: Colors.White,
            bottom: Colors.White
        ));
    }

    YawPitchContoller cameraController = new();
    Lights lights = new Lights();

    void draw() {
        noSmooth();
        noStroke();
        noFill();
        background(0);
        CameraExtensions.InitCoords();

        var c = cameraController.CreateCamera();
        //vertices.RotateY(deltaTime / 1000f);

        var dx = mouseX - pmouseX;
        var dy = mouseY - pmouseY;

        if(isLeftMousePressed) {
            const float scale = 200;
            cameraController.Pitch(-dy / scale);
            cameraController.Yaw(-dx / scale);
        }
        if(isRightMousePressed) {
            cube.Rotate(c, dx, -dy);
        }


        var lightCalulator = lights.GetLuminocityCalulator();

        foreach(var (i1, i2, i3, i4, col) in cube.Quads) {
            var v1 = cube.GetVertex(i1);
            var v2 = cube.GetVertex(i2);
            var v3 = cube.GetVertex(i3);
            var v4 = cube.GetVertex(i4);
            var lum = lightCalulator(v1, v2, v3);
            var actualColor = color(
                col.Red * lum,
                col.Green * lum,
                col.Blue * lum
            );
            fill(actualColor);
            c.quad3(v1, v2, v3, v4);
        }
    }

    void keyPressed() {
        var delta = .05f;
        if(key == '+')
            lights.ChangeAmbient(delta);
        if(key == '-')
            lights.ChangeAmbient(-delta);

        CameraExtensions.MoveOnShepeOnKeyPressed(lights.lightsController);

    }
}
