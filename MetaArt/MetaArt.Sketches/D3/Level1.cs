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
        scene["Bottom"].Translate = new Vector3(0, -15, 0);

        const float offset = 200f;
        var scale = -offset / 500f;
        var duration = TimeSpan.FromSeconds(3);
        animations = new(new[] {
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

class AnimationsController {
    readonly IAnimation[] animations;

    public AnimationsController(IAnimation[] animations) {
        this.animations = animations;
    }

    public void Next(TimeSpan deltaTime) {
        foreach(var animation in animations) {
            animation.Next(deltaTime);
        }
    }
}

public interface IAnimation {
    bool Next(TimeSpan deltaTime);
}

public sealed class Animation<T, TTarget> : IAnimation {
    public TimeSpan Duration { get; init; }
    public T From { get; init; } = default!;
    public T To { get; init; } = default!;
    public TTarget Target { get; init; } = default!;
    public Action<TTarget, T> SetValue { get; init; } = null!;
    public Func<(T from, T to), float, T> Lerp { get; init; } = null!;

    TimeSpan time = TimeSpan.Zero;
    public bool Next(TimeSpan deltaTime) {
        time += deltaTime;
        float amount = min(1, (float)(time.TotalMilliseconds / Duration.TotalMilliseconds));
        var value = Lerp((From, To), amount);
        SetValue(Target, value);
        return amount < 1;
    }
}

public static class Animations {
    public static IAnimation CreateModelTranslate<TValue>(Model<TValue> model, Vector3 delta, TimeSpan duration) {
        return new Animation<Vector3, Model<TValue>> {
            Duration = duration,
            From = model.Translate,
            To = model.Translate + delta,
            Target = model,
            SetValue = (target, value) => target.Translate = value,
            Lerp = Lerp
        };
    }

    public static IAnimation CreateModelScale<TValue>(Model<TValue> model, Vector3 delta, TimeSpan duration) {
        return new Animation<Vector3, Model<TValue>> {
            Duration = duration,
            From = model.Scale,
            To = model.Scale + delta,
            Target = model,
            SetValue = (target, value) => target.Scale = value,
            Lerp = Lerp
        };
    }
    static Vector3 Lerp((Vector3 from, Vector3 to) range, float amount) {
        return Vector3.Lerp(range.from, range.to, amount);
    }
}
