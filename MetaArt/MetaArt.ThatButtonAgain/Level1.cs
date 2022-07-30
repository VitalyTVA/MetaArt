using System.Diagnostics;

namespace ThatButtonAgain;
class Level1 {

    Game game = null!;
    void setup()
    {
        //size(640, 360);
        fullRedraw();
        textFont(createFont("SourceCodePro-Regular.ttf", 30));
        textAlign(TextAlign.CENTER, TextVerticalAlign.CENTER);
        rectMode(CORNER);
        game = new Game(width, height);

        const int buttonWidth = 100;
        const int buttonHeight = 50;

        game.AddElement(new Button {
            Rect = new SKRect(
                width / displayDensity() / 2 - buttonWidth / 2,
                height / displayDensity() / 2 - buttonHeight / 2,
                width / displayDensity() / 2 + buttonWidth / 2,
                height / displayDensity() / 2 + buttonHeight / 2
            )
        });

        int index = 0;
        foreach (var letter in "TOUCH")
        {
            game.AddElement(new Letter() {
                Rect = SKRect.Create(110 + index * 70, 110, 50, 50),
                Value = letter
            });
            index++;
        }
        noLoop();
    }

    void draw()
    {
        background(0);
        scale(displayDensity(), displayDensity());

        foreach (var item in game.Elements) {
            switch (item) {
                case Button:
                    fill(White);
                    rect(item.Rect.Left, item.Rect.Top, item.Rect.Width, item.Rect.Height);
                    break;
                case Letter l:
                    fill(new Color(255, 0, 0));
                    rect(item.Rect.Left, item.Rect.Top, item.Rect.Width, item.Rect.Height);
                    fill(new Color(0, 255, 0));
                    text(l.Value.ToString(), item.Rect.MidX, item.Rect.MidY);
                    break;
            }
        }


        /*
        // Test if the cursor is over the box 
        if (mouseX > bx - boxSize && mouseX < bx + boxSize &&
            mouseY > by - boxSize && mouseY < by + boxSize)
        {
            overBox = true;
            if (!locked)
            {
                stroke(255);
                fill(153);
            }
        }
        else
        {
            stroke(153);
            fill(153);
            overBox = false;
        }

        // Draw the box
        rect(bx, by, boxSize, boxSize);
        */
    }

    void mousePressed()
    {
        /*
        if (overBox)
        {
            locked = true;
            fill(255, 255, 255);
        }
        else
        {
            locked = false;
        }
        xOffset = mouseX - bx;
        yOffset = mouseY - by;
        */
        game.Press(new SKPoint(mouseX / displayDensity(), mouseY / displayDensity()));
        loop();
    }

    void mouseDragged()
    {
        /*
        if (locked)
        {
            bx = mouseX - xOffset;
            by = mouseY - yOffset;
        }
        */
        game.Drag(new SKPoint(mouseX / displayDensity(), mouseY / displayDensity()));
    }

    void mouseReleased()
    {
        //locked = false;
        game.Release();
        noLoop();
    }
}

public class Game {
    readonly int width, height;

    InputState inputState;
    readonly NoInputState noInputState;


    public Game(int width, int height)
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

    public void Press(SKPoint point) {
        inputState = inputState.Press(point);
    }
    public void Drag(SKPoint point) {
        inputState = inputState.Drag(point);
    }
    public void Release() {
        inputState = inputState.Release();
    }
}

public abstract class Element {
    public SKRect Rect { get; set; }
    public virtual InputState? GetPressState(SKPoint startPoint, NoInputState releaseState) => null;
}

public class Button : Element {
    public bool IsEnabled { get; set; }
}

public class Letter : Element {
    public char Value { get; set; }
    public override InputState? GetPressState(SKPoint startPoint, NoInputState releaseState) {
        var startRect = Rect;
        return new DragInputState(startPoint, delta => {
            var newRect = startRect;
            newRect.Offset(delta);
            Rect = newRect;
            //Debug.WriteLine(delta.ToString());
        }, releaseState);
    }
}

public abstract class InputState {
    public abstract InputState Press(SKPoint point);
    public abstract InputState Release();
    public abstract InputState Drag(SKPoint point);
}

public class NoInputState : InputState {
    readonly Func<SKPoint, InputState> getPressState;

    public NoInputState(Func<SKPoint, InputState> getPressState) {
        this.getPressState = getPressState;
    }

    public override InputState Drag(SKPoint point) {
        return this;
    }

    public override InputState Press(SKPoint point) {
        return getPressState(point);
    }

    public override InputState Release() {
        throw new InvalidOperationException();
    }
}

public class DragInputState : InputState {
    readonly SKPoint startPoint;
    readonly Action<SKPoint> onDrag;
    readonly InputState releaseState;

    public DragInputState(SKPoint startPoint, Action<SKPoint> onDrag, InputState releaseState) {
        this.startPoint = startPoint;
        this.onDrag = onDrag;
        this.releaseState = releaseState;
    }

    public override InputState Drag(SKPoint point) {
        onDrag(point - startPoint);
        return this;
    }

    public override InputState Press(SKPoint point) {
        throw new InvalidOperationException();
    }

    public override InputState Release() {
        return releaseState;
    }
}

