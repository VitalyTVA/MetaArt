using System.Numerics;
using MetaArt.D3;
using Vector = MetaArt.Vector;

namespace D3;
class Emergent {
    Scene<int[]> scene = null!;
    void setup() {
        size(600, 400);

        var density = 0.003f;
        var models = new[] {
            Loader.LoadModels("icosphere", new LoadOptions<VoidType>(scale: 500, invert: true)).Select(x => x.AddRandomPoints(density / 5)),
            
            //Loader.LoadModels("cubes", new LoadOptions<VoidType>(scale: 50)).Select(x => AddRandomPoints(x, density)),
            //Loader.LoadModels("heart_broken", new LoadOptions<VoidType>(scale: 150)).Select(x => x.AddRandomPoints(density / 1.5f)),
            Loader.LoadModels("heart", new LoadOptions<VoidType>(scale: 150)).Select(x => x.AddRandomPoints(density / 1.5f)),
            //Loader.LoadModels("primitives", new LoadOptions<VoidType>(scale: 50)).Select(x => AddRandomPoints(x, density)),
        }.SelectMany(x => x).ToArray();
        //models.Last().Translate = Vector3.UnitX * 10;
        foreach(var item in models) {
            //item.Rotation = new Quaternion(-0.37328503f, 0.57099724f, 0.23575023f, 0.6921285f);
        }
        scene = new Scene<int[]>(models);
    }

    YawPitchContoller controller = new();
    void draw() {
        if(isLeftMousePressed) {
            CameraExtensions.MoveOnShpereOnMousePressed(controller);
        }
        var c = controller.CreateCamera(600, 400);
        if(isRightMousePressed) {
            foreach(var model in scene.GetModels().Skip(1)) {
                model.Rotate(c, mouseXOffset, -mouseYOffset);

            }
        }

        EmergentHelper.Render(scene, c);
    }

    void keyPressed() {
        CameraExtensions.MoveOnShepeOnKeyPressed(controller);
    }
}
