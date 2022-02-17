using System.Numerics;
using System.Reflection;
using MetaArt.D3;
using Vector = MetaArt.Vector;

namespace D3;
class LightCube {
    Scene<int> scene = null!;

    void setup() {
        size(600, 400);

        scene = Loader.LoadScene<int>("primitives", 50, info => info.LineIndex);
        //scene = Loader.LoadScene<int>("cubes", 50, info => info.LineIndex);

        //lights.lightsController.Yaw(PI / 2);

        //foreach(var item in scene.GetModels()) {
        //    item.Rotation = new Quaternion(0.41467953f, 0.323241f, 0.5681285f, -0.6330752f);
        //}
    }

    //YawPitchContoller cameraController = new(yaw: 2.04999971f, pitch: -0.759999931f);
    YawPitchContoller cameraController = new(yaw: 0, pitch: 0);

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
            foreach(var item in scene.GetModels()) {
                item.Rotate(c, dx, -dy);
            }
        }

        var lightCalulator = lights.GetLuminocityCalulator(c);

        foreach(var (i1, i2, i3, i4, _, vertices) in scene.GetQuads(c)) {
            var v1 = vertices[i1];
            var v2 = vertices[i2];
            var v3 = vertices[i3];
            var v4 = vertices[i4];
            var lum = lightCalulator(v1, v2, v3);
            var actualColor = color(
                byte.MaxValue * lum,
                byte.MaxValue * lum,
                byte.MaxValue * lum
            );
            fill(actualColor);
            CameraExtensions.quad3(v1, v2, v3, v4);
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
static class Loader {
    public static Scene<T> LoadScene<T>(string fileName, float scale, Func<QuadInfo, T> getValue) {
        return new Scene<T>(LoadModels(fileName, scale, getValue));
    }
    public static Model<T>[] LoadModels<T>(string fileName, float scale, Func<QuadInfo, T> getValue) {
        var asm = Assembly.GetExecutingAssembly();
        var models = ObjLoader.Load<T>(
            asm.GetManifestResourceStream(asm.GetName().Name + $".D3.Models.{fileName}.obj")!,
            getValue
        ).ToArray();
        foreach(var item in models) {
            item.Scale = new Vector3(scale, scale, scale);
        }
        return models;
    }

}

