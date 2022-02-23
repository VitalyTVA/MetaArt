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
            Loader.LoadModels("monolith_broken", new LoadOptions<VoidType>(scale: 20)).Select(x => x.AddRandomPoints(density / 1.5f)),
        }.SelectMany(x => x).ToArray();
        scene = new Scene<int[]>(models);
    }

    YawPitchContoller controller = new();

    //float offset = 0;

    void draw() {
        if(isMousePressed) {
            CameraExtensions.MoveOnShpereOnMousePressed(controller);
        }

        scene["Top"].Translate = new Vector3(0, 10f, 0);
        scene["Bottom"].Translate = new Vector3(0, -10f, 0);

        //if(offset < 200)
        //    offset += deltaTime / 10f;
        //scene["Right"].Translate = new Vector3(10 + offset, offset / 3f, offset / 1.5f);
        //var scale = 1 - offset / 500f;
        //scene["Right"].Scale = new Vector3(scale, scale, scale);

        var c = controller.CreateCamera(600, 400);
        EmergentHelper.Render(scene, c);
    }
}