using MetaArt.Core;
using System.Diagnostics;
using System.Numerics;

namespace ThatButtonAgain;
public class Level1 {

    Game game = null!;
    float letterVerticalOffset;
    void setup()
    {
        if(deviceType() == DeviceType.Desktop)
            size(400, 700);
        fullRedraw();
        textAlign(TextAlign.CENTER, TextVerticalAlign.CENTER);
        rectMode(CORNER);
        game = new Game(width / displayDensity(), height / displayDensity());

        var buttonWidth = game.width * Constants.ButtonRelativeWidth;
        var buttonHeight = buttonWidth * Constants.ButtonHeightRatio;
        var letterSize = buttonHeight * Constants.LetterHeightRatio;
        textFont(createFont("SourceCodePro-Regular.ttf", letterSize));
        var letterDragBoxSize = buttonHeight * Constants.LetterDragBoxRatio;
        letterVerticalOffset = letterSize * Constants.LetterVerticalOffsetRatio;

        var letterHorzStep = buttonWidth * Constants.LetterHorizontalStepRatio;


        var button = new Button {
            Rect = new Rect(
                game.width / 2 - buttonWidth / 2,
                game.height / 2 - buttonHeight / 2,
                buttonWidth,
                buttonHeight
            )
        };
        game.AddElement(button);

        int index = 0;
        foreach (var letter in "TOUCH")
        {
            game.AddElement(new Letter() {
                Rect = Rect.FromCenter(
                    button.Rect.Mid + new Vector2(( index - 2) * letterHorzStep, 0), 
                    new Vector2(letterDragBoxSize, letterDragBoxSize)
                ),
                Value = letter
            });
            index++;
        }
        noLoop();
    }

    void draw()
    {
        background(Constants.Background);
        scale(displayDensity(), displayDensity());
        noStroke();

        foreach (var item in game.Elements) {
            switch (item) {
                case Button b:
                    fill(b.IsPressed ? Constants.ButtonBackPressed : Constants.ButtonBackNormal);
                    rect(item.Rect.Left, item.Rect.Top, item.Rect.Width, item.Rect.Height);
                    break;
                case Letter l:
                    fill(Constants.LetterDragBox);
                    rect(item.Rect.Left, item.Rect.Top, item.Rect.Width, item.Rect.Height);
                    fill(Constants.LetterColor);
                    text(l.Value.ToString(), item.Rect.MidX, item.Rect.MidY - letterVerticalOffset);
                    break;
            }
        }
    }

    void mousePressed()
    {
        game.Press(new System.Numerics.Vector2(mouseX / displayDensity(), mouseY / displayDensity()));
        loop();
    }

    void mouseDragged()
    {
        game.Drag(new Vector2(mouseX / displayDensity(), mouseY / displayDensity()));
    }

    void mouseReleased()
    {
        //locked = false;
        game.Release();
        noLoop();
    }

    static class Constants {
        public static float ButtonRelativeWidth => 0.6f;
        public static float ButtonHeightRatio => 1f / 3f;

        public static float LetterHeightRatio => 3f / 4f;
        public static float LetterVerticalOffsetRatio => 1f / 8f;
        public static float LetterDragBoxRatio => 3f / 4f;
        public static float LetterHorizontalStepRatio => 0.17f;


        public static Color LetterColor => new Color(0, 0, 0);
        public static Color LetterDragBox => new Color(120, 0, 0, 10);
        public static Color Background => new Color(100, 100, 100);
        public static Color ButtonBackNormal => new Color(255, 255, 255);
        public static Color ButtonBackPressed => new Color(200, 200, 200);
    }
}

