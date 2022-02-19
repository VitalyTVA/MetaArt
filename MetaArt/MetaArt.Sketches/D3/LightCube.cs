using System.Numerics;
using System.Reflection;
using MetaArt.D3;
using Vector = MetaArt.Vector;

namespace D3;
class LightCube {
    Scene<int> scene = null!;

    void setup() {
        size(600, 400);

        var models = new[] {
            Loader.LoadModels<int>("icosphere", new LoadOptions<int>(info => info.LineIndex, 10, invert: true)),
            //Loader.LoadModels<int>("monkey", new LoadOptions<int>(info => info.LineIndex, 3))
            //Loader.LoadModels<int>("heart", new LoadOptions<int>(info => info.LineIndex, 3))
            Loader.LoadModels<int>("primitives", new LoadOptions<int>(info => info.LineIndex, 1))
        }.SelectMany(x => x).ToArray();
        scene = new Scene<int>(models);
        //scene = Loader.LoadScene<int>("cilinder", new LoadOptions<int>(info => info.LineIndex, 5, invert: true));

        //scene = Loader.LoadScene<int>("primitives", new LoadOptions<int>(info => info.LineIndex, 1));
        //scene = Loader.LoadScene<int>("cubes", 50, info => info.LineIndex);

        //lights.lightsController.Yaw(PI / 2);

        //foreach(var item in scene.GetModels()) {
        //    item.Rotation = new Quaternion(0.41467953f, 0.323241f, 0.5681285f, -0.6330752f);
        //}

        lights.lightsController.Yaw(PI / 4);
        lights.lightsController.Pitch(-PI / 4);
    }

    //YawPitchContoller cameraController = new(yaw: 2.04999971f, pitch: -0.759999931f);
    YawPitchContoller cameraController = new(yaw: 0, pitch: 0);

    Lights lights = new Lights();

    void draw() {
        noSmooth();
        noStroke();
        noFill();
        background(0);
        CameraExtensions.InitCoords(50);

        var c = cameraController.CreateCamera(12, 8);
        //vertices.RotateY(deltaTime / 1000f);

        var dx = mouseX - pmouseX;
        var dy = mouseY - pmouseY;

        if(isLeftMousePressed) {
            const float scale = 200;
            cameraController.Pitch(-dy / scale);
            cameraController.Yaw(-dx / scale);
            lights.lightsController.Pitch(-dy / scale);
            lights.lightsController.Yaw(-dx / scale);

        }
        if(isRightMousePressed) {
            foreach(var item in scene.GetModels().Skip(1)) {
                item.Rotate(c, dx, -dy);
            }
        }

        var lightCalulator = lights.GetLuminocityCalulator(c);

        foreach(var (i1, i2, i3, _, vertices, normalVertices) in scene.GetTriangles(c)) {
            var lum = lightCalulator(normalVertices[i1], normalVertices[i2], normalVertices[i3]);
            var actualColor = color(
                byte.MaxValue * lum,
                byte.MaxValue * lum,
                byte.MaxValue * lum
            );
            fill(actualColor);
            var v1 = vertices[i1];
            var v2 = vertices[i2];
            var v3 = vertices[i3];
            CameraExtensions.triangle(v1, v2, v3);
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
    public static Scene<T> LoadScene<T>(string fileName, LoadOptions<T> options) {
        return new Scene<T>(LoadModels(fileName, options));
    }
    public static Model<T>[] LoadModels<T>(string fileName, LoadOptions<T> options) {
        var asm = Assembly.GetExecutingAssembly();
        var models = ObjLoader.Load(
            asm.GetManifestResourceStream(asm.GetName().Name + $".D3.Models.{fileName}.obj")!,
            options
        ).ToArray();
        return models;
    }

}

