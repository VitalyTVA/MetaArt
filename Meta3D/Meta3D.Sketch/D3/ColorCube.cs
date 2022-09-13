using System.Numerics;
using System.Reflection;
using MetaArt.D3;
using Vector = MetaArt.Vector;

namespace D3;
class ColorCube {
    Scene<Color> scene = null!;
    void setup() {
        size(600, 400);

        var colors = new[] {
            Colors.Red,
            Colors.Blue,
            Colors.Pink,
            Colors.Green,
            Colors.Yellow,
            Colors.Orange,
        };

        scene = Loader.LoadScene("cube", new LoadOptions<Color>(x => colors[x.Index / 2], scale: 1));
    }

    YawPitchContoller cameraController = new();

    void draw() {
        noSmooth();
        noStroke();
        noFill();
        background(0);
        CameraExtensions.InitCoords(100);

        var c = cameraController.CreateCamera(6, 4);
        //vertices.RotateY(deltaTime / 1000f);

        var dx = mouseX - pmouseX;
        var dy = mouseY - pmouseY;

        if(isLeftMousePressed) {
            const float scale = 200;
            cameraController.ChangePitch(-dy / scale);
            cameraController.ChangeYaw(-dx / scale);
        }
        if(isRightMousePressed) {
            scene.GetModels().Single().Rotate(c, dx, -dy);
        }

        foreach(var (i1, i2, i3, col, vertices, _) in scene.GetTriangles(c)) {
            fill(col);
            CameraExtensions.triangle(
                vertices[i1],
                vertices[i2],
                vertices[i3]
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
