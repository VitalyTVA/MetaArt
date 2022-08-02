using MetaArt.Core;
using System.Diagnostics;
using System.Numerics;

namespace ThatButtonAgain;
public class Level1 {

    Game game = null!;
    AnimationsController animations = null!;
    float letterVerticalOffset;
    float buttonWidth;
    float buttonHeight;
    float letterSize;
    float letterDragBoxSize;
    float letterHorzStep;
    void setup() {
        if(deviceType() == DeviceType.Desktop)
            size(400, 700);
        fullRedraw();
        textAlign(TextAlign.CENTER, TextVerticalAlign.CENTER);
        rectMode(CORNER);
        game = new Game(width / displayDensity(), height / displayDensity());
        animations = new(new IAnimation[0]);

        buttonWidth = game.width * Constants.ButtonRelativeWidth;
        buttonHeight = buttonWidth * Constants.ButtonHeightRatio;
        letterSize = buttonHeight * Constants.LetterHeightRatio;
        textFont(createFont("SourceCodePro-Regular.ttf", letterSize));
        letterDragBoxSize = buttonHeight * Constants.LetterDragBoxRatio;
        letterVerticalOffset = letterSize * Constants.LetterVerticalOffsetRatio;
        letterHorzStep = buttonWidth * Constants.LetterHorizontalStepRatio;

        SetUpLevel1();
    }


    void SetFadeIn() {
        var element = new FadeOutElement() { Opacity = 255 };
        var animation = new Animation<float, FadeOutElement> {
            Duration = Constants.FadeOutDuration,
            From = 255,
            To = 0,
            Target = element,
            SetValue = (target, value) => target.Opacity = value,
            Lerp = (range, amt) => lerp(range.from, range.to, amt),
            OnEnd = () => {
                game.RemoveElement(element);
            }
        };
        animations.AddAnimation(animation);
        game.AddElement(element);
    }


    Button CreateButton() {
        return new Button {
            Rect = new Rect(
                        game.width / 2 - buttonWidth / 2,
                        game.height / 2 - buttonHeight / 2,
                        buttonWidth,
                        buttonHeight
                    ),
            HitTestVisible = true,
            Click = () => {
                var element = new FadeOutElement();
                var animation = new Animation<float, FadeOutElement> {
                    Duration = Constants.FadeOutDuration,
                    From = 0,
                    To = 255,
                    Target = element,
                    SetValue = (target, value) => target.Opacity = value,
                    Lerp = (range, amt) => lerp(range.from, range.to, amt),
                    OnEnd = () => {
                        SetUpLevel2();
                    }
                };
                animations.AddAnimation(animation);
                game.AddElement(element);
            }
        };
    }

    void SetUpLevel2() {
        game.ClearElements();
        var button = CreateButton();
        game.AddElement(button);

        CreateLetters((letter, index) => {
            float margin = letterDragBoxSize * 2;
            letter.Rect = Rect.FromCenter(
                            new Vector2(MathFEx.Random(margin, game.width - margin), MathFEx.Random(margin, game.height - margin)),
                            new Vector2(letterDragBoxSize, letterDragBoxSize)
                        );
            letter.HitTestVisible = true;
        });

        SetFadeIn();
    }

    void SetUpLevel1() {
        game.ClearElements();
        var button = CreateButton();
        game.AddElement(button);

        CreateLetters((letter, index) => {
            letter.Rect = Rect.FromCenter(
                            button.Rect.Mid + new Vector2((index - 2) * letterHorzStep, 0),
                            new Vector2(letterDragBoxSize, letterDragBoxSize)
                        );
        });

        SetFadeIn();
    }

    void CreateLetters(Action<Letter, int> setUp) {
        int index = 0;
        foreach (var value in "TOUCH") {
            var letter = new Letter() {
                Value = value,
            };
            setUp(letter, index);
            game.AddElement(letter);
            index++;
        }
    }

    void draw()
    {
        animations.Next(TimeSpan.FromMilliseconds(deltaTime));
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
                case FadeOutElement f:
                    fill(0, f.Opacity);
                    rect(0, 0, game.width, game.height);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        if(isMousePressed || animations.HasAnimations)
            loop();
        else
            noLoop();
    }

    void mousePressed()
    {
        game.Press(new System.Numerics.Vector2(mouseX / displayDensity(), mouseY / displayDensity()));
        //loop();
    }

    void mouseDragged()
    {
        game.Drag(new Vector2(mouseX / displayDensity(), mouseY / displayDensity()));
    }

    void mouseReleased()
    {
        //locked = false;
        game.Release();
        //noLoop();
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

        //public static Color FadeOutColor = new Color(0, 0, 0);
        public static readonly TimeSpan FadeOutDuration = TimeSpan.FromMilliseconds(500);
    }
}

