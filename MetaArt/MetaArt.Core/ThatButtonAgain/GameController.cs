﻿using MetaArt.Core;
using System.Numerics;

namespace ThatButtonAgain {
    public enum SoundKind {
        Win1,
        DisabledClick,
        ErrorClick,
        Snap,
        SuccessSwitch,
        Swap,
    }
    public class GameController {
        public readonly Scene scene;
        readonly AnimationsController animations = new(new IAnimation[0]);

        public readonly float letterVerticalOffset;
        readonly float buttonWidth;
        readonly float buttonHeight;
        public readonly float letterSize;
        readonly float letterDragBoxHeight;
        readonly float letterDragBoxWidth;
        readonly float letterHorzStep;
        readonly Action<SoundKind> playSound;

        public GameController(float width, float height, Action<SoundKind> playSound) {
            scene = new Scene(width, height);

            buttonWidth = scene.width * Constants.ButtonRelativeWidth;
            buttonHeight = buttonWidth * Constants.ButtonHeightRatio;
            letterSize = buttonHeight * Constants.LetterHeightRatio;
            letterDragBoxHeight = buttonHeight * Constants.LetterDragBoxHeightRatio;
            letterDragBoxWidth = buttonHeight * Constants.LetterDragBoxWidthRatio;
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
                Level_Mod2Vectors,
            };
            this.playSound = playSound;
        }

        public void NextFrame(float deltaTime) {
            animations.Next(TimeSpan.FromMilliseconds(deltaTime));
        }

        void Level_TrivialClick() {
            var button = CreateButton(StartNextLevelAnimation);
            scene.AddElement(button);

            CreateLetters((letter, index) => {
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

            var letters = CreateLetters((letter, index) => {
                letter.Rect = Rect.FromCenter(
                    new Vector2(button.Rect.MidX + letterDragBoxWidth * points[index].Item1, button.Rect.MidY + letterDragBoxHeight * points[index].Item2),
                    new Vector2(letterDragBoxWidth, letterDragBoxHeight)
                ).GetRestrictedRect(scene.Bounds);
                letter.HitTestVisible = true;
                letter.GetPressState = Element.GetAnchorAndSnapDragStateFactory(
                    letter, 
                    () => 0, 
                    () => (letterSize * Constants.LetterSnapDistanceRatio, GetLetterTargetRect(index, button.Rect).Location),
                    OnElementSnap,
                    coerceRectLocation: rect => rect.GetRestrictedLocation(scene.Bounds)
                );
            });
            animations.AddAnimation(new WaitConditionAnimation(
                condition: GetAreLettersInPlaceCheck(button.Rect, letters),
                end: () => button.IsEnabled = true
            ));
        }

        void Level_16xClick() {
            SetupCapitalLettersSwitchLevel(0b10000, (value, index) => value + 1);
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
            letters = CreateLetters((letter, index) => {
                letter.Rect = GetLetterTargetRect(indices[index], button.Rect);
                var onPress = () => {
                    if(rotating)
                        return;

                    var leftLetter = letters!.OrderByDescending(x => x.Rect.Left).FirstOrDefault(x => MathFEx.Less(x.Rect.Left, letter.Rect.Left));
                    var rightLetter = letters!.OrderBy(x => x.Rect.Left).FirstOrDefault(x => MathFEx.Greater(x.Rect.Left, letter.Rect.Left));
                    if(leftLetter == null || rightLetter == null)
                        return;

                    rotating = true;
                    playSound(SoundKind.Swap);
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

            var letters = CreateLetters((letter, index) => {
                letter.Rect = GetLetterTargetRect(index, buttonRect);
            });
            (float, Vector2)? snapInfo = default;
            float snapDistance = buttonHeight * Constants.ButtonAnchorDistanceRatio;
            var dragableButton = new DragableButton { 
                Rect = buttonRect,
            };
            dragableButton.GetPressState = Element.GetAnchorAndSnapDragStateFactory(
                dragableButton, 
                () => snapDistance, 
                () => snapInfo,
                OnElementSnap,
                coerceRectLocation: rect => rect.GetRestrictedLocation(scene.Bounds.Inflate(dragableButton.Rect.Size * Constants.ButtonOutOfBoundDragRatio))
            );

            scene.AddElement(dragableButton);

            animations.AddAnimation(new WaitConditionAnimation(
                condition: deltaTime => letters.All(l => !l.Rect.Intersects(dragableButton.Rect)),
                end: () => {
                    scene.SendToBack(dragableButton);
                    snapInfo = (snapDistance, buttonRect.Location);
                    ReplaceWithRealButtonWhenInPlace(buttonRect, dragableButton);
                }));
        }

        void Level_ClickInsteadOfTouch() {
            var button = CreateButton(StartNextLevelAnimation);
            button.HitTestVisible = false;
            scene.AddElement(button);

            var indices = new[] { 0, 4, 2, 1 };
            int replaceIndex = 0;

            var letters = CreateLetters((letter, index) => {
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
                            letter.Scale = Letter.NoScale;
                            letter.HitTestVisible = false;
                            replaceIndex++;
                            playSound(SoundKind.SuccessSwitch);
                        },
                        SetValue = value => letter.Scale = new Vector2(value, value)
                    };
                    animations.AddAnimation(animation);
                };
                var onRelease = () => {
                    animations.RemoveAnimation(animation);
                    letter.Scale = Letter.NoScale;
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
            var letters = CreateLetters((letter, index) => {
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

            bool firstAppear = true;
            float GetWaitTime() {
                if(firstAppear) {
                    firstAppear = false;
                    return Constants.MinButtonInvisibleInterval * 2;
                }
                return MathFEx.Random(Constants.MinButtonInvisibleInterval, Constants.MaxButtonInvisibleInterval);   
            }

            void StartWaitButton() {
                animations.AddAnimation(WaitConditionAnimation.WaitTime(
                    TimeSpan.FromMilliseconds(GetWaitTime()),
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
            Action<bool, bool> syncButtons = null!;


            (DragableButton button, Letter[] letters, Action syncLettersOnMoveAction) CreateButtonAndLetters(
                Vector2 offset, 
                string word, 
                bool flipV, 
                bool flipH
            ) {
                Rect buttonRect = GetButtonRect();
                var button = new DragableButton {
                    Rect = buttonRect.Offset(offset),
                };
                scene.AddElement(button);
                var letters = CreateLetters((letter, index) => {
                    letter.Rect = GetLetterTargetRect(index, button.Rect);
                    letter.Scale = new Vector2(flipH ? -1 : 1, flipV ? -1 : 1);
                }, word);
                Action syncLettersOnMoveAction = Element.CreateSetOffsetAction(button, letters);
                button.GetPressState = Element.GetAnchorAndSnapDragStateFactory(
                    button,
                    () => (flipH && flipV) ? buttonHeight * Constants.ButtonAnchorDistanceRatio : 0,
                    () => (flipH || flipV) ? null : (buttonHeight * Constants.ButtonAnchorDistanceRatio, buttonRect.Location),
                    OnElementSnap,
                    onMove: () => { 
                        syncLettersOnMoveAction();
                        syncButtons(flipV, flipH);
                    },
                    coerceRectLocation: rect => {
                        if(rect.Left < 0 && rect.Top < 0) {
                            if(rect.Left < rect.Top)
                                return new Vector2(rect.Left, 0);
                            else
                                return new Vector2(0, rect.Top);
                        }
                        return rect.Location;
                    }
                );
                return (button, letters, syncLettersOnMoveAction);
            }

            var invisiblePosition = new Vector2(-3000, -3000);
            var flippedHV = CreateButtonAndLetters(new Vector2(0, 0), "HCUOT", flipV: true, flipH: true);
            var flippedH = CreateButtonAndLetters(invisiblePosition, "HCUOT", flipV: false, flipH: true);
            var flippedV = CreateButtonAndLetters(invisiblePosition, "TOUCH", flipV: true, flipH: false);

            var normal = CreateButtonAndLetters(invisiblePosition, "TOUCH", flipV: false, flipH: false);

            void SyncLocations(
                DragableButton movingButton,
                (DragableButton button, Letter[] letters, Action syncLettersOnMoveAction) horizontal,
                (DragableButton button, Letter[] letters, Action syncLettersOnMoveAction) vertical
            ) {
                horizontal.button.Rect = new Rect(invisiblePosition, horizontal.button.Rect.Size);
                vertical.button.Rect = new Rect(invisiblePosition, vertical.button.Rect.Size);

                if(movingButton.Rect.Left + movingButton.Rect.Width > scene.width)
                    horizontal.button.Rect = movingButton.Rect.Offset(new Vector2(-scene.width, 0));
                else if(movingButton.Rect.Left < 0)
                    horizontal.button.Rect = movingButton.Rect.Offset(new Vector2(scene.width, 0));

                else if(movingButton.Rect.Top + movingButton.Rect.Height > scene.height)
                    vertical.button.Rect = movingButton.Rect.Offset(new Vector2(0, - scene.height));
                else if(movingButton.Rect.Top < 0)
                    vertical.button.Rect = movingButton.Rect.Offset(new Vector2(0, scene.height));

                horizontal.syncLettersOnMoveAction();
                vertical.syncLettersOnMoveAction();
            }

            syncButtons = (flipV, flipH) => {
                switch((flipV, flipH)) {
                    case (true, true):
                        SyncLocations(flippedHV.button, flippedH, flippedV);
                        break;
                    case (true, false):
                        SyncLocations(flippedV.button, normal, flippedHV);
                        break;
                    case (false, true):
                        SyncLocations(flippedH.button, flippedHV, normal);
                        break;
                    case (false, false):
                        SyncLocations(normal.button, flippedV, flippedH);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            };

            ReplaceWithRealButtonWhenInPlace(flippedHV.button.Rect, normal.button);
        }

        void Level_Mod2Vectors() {
            var vectors = new[] {
                0b11001,//--
                0b01010,
                0b10100,//--
                0b10010,//--
                0b00101
            };
            SetupCapitalLettersSwitchLevel(0b00000, (value, index) => value ^ vectors[index]);
        }

        void SetupCapitalLettersSwitchLevel(int initialValue, Func<int, int, int> changeValue) {
            Letter[] letters = null!;

            var button = CreateButton(StartNextLevelAnimation);
            button.HitTestVisible = false;
            scene.AddElement(button);

            int value = initialValue;
            const int target = 0b11111;
            void SetLetters() {
                for(int i = 0; i < 5; i++) {
                    bool isCapitalLetter = (value & (1 << (4 - i))) > 0;
                    letters[i].Value = (isCapitalLetter ? "TOUCH" : "touch")[i];
                }
            };

            letters = CreateLetters((letter, index) => {
                letter.HitTestVisible = true;
                letter.Rect = GetLetterTargetRect(index, button.Rect);
                letter.GetPressState = (startPoint, releaseState) => {
                    return new TapInputState(
                        button,
                        () => {
                            playSound(SoundKind.DisabledClick);
                            value = changeValue(value, index);
                            SetLetters();
                            if(value == target) {
                                foreach(var item in letters) {
                                    item.HitTestVisible = false;
                                }
                                button.HitTestVisible = true;
                            }
                        },
                        setState: isPressed => {
                            button.IsPressed = isPressed;
                        },
                        releaseState
                    );
                };

            });
            SetLetters();
        }

        void OnElementSnap(Element element) {
            element.HitTestVisible = false;
            playSound(SoundKind.Snap);
        }

        void ReplaceWithRealButtonWhenInPlace(Rect buttonRect, DragableButton dragableButton) {
            animations.AddAnimation(new WaitConditionAnimation(
                condition: deltaTime => MathFEx.VectorsEqual(dragableButton.Rect.Location, buttonRect.Location),
                end: () => {
                    scene.RemoveElement(dragableButton);
                    scene.AddElementBehind(CreateButton(StartNextLevelAnimation));
                }));
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
                new Vector2(letterDragBoxWidth, letterDragBoxHeight)
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
                    setState: isPressed => {
                        if(isPressed == button.IsPressed)
                            return;
                        button.IsPressed = isPressed;
                        if(isPressed && !button.IsEnabled)
                            playSound(SoundKind.ErrorClick);
                    },
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
            playSound(SoundKind.Win1);
        }

        Letter[] CreateLetters(Action<Letter, int> setUp, string word = "TOUCH") {
            int index = 0;
            var letters = new Letter[5];
            foreach(var value in word) {
                var letter = new Letter() {
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
        public static float LetterVerticalOffsetRatio => 0.16f;
        public static float LetterDragBoxHeightRatio => 0.9f;
        public static float LetterDragBoxWidthRatio => 0.57f;
        public static float LetterHorizontalStepRatio => 0.18f;

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

        public static float ButtonOutOfBoundDragRatio => 0.7f;
    }
}

