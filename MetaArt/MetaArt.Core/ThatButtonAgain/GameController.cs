using MetaArt.Core;
using System.Numerics;

namespace ThatButtonAgain {
    public enum SoundKind {
        Win,
    }
    public class GameController {
        public readonly Scene scene;
        readonly AnimationsController animations = new(new IAnimation[0]);

        public readonly float letterVerticalOffset;
        readonly float buttonWidth;
        readonly float buttonHeight;
        public readonly float letterSize;
        readonly float letterDragBoxSize;
        readonly float letterHorzStep;
        readonly Action<SoundKind> playSound;

        public GameController(float width, float height, Action<SoundKind> playSound) {
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
                Level_RandomButton,
                Level_ReflectedButton,
            };
            this.playSound = playSound;
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

            var letters = CreateLetters<Letter>((letter, index) => {
                float margin = letterDragBoxSize * 2;
                letter.Rect = Rect.FromCenter(
                    //TODO Ensure letter box is within bounds
                    new Vector2(button.Rect.MidX + letterDragBoxSize * points[index].Item1, button.Rect.MidY + letterDragBoxSize * points[index].Item2),
                    new Vector2(letterDragBoxSize, letterDragBoxSize)
                );
                letter.HitTestVisible = true;
                letter.GetPressState = Element.GetAnchorAndSnapDragStateFactory(
                    letter, 
                    () => 0, () => (letterSize * Constants.LetterSnapDistanceRatio, GetLetterTargetRect(index, button.Rect).Location)
                );
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
            Letter[] letters = null!;
            bool rotating = false;
            letters = CreateLetters<Letter>((letter, index) => {
                letter.Rect = GetLetterTargetRect(indices[index], button.Rect);
                var onPress = () => {
                    if(rotating)
                        return;

                    var leftLetter = letters!.OrderByDescending(x => x.Rect.Left).FirstOrDefault(x => MathFEx.Less(x.Rect.Left, letter.Rect.Left));
                    var rightLetter = letters!.OrderBy(x => x.Rect.Left).FirstOrDefault(x => MathFEx.Greater(x.Rect.Left, letter.Rect.Left));
                    if(leftLetter == null || rightLetter == null)
                        return;

                    rotating = true;

                    void AddRotateAnimation(float fromAngle, float toAngle, Letter sideLetter) {
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
                letter.GetPressState = Element.GetPressReleaseStateFactory(letter, onPress, () => { });
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
            (float, Vector2)? snapInfo = default;
            float snapDistance = buttonHeight * Constants.ButtonAnchorDistanceRatio;
            var dragableButton = new DragableButton { 
                Rect = buttonRect,
            };
            dragableButton.GetPressState = Element.GetAnchorAndSnapDragStateFactory(dragableButton, () => snapDistance, () => snapInfo);

            scene.AddElement(dragableButton);

            animations.AddAnimation(new WaitConditionAnimation(
                condition: deltaTime => letters.All(l => !l.Rect.Intersects(dragableButton.Rect)),
                end: () => {
                    scene.SendToBack(dragableButton);
                    snapInfo = (snapDistance, buttonRect.Location);
                    animations.AddAnimation(new WaitConditionAnimation(
                        condition: deltaTime => MathFEx.VectorsEqual(dragableButton.Rect.Location, buttonRect.Location),
                        end: () => {
                            scene.RemoveElement(dragableButton);
                            scene.AddElementBehind(CreateButton(StartNextLevelAnimation));
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
                var onPress = () => {
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
                var onRelease = () => {
                    animations.RemoveAnimation(animation);
                    letter.Scale = 1;
                };
                letter.GetPressState = Element.GetPressReleaseStateFactory(letter, onPress, onRelease);

            }, "CLICK");
            animations.AddAnimation(new WaitConditionAnimation(
                condition: deltaTime => replaceIndex == 4,
                end: () => {
                    button.HitTestVisible = true;
                    foreach(var item in letters) {
                        item.HitTestVisible = false;
                    }
                }));
        }

        void Level_RandomButton() {

            var button = CreateButton(StartNextLevelAnimation);
            scene.AddElement(button);
            var letters = CreateLetters<Letter>((letter, index) => {
                letter.Rect = GetLetterTargetRect(index, button.Rect);
            });

            void SetVisibility(bool visible) {
                button!.IsVisible = visible;
                foreach(var letter in letters!) {
                    letter.IsVisible = visible;
                }
            }

            SetVisibility(false);

            var appearInterval = Constants.MinButtonAppearInterval;

            void StartWaitButton() {
                animations.AddAnimation(WaitConditionAnimation.WaitTime(
                    TimeSpan.FromMilliseconds(MathFEx.Random(Constants.MinButtonInvisibleInterval, Constants.MaxButtonInvisibleInterval)),
                    () => {
                        SetVisibility(true);
                        animations.AddAnimation(WaitConditionAnimation.WaitTime(
                            TimeSpan.FromMilliseconds(appearInterval),
                            () => {
                                appearInterval = Math.Min(appearInterval + Constants.ButtonAppearIntervalIncrease, Constants.MaxButtonAppearInterval);
                                SetVisibility(false);
                                StartWaitButton();
                            }
                        ));
                    }
                ));
            };

            StartWaitButton();

        }

        void Level_ReflectedButton() {
            var buttonRect = GetButtonRect();

            float snapDistance = buttonHeight * Constants.ButtonAnchorDistanceRatio;
            var dragableButton = new DragableButton {
                Rect = buttonRect,
            };
            scene.AddElement(dragableButton);

            var letters = CreateLetters<Letter>((letter, index) => {
                letter.Rect = GetLetterTargetRect(index, buttonRect);
            });
        }

        Func<TimeSpan, bool> GetAreLettersInPlaceCheck(Rect buttonRect, Letter[] letters) {
            var targetLocations = GetLettersTargetLocations(buttonRect);
            return deltaTime => letters.Select((l, i) => (l, i)).All(x => MathFEx.VectorsEqual(x.l.Rect.Location, targetLocations[x.i]));
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
            var button = new Button {
                Rect = GetButtonRect(),
                HitTestVisible = true,
            };
            button.GetPressState = (startPoint, releaseState) => {
                return new TapInputState(
                    button,
                    () => {
                        if(button.IsEnabled)
                            click();
                    },
                    setState: isPressed => button.IsPressed = isPressed,
                    releaseState
                );
            };
            return button;
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
            StartFade(0, 255, () => SetLevel(levelIndex + 1));
            playSound(SoundKind.Win);
        }

        TLetter[] CreateLetters<TLetter>(Action<TLetter, int> setUp, string word = "TOUCH") where TLetter : Letter, new() {
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

        public static float MinButtonInvisibleInterval => 1000;
        public static float MaxButtonInvisibleInterval => 5000;
        public static float MinButtonAppearInterval => 200;
        public static float MaxButtonAppearInterval => 500;
        public static float ButtonAppearIntervalIncrease => 25;
    }
}

