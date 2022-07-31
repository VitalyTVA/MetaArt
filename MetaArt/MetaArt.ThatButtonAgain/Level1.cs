using MetaArt.Core;
using System.Diagnostics;
using System.Numerics;

namespace ThatButtonAgain;
class Level1 {

    Game game = null!;
    void setup()
    {
        if(deviceType() == DeviceType.Desktop)
            size(400, 700);
        fullRedraw();
        textFont(createFont("SourceCodePro-Regular.ttf", 30));
        textAlign(TextAlign.CENTER, TextVerticalAlign.CENTER);
        rectMode(CORNER);
        game = new Game(width, height);

        const int buttonWidth = 100;
        const int buttonHeight = 50;

        game.AddElement(new Button {
            Rect = new Rect(
                width / displayDensity() / 2 - buttonWidth / 2,
                height / displayDensity() / 2 - buttonHeight / 2,
                buttonWidth,
                buttonHeight
            )
        });

        int index = 0;
        foreach (var letter in "TOUCH")
        {
            game.AddElement(new Letter() {
                Rect = new Rect(110 + index * 70, 110, 50, 50),
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
        game.Press(new System.Numerics.Vector2(mouseX / displayDensity(), mouseY / displayDensity()));
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
        game.Drag(new Vector2(mouseX / displayDensity(), mouseY / displayDensity()));
    }

    void mouseReleased()
    {
        //locked = false;
        game.Release();
        noLoop();
    }
}

