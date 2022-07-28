namespace ThatButtonAgain;
class Level1 {

    Game game = null!;

    void setup()
    {
        //size(640, 360);
        fullRedraw();
        textFont(createFont("SourceCodePro-Regular.ttf", 100));
        textAlign(TextAlign.CENTER, TextVerticalAlign.CENTER);
        rectMode(CORNER);
        game = new Game(width, height);
        game.AddElement(new Button { Rect = SKRect.Create(100, 100, 400, 200) });

        int index = 0;
        foreach (var letter in "TOUCH")
        {
            game.AddElement(new Letter() {
                Rect = SKRect.Create(110 + index * 150, 110, 100, 100),
                Value = letter
            });
            index++;
        }
    }

    void draw()
    {
        background(0);

        foreach (var item in game.Elements)
        {
            switch (item) {
                case Button:
                    fill(White);
                    rect(item.Rect.Left, item.Rect.Top, item.Rect.Width, item.Rect.Height);
                    break;
                case Letter l:
                    fill(new Color(255, 0, 0));
                    rect(item.Rect.Left, item.Rect.Top, item.Rect.Width, item.Rect.Height);
                    fill(new Color(0, 255, 0));
                    text(l.Value.ToString(), item.Rect.MidX, item.Rect.MidY - 10);
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
    }

    void mouseReleased()
    {
        //locked = false;
    }
}

public class Game {
    readonly int width, height;

    public Game(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    List<Element> elements = new();

    public IEnumerable<Element> Elements => elements;

    public void AddElement(Element element) => elements.Add(element);
}

public abstract class Element {
    public SKRect Rect { get; set; }
}

public class Button : Element {
    public bool IsEnabled { get; set; }
}

public class Letter : Element {
    public char Value { get; set; }
}
