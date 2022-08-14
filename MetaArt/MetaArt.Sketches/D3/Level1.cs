using System.Numerics;
using MetaArt.Core;
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
        scene["Bottom"].Translate = new Vector3(0, -15, 0);

        const float offset = 200f;
        var scale = -offset / 500f;
        var duration = TimeSpan.FromSeconds(2.5);
        //var rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, PI);
        animations = new();
        animations.AddAnimations(new[] {
            //Animations.CreateModelRotation(scene["Top"], rotation, duration),
            //Animations.CreateModelRotation(scene["Middle"], rotation, duration),
            //Animations.CreateModelRotation(scene["Bottom"], rotation, duration),
            Animations.CreateCameraControllerYaw(controller, PI, duration),
            Animations.CreateModelTranslate(scene["Top"], new Vector3(-offset, offset / 3, - offset / 2), duration),
            Animations.CreateModelScale(scene["Top"], new Vector3(scale, scale, scale), duration),
            Animations.CreateModelTranslate(scene["Bottom"], new Vector3(offset, - offset / 3, offset / 2), duration),
            Animations.CreateModelScale(scene["Bottom"], new Vector3(scale, scale, scale), duration),
        });
    }

    YawPitchContoller controller = new(pitchMax: PI / 2);

    AnimationsController animations = null!;

    void draw() {
        if(isMousePressed) {
            CameraExtensions.MoveOnShpereOnMousePressed(controller);
        }

        animations.Next(TimeSpan.FromMilliseconds(deltaTime));


        var c = controller.CreateCamera(600, 400);
        EmergentHelper.Render(scene, c);
    }
}

public static class Animations {
    public static IAnimation CreateModelTranslate<TValue>(Model<TValue> model, Vector3 delta, TimeSpan duration) {
        return new LerpAnimation<Vector3> {
            Duration = duration,
            From = model.Translate,
            To = model.Translate + delta,
            SetValue = value => model.Translate = value,
            Lerp = Lerp
        };
    }

    public static IAnimation CreateModelScale<TValue>(Model<TValue> model, Vector3 delta, TimeSpan duration) {
        return new LerpAnimation<Vector3> {
            Duration = duration,
            From = model.Scale,
            To = model.Scale + delta,
            SetValue = value => model.Scale = value,
            Lerp = Lerp
        };
    }

    public static IAnimation CreateCameraControllerYaw(YawPitchContoller controller, float delta, TimeSpan duration) {
        return new LerpAnimation<float> {
            Duration = duration,
            From = controller.Yaw,
            To = controller.Yaw + delta,
            SetValue = value => controller.SetYaw(value),
            Lerp = Sketch.lerp
        };
    }

    //public static IAnimation CreateModelRotation<TValue>(Model<TValue> model, Quaternion delta, TimeSpan duration) {
    //    if(model.Rotation != Quaternion.Identity)
    //        throw new InvalidOperationException();
    //    return new Animation<Quaternion, Model<TValue>> {
    //        Duration = duration,
    //        From = Quaternion.Identity,
    //        To = delta,
    //        Target = model,
    //        SetValue = (target, value) => target.Rotation = value,
    //        Lerp = (range, amount) => Quaternion.Slerp(range.from, range.to, amount)
    //    };
    //}
    static Vector3 Lerp(Vector3 from, Vector3 to, float amount) {
        return Vector3.Lerp(from, to, amount);
    }
}
