﻿
namespace MetaArt.Core;
public class Scene {
    public readonly float width, height;

    InputState inputState;
    readonly NoInputState noInputState;


    public Scene(float width, float height)
    {
        this.width = width;
        this.height = height;
        this.inputState = this.noInputState = new NoInputState(point => {
            for (int i = elements.Count - 1; i >= 0; i--) {
                var element = elements[i];
                if(element.IsVisible && element.HitTestVisible && element.Rect.Contains(point)) {
                    return (element.GetPressState?.Invoke(point, noInputState!) ?? inputState)!;
                }
            }
            return noInputState!;
        });
    }

    List<Element> elements = new();

    public IEnumerable<Element> VisibleElements => elements.Where(x => x.IsVisible);

    public void AddElement(Element element) {
        elements.Add(element);
    }
    public void AddElementBehind(Element element) {
        elements.Insert(0, element);
    }
    public bool RemoveElement(Element element) {
        return elements.Remove(element);
    }
    public void ClearElements() => elements.Clear();
    public void SendToBack(Element element) {
        if(!RemoveElement(element)) throw new InvalidOperationException();
        AddElementBehind(element);
    }

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
public abstract class Element {
     public static Func<Vector2, NoInputState, InputState> GetAnchorAndSnapDragStateFactory(
        Element element,
        Func<float> getAnchorDistance,
        Func<(float snapDistance, Vector2 snapPoint)?> getSnapInfo) {
        bool allowDrag = true;
        bool anchored = true;
        return (startPoint, releaseState) => {
            if (!allowDrag)
                return releaseState;

            var startRect = element.Rect;
            return new DragInputState(startPoint, delta => {
                Rect newRect = startRect;
                var anchorDistance = getAnchorDistance();
                if (!anchored || delta.LengthSquared() >= anchorDistance * anchorDistance) {
                    newRect = newRect.Offset(delta);
                    anchored = false;
                }
                var snapInfo = getSnapInfo();
                if (snapInfo != null && (newRect.Location - snapInfo.Value.snapPoint).LengthSquared() <= snapInfo.Value.snapDistance * snapInfo.Value.snapDistance) {
                    newRect = new Rect(snapInfo.Value.snapPoint, newRect.Size);
                    allowDrag = false;
                    element.HitTestVisible = false;
                }
                element.Rect = newRect;
                return allowDrag;
            }, releaseState);
        };
    }

     public static Func<Vector2, NoInputState, InputState> GetPressReleaseStateFactory(
        Element element,
        Action onPress,
        Action onRelease
     ) {
        return (startPoint, releaseState) => {
            if(!element.HitTestVisible)
                return releaseState;
            onPress();
            return new TapInputState(
                element,
                () => { },
                setState: isPressed => {
                    if(!isPressed) onRelease?.Invoke();
                },
                releaseState
            );
        };
    }

    public Rect Rect { get; set; }
    public bool HitTestVisible { get; set; }
    public bool IsVisible { get; set; } = true;
    public Func<Vector2, NoInputState, InputState>? GetPressState { get; set; }
}

public class FadeOutElement : Element {
    public float Opacity { get; set; }
    public FadeOutElement() {
        HitTestVisible = true;
    }
}

public class Button : Element {
    public bool IsEnabled { get; set; } = true;
    public bool IsPressed { get; set; }
}

public class Text : Element {
    public string Value { get; set; } = null!;
}


public class Letter : Element {
    public char Value { get; set; }
}

public class DragableButton : Element {
    public DragableButton() {
        HitTestVisible = true;
    }
}


public class InflateLetter : Letter {
    public float Scale { get; set; } = 1;
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
        return this;
    }
}

public class DragInputState : InputState {
    readonly Vector2 startPoint;
    readonly Func<Vector2, bool> onDrag;
    readonly InputState releaseState;

    public DragInputState(Vector2 startPoint, Func<Vector2, bool> onDrag, InputState releaseState) {
        this.startPoint = startPoint;
        this.onDrag = onDrag;
        this.releaseState = releaseState;
    }

    public override InputState Drag(Vector2 point) {
        if(onDrag(point - startPoint))
            return this;
        return releaseState;
    }

    public override InputState Press(Vector2 point) {
        throw new InvalidOperationException();
    }

    public override InputState Release() {
        return releaseState;
    }
}

public class TapInputState : InputState {
    readonly Element element;
    readonly Action onTap;
    readonly Action<bool> setState;
    readonly InputState releaseState;

    public TapInputState(Element element, Action onTap, Action<bool> setState, InputState releaseState) {
        this.element = element;
        this.onTap = onTap;
        this.setState = setState;
        this.releaseState = releaseState;
        setState(true);
    }

    public override InputState Drag(Vector2 point) {
        if(!element.Rect.Contains(point)) {
            setState(false);
            return releaseState;
        }
        return this;
    }

    public override InputState Press(Vector2 point) {
        throw new InvalidOperationException();
    }

    public override InputState Release() {
        setState(false);
        if(element.IsVisible)
            onTap();
        return releaseState;
    }
}

