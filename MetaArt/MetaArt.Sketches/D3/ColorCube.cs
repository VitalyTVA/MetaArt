using System.Numerics;
using System.Reflection;
using MetaArt.D3;
using Vector = MetaArt.Vector;

namespace D3;
class ColorCube {
    Scene<Color> scene = null!;
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
        scene = new Scene<Color>(cube);
    }

    YawPitchContoller cameraController = new();

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

        foreach(var (i1, i2, i3, i4, col, vertices) in scene.GetQuads(c)) {
            fill(col);
            CameraExtensions.quad3(
                vertices[i1],
                vertices[i2],
                vertices[i3],
                vertices[i4]
            );
        }
    }

    void keyPressed() {
        CameraExtensions.MoveOnShepeOnKeyPressed(cameraController);
    }
}

//public class Lights {
//    public float Ambient;
//    public DirectionalLight Directional;
//}
//public record struct DirectionalLight(Vector3 deirection, float intensity);
