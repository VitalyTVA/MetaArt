using MetaArt.Core;
using System.Numerics;

namespace ThatButtonAgain {
    public class GameController {
        public readonly Scene scene;
        readonly AnimationsController animations = new(new IAnimation[0]);

        public readonly float letterVerticalOffset;
        readonly float buttonWidth;
        readonly float buttonHeight;
        public readonly float letterSize;
        readonly float letterDragBoxSize;
        readonly float letterHorzStep;

        public GameController(float width, float height) {
            scene = new Scene(width, height);

            buttonWidth = scene.width * Constants.ButtonRelativeWidth;
            buttonHeight = buttonWidth * Constants.ButtonHeightRatio;
            letterSize = buttonHeight * Constants.LetterHeightRatio;
            letterDragBoxSize = buttonHeight * Constants.LetterDragBoxRatio;
            letterVerticalOffset = letterSize * Constants.LetterVerticalOffsetRatio;
            letterHorzStep = buttonWidth * Constants.LetterHorizontalStepRatio;

            levels = new[] {
            Level_TrivialClick,
            Level_DragLettersOntoButton,
        };
        }

        public void NextFrame(float deltaTime) {
            animations.Next(TimeSpan.FromMilliseconds(deltaTime));
        }

        void SetFadeIn() {
            var element = new FadeOutElement() { Opacity = 255 };
            var animation = new Animation<float, FadeOutElement> {
                Duration = Constants.FadeOutDuration,
                From = 255,
                To = 0,
                Target = element,
                SetValue = (target, value) => target.Opacity = value,
                Lerp = (range, amt) => MathFEx.Lerp(range.from, range.to, amt),
                OnEnd = () => {
                    scene.RemoveElement(element);
                }
            };
            animations.AddAnimation(animation);
            scene.AddElement(element);
        }

        void Level_DragLettersOntoButton() {
            var button = CreateButton();
            button.IsEnabled = false;
            scene.AddElement(button);

            var points = new[] {
                (-2.1f, 2.3f),
                (-1.5f, -2.7f),
                (.7f, -1.5f),
                (1.3f, 3.4f),
                (2.3f, -3.4f),
            };

            var letters = CreateLetters<DragableLetter>((letter, index) => {
                float margin = letterDragBoxSize * 2;
                letter.Rect = Rect.FromCenter(
                    //TODO Ensure letter box is within bounds
                    new Vector2(button.Rect.MidX + letterDragBoxSize * points[index].Item1, button.Rect.MidY + letterDragBoxSize * points[index].Item2),
                    new Vector2(letterDragBoxSize, letterDragBoxSize)
                );
                letter.TargetDragPoint = GetLetterTargetRect(index, button.Rect).Location;
                letter.HitTestVisible = true;
                letter.SnapDistance = letterSize * Constants.LetterSnapDistanceRatio;
            });

            SetFadeIn();

            animations.AddAnimation(new WaitConditionAnimation(
                condition: () => letters.All(l => MathFEx.VectorsEqual(l.Rect.Location, l.TargetDragPoint)),
                end: () => button.IsEnabled = true
            ));
        }

        void Level_TrivialClick() {
            var button = CreateButton();
            scene.AddElement(button);

            CreateLetters<Letter>((letter, index) => {
                letter.Rect = GetLetterTargetRect(index, button.Rect);
            });

            SetFadeIn();
        }

        Rect GetLetterTargetRect(int index, Rect buttonRect) =>
            Rect.FromCenter(
                buttonRect.Mid + new Vector2((index - 2) * letterHorzStep, 0),
                new Vector2(letterDragBoxSize, letterDragBoxSize)
            );

        Button CreateButton() {
            return new Button {
                Rect = new Rect(
                            scene.width / 2 - buttonWidth / 2,
                            scene.height / 2 - buttonHeight / 2,
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
                        Lerp = (range, amt) => MathFEx.Lerp(range.from, range.to, amt),
                        OnEnd = NextLevel
                    };
                    animations.AddAnimation(animation);
                    scene.AddElement(element);
                }
            };
        }

        TLetter[] CreateLetters<TLetter>(Action<TLetter, int> setUp) where TLetter : LetterBase, new() {
            int index = 0;
            var letters = new TLetter[5];
            foreach(var value in "TOUCH") {
                var letter = new TLetter() {
                    Value = value,
                };
                setUp(letter, index);
                scene.AddElement(letter);
                letters[index] = letter;
                index++;
            }
            return letters;
        }

        readonly Action[] levels;

        int levelIndex = 0;
        public void SetLevel(int level) {
            levelIndex = Math.Min(level, levels.Length - 1);
            
            scene.ClearElements();
            scene.AddElement(new Text { 
                Value = levelIndex.ToString(),
                Rect = new Rect(letterSize * Constants.LetterIndexOffsetRatioX, letterSize * Constants.LetterIndexOffsetRatioY, 0, 0)
            });
            levels[levelIndex]();
        }
        void NextLevel() {
            SetLevel(levelIndex + 1);
        }
    }
    static class Constants {
        public static float ButtonRelativeWidth => 0.6f;
        public static float ButtonHeightRatio => 1f / 3f;

        public static float LetterHeightRatio => 3f / 4f;
        public static float LetterVerticalOffsetRatio => 1f / 8f;
        public static float LetterDragBoxRatio => 3f / 4f;
        public static float LetterHorizontalStepRatio => 0.17f;

        //public static Color FadeOutColor = new Color(0, 0, 0);
        public static readonly TimeSpan FadeOutDuration = TimeSpan.FromMilliseconds(500);

        public static float LetterIndexOffsetRatioX => .3f;
        public static float LetterIndexOffsetRatioY => .5f;
        public static float LetterSnapDistanceRatio => .2f;
    }
}

