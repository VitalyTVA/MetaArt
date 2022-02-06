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
    YawPitchContoller lightsController = new();
    float ambientLight = 0.5f;
    float directionalLight = 0.5f;

    void draw() {
        noSmooth();
        strokeWeight(1);
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


        var lines = new[] {
            (0, 1), (1, 2), (2, 3), (3, 0),
            (4, 5), (5, 6), (6, 7), (7, 4),
            (0, 4), (1, 5), (2, 6), (3, 7),
        };
        stroke(ambientLight * 255);
        foreach(var (from, to) in lines) {
            c.line3(
                cube.GetVertex(from),
                cube.GetVertex(to)
            );
        }
        Vector3 lightDirection = lightsController.GetDirection();

        foreach(var (i1, i2, i3, i4, col) in cube.Quads) {
            var v1 = cube.GetVertex(i1);
            var v2 = cube.GetVertex(i2);
            var v3 = cube.GetVertex(i3);
            var v4 = cube.GetVertex(i4);

            var norm = Vector3.Normalize(Extensions.GetNormal(v3, v2, v1));
            var lum = ambientLight + max(0, Vector3.Dot(lightDirection, norm)) * directionalLight;

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
            ambientLight += delta;
        if(key == '-')
            ambientLight -= delta;
        ambientLight = constrain(ambientLight, 0, 1);

        CameraExtensions.MoveOnShepeOnKeyPressed(lightsController);

    }
}

//public class Lights {
//    public float Ambient;
//    public DirectionalLight Directional;
//}
//public record struct DirectionalLight(Vector3 deirection, float intensity);
