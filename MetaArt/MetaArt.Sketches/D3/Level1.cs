using System.Numerics;
using MetaArt.D3;
using Vector = MetaArt.Vector;

namespace D3;
class Level1 {
    Scene<int[]> scene = null!;
    void setup() {
        size(600, 400);

        var density = 0.004f;
        var models = new[] {
            Loader.LoadModels("icosphere", new LoadOptions<VoidType>(scale: 500, invert: true)).Select(x => x.AddRandomPoints(density / 5)),
            Loader.LoadModels("heart_broken", new LoadOptions<VoidType>(scale: 100)).Select(x => x.AddRandomPoints(density / 1.5f)),
        }.SelectMany(x => x).ToArray();
        scene = new Scene<int[]>(models);
    }

    YawPitchContoller controller = new();
    void draw() {
        if(isMousePressed) {
            CameraExtensions.MoveOnShpereOnMousePressed(controller);
        }
        var c = controller.CreateCamera(600, 400);
        EmergentHelper.Render(scene, c);
    }
}