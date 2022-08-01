
namespace MetaArt.Core;
public class Game {
    public readonly float width, height;

    InputState inputState;
    readonly NoInputState noInputState;


    public Game(float width, float height)
    {
        this.width = width;
        this.height = height;
        this.inputState = this.noInputState = new NoInputState(point => {
            for (int i = elements.Count - 1; i >= 0; i--) {
                var element = elements[i];
                if (element.Rect.Contains(point)) {
                    return (element.GetPressState(point, noInputState!) ?? inputState)!;
                }
            }
            return noInputState!;
        });
    }

    List<Element> elements = new();

    public IEnumerable<Element> Elements => elements;

    public void AddElement(Element element) => elements.Add(element);

    public void Press(Vector2 point) {
        inputState = inputState.Press(point);
    }
    public void Drag(Vector2 point) {
        inputState = inputState.Drag(point);
    }
    public void Release() {
        inputState = inputState.Release();
    }
}

public record struct Rect(Vector2 Location, Vector2 Size) {
    public static Rect FromCenter(Vector2 center, Vector2 size) => new Rect(center - size / 2, size);

    public float Left => Location.X;
    public float Top => Location.Y;
    public float Width => Size.X;
    public float Height => Size.Y;
    public Vector2 Mid => Location + Size / 2;
    public float MidX => Location.X + Size.X / 2;
    public float MidY => Location.Y + Size.Y / 2;

    public Rect(float left, float top, float width, float height)
        : this(new Vector2(left, top), new Vector2(width, height)) { }

    public Rect Offset(Vector2 offset) 
        => new Rect(Location + offset, Size);

    public bool Contains(Vector2 point) => 
        MathFEx.LessOrEqual(Location.X, point.X) &&
        MathFEx.LessOrEqual(Location.Y, point.Y) &&
        MathFEx.LessOrEqual(point.X, Location.X + Size.X) &&
        MathFEx.LessOrEqual(point.Y, Location.Y + Size.Y);
}

public abstract class Element {
    public Rect Rect { get; set; }
    public virtual InputState? GetPressState(Vector2 startPoint, NoInputState releaseState) => null;
}

public class Button : Element {
    public bool IsEnabled { get; set; }
    public bool IsPressed { get; set; }
    public override InputState? GetPressState(Vector2 startPoint, NoInputState releaseState) {
        return new TapInputState(
            () => {
                Debug.WriteLine("Click");
            },
            setState: isPressed => IsPressed = isPressed,
            releaseState);
    }
}

public class Letter : Element {
    public char Value { get; set; }
    public override InputState? GetPressState(Vector2 startPoint, NoInputState releaseState) {
        var startRect = Rect;
        return new DragInputState(startPoint, delta => {
            Rect = startRect.Offset(delta);
            //Debug.WriteLine(delta.ToString());
        }, releaseState);
    }
}

public abstract class InputState {
    public abstract InputState Press(Vector2 point);
    public abstract InputState Release();
    public abstract InputState Drag(Vector2 point);
}

public class NoInputState : InputState {
    readonly Func<Vector2, InputState> getPressState;

    public NoInputState(Func<Vector2, InputState> getPressState) {
        this.getPressState = getPressState;
    }

    public override InputState Drag(Vector2 point) {
        return this;
    }

    public override InputState Press(Vector2 point) {
        return getPressState(point);
    }

    public override InputState Release() {
        throw new InvalidOperationException();
    }
}

public class DragInputState : InputState {
    readonly Vector2 startPoint;
    readonly Action<Vector2> onDrag;
    readonly InputState releaseState;

    public DragInputState(Vector2 startPoint, Action<Vector2> onDrag, InputState releaseState) {
        this.startPoint = startPoint;
        this.onDrag = onDrag;
        this.releaseState = releaseState;
    }

    public override InputState Drag(Vector2 point) {
        onDrag(point - startPoint);
        return this;
    }

    public override InputState Press(Vector2 point) {
        throw new InvalidOperationException();
    }

    public override InputState Release() {
        return releaseState;
    }
}

public class TapInputState : InputState {
    readonly Action onTap;
    readonly Action<bool> setState;
    readonly InputState releaseState;

    public TapInputState(Action onTap, Action<bool> setState, InputState releaseState) {
        this.onTap = onTap;
        this.setState = setState;
        this.releaseState = releaseState;
        setState(true);
    }

    public override InputState Drag(Vector2 point) {
        return this;
    }

    public override InputState Press(Vector2 point) {
        throw new InvalidOperationException();
    }

    public override InputState Release() {
        setState(false);
        onTap();
        return releaseState;
    }
}

