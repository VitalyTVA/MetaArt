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
                Level_16xClick,
                Level_RotationsGroup,
                Level_LettersBehindButton,
                Level_ClickInsteadOfTouch,
            };
        }

        public void NextFrame(float deltaTime) {
            animations.Next(TimeSpan.FromMilliseconds(deltaTime));
        }

        void Level_TrivialClick() {
            var button = CreateButton(StartNextLevelAnimation);
            scene.AddElement(button);

            CreateLetters<Letter>((letter, index) => {
                letter.Rect = GetLetterTargetRect(index, button.Rect);
            });
        }

        void Level_DragLettersOntoButton() {
            var button = CreateButton(StartNextLevelAnimation);
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
            animations.AddAnimation(new WaitConditionAnimation(
                condition: GetAreLettersInPlaceCheck(button.Rect, letters),
                end: () => button.IsEnabled = true
            ));
        }

        void Level_16xClick() {
            var words = new[] {
                "Touch", //16
                "ToucH", 
                "TouCh", 
                "TouCH", 
                "ToUch", 
                "ToUcH", 
                "ToUCh", 
                "ToUCH", 

                "TOuch", //24
                "TOucH", 
                "TOuCh", 
                "TOuCH",
                "TOUch",
                "TOUcH",
                "TOUCh",
                "TOUCH", //32
            };
            int clickIndex = 0;
            Letter[] letters = null!;
            void SetLetters() {
                for(int i = 0; i < 5; i++) {
                    letters[i].Value = words![clickIndex][i]; 
                }
            };
            var button = CreateButton(() => {
                if(clickIndex < words.Length - 1) { 
                    clickIndex++;
                    SetLetters();
                } else {
                    StartNextLevelAnimation();
                }
            });
            scene.AddElement(button);

            letters = CreateLetters<Letter>((letter, index) => {
                letter.Rect = GetLetterTargetRect(index, button.Rect);
            });
            SetLetters();
        }

        void Level_RotationsGroup() {
            var button = CreateButton(StartNextLevelAnimation);
            button.HitTestVisible = false;
            scene.AddElement(button);

            //01234
            //21034
            //21430
            //41230
            //43210
            var indices = new[] { 4, 3, 2, 1, 0 };
            PressActionLetter[] letters = null!;
            bool rotating = false;
            letters = CreateLetters<PressActionLetter>((letter, index) => {
                letter.Rect = GetLetterTargetRect(indices[index], button.Rect);
                letter.OnPress = () => {
                    if(rotating)
                        return;

                    var leftLetter = letters!.OrderByDescending(x => x.Rect.Left).FirstOrDefault(x => MathFEx.Less(x.Rect.Left, letter.Rect.Left));
                    var rightLetter = letters!.OrderBy(x => x.Rect.Left).FirstOrDefault(x => MathFEx.Greater(x.Rect.Left, letter.Rect.Left));
                    if(leftLetter == null || rightLetter == null)
                        return;

                    rotating = true;

                    void AddRotateAnimation(float fromAngle, float toAngle, PressActionLetter sideLetter) {
                        var animation = new RotateAnimation {
                            Duration = Constants.RotateAroundLetterDuration,
                            From = fromAngle,
                            To = toAngle,
                            Center = letter.Rect.Mid,
                            Radius = (sideLetter.Rect.Mid - letter.Rect.Mid).Length(),
                            SetLocation = center => {
                                sideLetter.Rect = Rect.FromCenter(center, sideLetter.Rect.Size);
                            },
                            OnEnd = () => rotating = false
                        };
                        animations.AddAnimation(animation);
                    };

                    AddRotateAnimation(0, MathFEx.PI, rightLetter);
                    AddRotateAnimation(MathFEx.PI, MathFEx.PI * 2, leftLetter);
                };
                letter.HitTestVisible = true;
            });

            animations.AddAnimation(new WaitConditionAnimation(
                condition: GetAreLettersInPlaceCheck(button.Rect, letters),
                end: () => {
                    button.HitTestVisible = true;
                    foreach(var item in letters) {
                        item.HitTestVisible = false;
                    }
                }));
        }

        void Level_LettersBehindButton() {
           var buttonRect = GetButtonRect();

            var letters = CreateLetters<Letter>((letter, index) => {
                letter.Rect = GetLetterTargetRect(index, buttonRect);
            });

            float snapDistance = buttonHeight * Constants.ButtonAnchorDistanceRatio;
            var dragableButton = new DragableButton { 
                Rect = buttonRect,
                AnchorDistance = snapDistance,
            };
            scene.AddElement(dragableButton);

            animations.AddAnimation(new WaitConditionAnimation(
                condition: () => letters.All(l => !l.Rect.Intersects(dragableButton.Rect)),
                end: () => {
                    scene.SendToBack(dragableButton);
                    dragableButton.SnapInfo = (snapDistance, buttonRect.Location);
                    animations.AddAnimation(new WaitConditionAnimation(
                        condition: () => MathFEx.VectorsEqual(dragableButton.Rect.Location, buttonRect.Location),
                        end: () => {
                            scene.RemoveElement(dragableButton);
                            scene.AddElementBehind(CreateButton(NextLevel));
                        }));
                }));
        }

        void Level_ClickInsteadOfTouch() {
            var button = CreateButton(StartNextLevelAnimation);
            button.HitTestVisible = false;
            scene.AddElement(button);

            var indices = new[] { 0, 4, 2, 1 };
            int replaceIndex = 0;

            var letters = CreateLetters<InflateLetter>((letter, index) => {
                float margin = letterDragBoxSize * 2;
                letter.Rect = GetLetterTargetRect(index, button.Rect);
                letter.HitTestVisible = true;
                LerpAnimation<float> animation = null!;
                letter.OnPress = () => {
                    if(replaceIndex == indices.Length || indices[replaceIndex] != index)
                        return;
                    animation = new() {
                        From = 1,
                        To = 2,
                        Duration = Constants.InflateButtonDuration,
                        Lerp = (range, amt) => MathFEx.Lerp(range.from, range.to, amt),
                        OnEnd = () => {
                            letter.Value = "TOUCH"[index];
                            letter.Scale = 1;
                            letter.HitTestVisible = false;
                            replaceIndex++;
                        },
                        SetValue = value => letter.Scale = value
                    };
                    animations.AddAnimation(animation);
                };
                letter.OnRelease = () => {
                    animations.RemoveAnimation(animation);
                    letter.Scale = 1;
                };
            }, "CLICK");
            animations.AddAnimation(new WaitConditionAnimation(
                condition: () => replaceIndex == 4,
                end: () => {
                    button.HitTestVisible = true;
                    foreach(var item in letters) {
                        item.HitTestVisible = false;
                    }
                }));
        }

        Func<bool> GetAreLettersInPlaceCheck(Rect buttonRect, LetterBase[] letters) {
            var targetLocations = GetLettersTargetLocations(buttonRect);
            return () => letters.Select((l, i) => (l, i)).All(x => MathFEx.VectorsEqual(x.l.Rect.Location, targetLocations[x.i]));
        }

        Vector2[] GetLettersTargetLocations(Rect buttonRect) => 
            Enumerable.Range(0, 5)
                .Select(index => GetLetterTargetRect(index, buttonRect).Location)
                .ToArray();

        Rect GetLetterTargetRect(int index, Rect buttonRect) =>
            Rect.FromCenter(
                buttonRect.Mid + new Vector2((index - 2) * letterHorzStep, 0),
                new Vector2(letterDragBoxSize, letterDragBoxSize)
            );

        Button CreateButton(Action click) {
            return new Button {
                Rect = GetButtonRect(),
                HitTestVisible = true,
                Click = click
            };
        }

        Rect GetButtonRect() {
            return new Rect(
                                        scene.width / 2 - buttonWidth / 2,
                                        scene.height / 2 - buttonHeight / 2,
                                        buttonWidth,
                                        buttonHeight
                                    );
        }

        void StartFade(float from, float to, Action end) {
            var element = new FadeOutElement() { Rect = new Rect(0, 0, scene.width, scene.height), Opacity = from };
            var animation = new LerpAnimation<float> {
                Duration = Constants.FadeOutDuration,
                From = from,
                To = to,
                SetValue = value => element.Opacity = value,
                Lerp = (range, amt) => MathFEx.Lerp(range.from, range.to, amt),
                OnEnd = () => {
                    scene.RemoveElement(element);
                    end();
                }
            };
            animations.AddAnimation(animation);
            scene.AddElement(element);
        }
        void StartNextLevelAnimation() {
            StartFade(0, 255, NextLevel);
        }

        TLetter[] CreateLetters<TLetter>(Action<TLetter, int> setUp, string word = "TOUCH") where TLetter : LetterBase, new() {
            int index = 0;
            var letters = new TLetter[5];
            foreach(var value in word) {
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
            StartFade(255, 0, () => { });
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
        public static TimeSpan FadeOutDuration => TimeSpan.FromMilliseconds(500);
        public static TimeSpan RotateAroundLetterDuration => TimeSpan.FromMilliseconds(500);
        public static TimeSpan InflateButtonDuration => TimeSpan.FromMilliseconds(1500);

        public static float LetterIndexOffsetRatioX => .3f;
        public static float LetterIndexOffsetRatioY => .5f;
        public static float LetterSnapDistanceRatio => .2f;
        public static float ButtonAnchorDistanceRatio => .2f;
    }
}

