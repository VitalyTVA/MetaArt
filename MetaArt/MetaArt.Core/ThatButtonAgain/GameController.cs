using MetaArt.Core;
using System.Numerics;

namespace ThatButtonAgain {
    public enum SoundKind {
        Win1,
        Cthulhu,
        DisabledClick,
        ErrorClick,
        Snap,
        SuccessSwitch,
        Swap,
        Hover,
    }
    public enum SvgKind {
        Cthulhu,
    }
    public class GameController {
        public readonly Scene scene;
        readonly AnimationsController animations = new();

        public readonly float letterVerticalOffset;
        readonly float buttonWidth;
        readonly float buttonHeight;
        public readonly float letterSize;
        readonly float letterDragBoxHeight;
        readonly float letterDragBoxWidth;
        readonly float letterHorzStep;
        readonly Action<SoundKind> playSound;

        public GameController(float width, float height, Action<SoundKind> playSound) {
            scene = new Scene(width, height, () => animations.AllowInput);

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
                Level_FindWord,
                Level_10,
                Level_11,
                Level_ScrollLetters,
                Level_ReorderLetters,
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
                MakeDraggableLetter(letter, index, button);
            });
            new WaitConditionAnimation(condition: GetAreLettersInPlaceCheck(button.Rect, letters)) { 
                End = () => button.IsEnabled = true 
            }.Start(animations);
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
                        new RotateAnimation {
                            Duration = Constants.RotateAroundLetterDuration,
                            From = fromAngle,
                            To = toAngle,
                            Center = letter.Rect.Mid,
                            Radius = (sideLetter.Rect.Mid - letter.Rect.Mid).Length(),
                            SetLocation = center => {
                                sideLetter.Rect = Rect.FromCenter(center, sideLetter.Rect.Size);
                            },
                            End = () => rotating = false
                        }.Start(animations, blockInput: true);
                    };

                    AddRotateAnimation(0, MathFEx.PI, rightLetter);
                    AddRotateAnimation(MathFEx.PI, MathFEx.PI * 2, leftLetter);
                };
                letter.GetPressState = Element.GetPressReleaseStateFactory(letter, onPress, () => { });
                letter.HitTestVisible = true;
            });

            new WaitConditionAnimation(
                condition: GetAreLettersInPlaceCheck(button.Rect, letters)) {
                End = () => {
                    button.HitTestVisible = true;
                    foreach(var item in letters) {
                        item.HitTestVisible = false;
                    }
                }
            }.Start(animations);
        }

        void Level_LettersBehindButton() {
            var buttonRect = GetButtonRect();

            var letters = CreateLetters((letter, index) => {
                letter.Rect = GetLetterTargetRect(index, buttonRect);
            });
            (float, Vector2)? snapInfo = default;
            float snapDistance = GetSnapDistance();
            var dragableButton = new Button {
                Rect = buttonRect,
                IsEnabled = false,
                HitTestVisible = true,
            };
            dragableButton.GetPressState = Element.GetAnchorAndSnapDragStateFactory(
                dragableButton,
                () => snapDistance,
                () => snapInfo,
                OnElementSnap,
                coerceRectLocation: rect => rect.GetRestrictedLocation(scene.Bounds.Inflate(dragableButton.Rect.Size * Constants.ButtonOutOfBoundDragRatio))
            );

            scene.AddElement(dragableButton);

            new WaitConditionAnimation(
                condition: deltaTime => letters.All(l => !l.Rect.Intersects(dragableButton.Rect))) {
                    End = () => {
                        scene.SendToBack(dragableButton);
                        snapInfo = (snapDistance, buttonRect.Location);
                        EnableButtonWhenInPlace(buttonRect, dragableButton);

                    }
            }.Start(animations);
        }

        float GetSnapDistance() {
            return buttonHeight * Constants.ButtonAnchorDistanceRatio;
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
                AnimationBase animation = null!;
                var onPress = () => {
                    if(replaceIndex == indices.Length || indices[replaceIndex] != index)
                        return;
                    animation = new LerpAnimation<float>() {
                        From = 1,
                        To = 2,
                        Duration = Constants.InflateButtonDuration,
                        Lerp = MathFEx.Lerp,
                        End = () => {
                            letter.Value = "TOUCH"[index];
                            letter.Scale = Letter.NoScale;
                            letter.HitTestVisible = false;
                            replaceIndex++;
                            playSound(SoundKind.SuccessSwitch);
                        },
                        SetValue = value => letter.Scale = new Vector2(value, value)
                    }.Start(animations);
                };
                var onRelease = () => {
                    animations.RemoveAnimation(animation);
                    letter.Scale = Letter.NoScale;
                };
                letter.GetPressState = Element.GetPressReleaseStateFactory(letter, onPress, onRelease);

            }, "CLICK");
            new WaitConditionAnimation(
                condition: deltaTime => replaceIndex == 4) {
                    End = () => {
                        button.HitTestVisible = true;
                        foreach(var item in letters) {
                            item.HitTestVisible = false;
                        }
                    }
            }.Start(animations);
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
                WaitConditionAnimation.WaitTime(
                    TimeSpan.FromMilliseconds(GetWaitTime()),
                    () => {
                        SetVisibility(true);
                        WaitConditionAnimation.WaitTime(
                            TimeSpan.FromMilliseconds(appearInterval),
                            () => {
                                appearInterval = Math.Min(appearInterval + Constants.ButtonAppearIntervalIncrease, Constants.MaxButtonAppearInterval);
                                SetVisibility(false);
                                StartWaitButton();
                            }
                        ).Start(animations);
                    }
                ).Start(animations);
            };

            StartWaitButton();

        }

        void Level_ReflectedButton() {
            Action<bool, bool> syncButtons = null!;


            (Button button, Letter[] letters, Action syncLettersOnMoveAction) CreateButtonAndLetters(
                Vector2 offset, 
                string word, 
                bool flipV, 
                bool flipH
            ) {
                Rect buttonRect = GetButtonRect();
                var button = new Button {
                    Rect = buttonRect.Offset(offset),
                    IsEnabled = false,
                    HitTestVisible = true,
                };
                scene.AddElement(button);
                var letters = CreateLetters((letter, index) => {
                    letter.Rect = GetLetterTargetRect(index, button.Rect);
                    letter.Scale = new Vector2(flipH ? -1 : 1, flipV ? -1 : 1);
                }, word);
                Action syncLettersOnMoveAction = Element.CreateSetOffsetAction(button, letters);
                button.GetPressState = Element.GetAnchorAndSnapDragStateFactory(
                    button,
                    () => (flipH && flipV) ? GetSnapDistance() : 0,
                    () => (flipH || flipV) ? null : (GetSnapDistance(), buttonRect.Location),
                    onElementSnap: OnElementSnap,
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
                Button movingButton,
                (Button button, Letter[] letters, Action syncLettersOnMoveAction) horizontal,
                (Button button, Letter[] letters, Action syncLettersOnMoveAction) vertical
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

            EnableButtonWhenInPlace(flippedHV.button.Rect, normal.button);
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

        void Level_FindWord() {
            bool winLevel = false;
            var button = CreateButton(() => {
                if(winLevel)
                    StartNextLevelAnimation();
                else {
                    scene.ClearElements();
                    scene.AddElement(new SvgElement(SvgKind.Cthulhu) {
                        Rect = Rect.FromCenter(
                            new Vector2(scene.width / 2, scene.height / 2), 
                            new Vector2(scene.width * Constants.CthulhuWidthScaleRatio)
                        )
                    });
                    StartCthulhuReloadLevelAnimation();
                }
            });
            button.Rect = Rect.Empty;
            button.HitTestVisible = false;
            scene.AddElement(button);

            var buttonRect = GetButtonRect();

            const int letterCount = 9;
            var chars = new[] {
                "LOKUHTOKU",
                "HCLICKOUT",
                "CTHLUOLUT",
                "KUKHIUTKL",
                "OUOLHKOTH",
                "LTUHKHULT",
                "TICHIOCLH",
                "KCTHULHUI",
                "TOUKCILCH"
            };

            var letters = new Letter[letterCount, letterCount];
            for(int i = 0; i < letterCount; i++) {
                for(int j = 0; j < letterCount; j++) {
                    var value = chars[j][i];
                    var letter = new Letter {
                        Value = value,
                        Rect = GetLetterTargetRect(i - (letterCount - 5) / 2, buttonRect, squared: true)
                            .Offset(new Vector2(0, (j - letterCount / 2) * letterDragBoxWidth)),
                        HitTestVisible = true,
                        Scale = new Vector2(Constants.FindWordLetterScale),
                    };
                    (int, int) GetLetterPosition(Letter letter) {
                        for(int i = 0; i < letterCount; i++) {
                            for(int j = 0; j < letterCount; j++) {
                                if(letters![i, j] == letter) {
                                    return (i, j);
                                }
                            }
                        }
                        throw new InvalidOperationException();
                    }
                    letter.GetPressState = (startPoint, releaseState)
                        => {
                            var hovered = new List<(Letter, int, int)>();
                            return new HoverInputState(
                                scene,
                                letter,
                                element => {
                                    if(element is Letter l && !hovered.Any(x => x.Item1 == l)) {
                                        var (i, j) = GetLetterPosition(l);
                                        if(hovered.Count == 0
                                        || hovered.Count == 1
                                        || (i == hovered[0].Item2 && i == hovered[0].Item2)
                                        || (j == hovered[0].Item3 && j == hovered[0].Item3)
                                        ) {
                                            hovered.Add((l, i, j));
                                            playSound(SoundKind.Hover);
                                        }
                                        button.Rect = hovered
                                            .Skip(1)
                                            .Aggregate(hovered[0].Item1.Rect, (rect, x) => rect.ContainingRect(x.Item1.Rect))
                                            .Inflate(new Vector2(Constants.ContainingButtonInflateValue));
                                    }
                                },
                                onRelease: () => {
                                    var result = hovered
                                        .OrderBy(x => x.Item2)
                                        .ThenBy(x => x.Item3)
                                        .Select(x => x.Item1.Value);
                                    bool win = Enumerable.SequenceEqual(result, "TOUCH".Select(x => x));
                                    bool fail = Enumerable.SequenceEqual(result, "CTHULHU".Select(x => x));
                                    if(win || fail) {
                                        winLevel = win;
                                        for(int i = 0; i < letterCount; i++) {
                                            for(int j = 0; j < letterCount; j++) {
                                                if(!hovered.Any(x => x.Item1 == letters[i, j])) {
                                                    scene.RemoveElement(letters[i, j]);
                                                } else {
                                                    letters[i, j].HitTestVisible = false;
                                                }
                                            }
                                        }
                                        button.HitTestVisible = true;
                                    } else {
                                        button.Rect = Rect.Empty;
                                    }
                                },
                                releaseState: releaseState
                            );
                        };
                    letters[i, j] = letter;
                    scene.AddElement(letter);
                }
            }
        }

        void Level_10() {
            var button = CreateButton(StartNextLevelAnimation);
            button.IsEnabled = false;
            scene.AddElement(button);

            var letters = CreateLetters((letter, index) => {
            letter.Rect = GetLetterTargetRect(index, button.Rect);
                if(index == 1) {
                    SetUpLevelIndexButton(letter, new Vector2(levelNumberElementRect.Right, levelNumberElementRect.Top));
                    var targetLocation = GetLetterTargetRect(index, button.Rect).Location;
                    var initialLocation = letter.Rect.Location;
                    float initialScale = letter.Scale.X;
                    var initialSize = letter.Rect.Size;
                    MakeDraggableLetter(
                        letter, 
                        index, 
                        button, 
                        onMove: () => {
                            var amount = 1 - (targetLocation - letter.Rect.Location).Length() / (targetLocation - initialLocation).Length();
                            letter.ActiveRatio = amount;
                            letter.Scale = new Vector2(MathFEx.Lerp(initialScale, 1, amount));
                            letter.Rect = letter.Rect.SetSize(Vector2.Lerp(initialSize, new Vector2(letterDragBoxWidth, letterDragBoxHeight), amount));
                        }
                    );
                }
            });
            new WaitConditionAnimation(condition: GetAreLettersInPlaceCheck(button.Rect, letters)) { 
                End = () => button.IsEnabled = true 
            }.Start(animations);
        }

        void Level_11() {
            bool win = false;
            var button = CreateButton(() => {
                if(win)
                    StartNextLevelAnimation();
                else
                    StartNextLevelFalseAnimation();
            });
            scene.AddElement(button);

            var letters = CreateLetters((letter, index) => {
                letter.Rect = GetLetterTargetRect(index, button.Rect);
            }, "TOUCH0");

            void SetZeroDigit() {
                letters.Last().Rect = letters![1].Rect;
                letters.Last().HitTestVisible = true;
                letters[1].Opacity = 0;
            }
            SetZeroDigit();
            var maxDistance = buttonWidth * Constants.ZeroDigitMaxDragDistance;
            var minDistance = buttonHeight;
            letters.Last().GetPressState = Element.GetAnchorAndSnapDragStateFactory(
                letters.Last(),
                () => GetSnapDistance(),
                () => null,
                onElementSnap: element => {
                    element.HitTestVisible = false;
                    OnElementSnap(element);
                },
                onMove: () => {
                    var amount = win 
                        ? 1
                        : Math.Min(maxDistance, (letters[1].Rect.Location - letters.Last().Rect.Location).Length() - minDistance) / maxDistance;
                    amount = Math.Max(0, amount);
                    letters[1].Opacity = amount;
                    if(MathFEx.FloatsEqual(amount, 1)) {
                        if(!win)
                            playSound(SoundKind.SuccessSwitch);
                        win = true;
                        scene.RemoveElement(letters.Last());
                    }
                },
                coerceRectLocation: rect => rect.GetRestrictedLocation(scene.Bounds),
                onRelease: anchored => {
                    if(win)
                        return false;
                    if(!anchored)
                        playSound(SoundKind.ErrorClick);
                    SetZeroDigit();
                    return true;
                },
                onClick: StartNextLevelFalseAnimation
            );
        }

        void Level_ScrollLetters() {
            var button = CreateButton(StartNextLevelAnimation);
            button.HitTestVisible = false;
            scene.AddElement(button);
            var lineHeight = letterDragBoxHeight;
            var linesCount = (int)Math.Ceiling(scene.height / lineHeight + 1) + 1;
            var positions = "CLICK".Select(x => (float)((byte)x - (byte)'A')).ToArray();
            var expectedPositions = "TOUCH".Select(x => (float)((byte)x - (byte)'A')).ToArray();
            var offsets = new float[5];
            var letters = new Letter[linesCount, 5];

            //trivial, 2 steps, C leter stays inplace
            //2 +12
            //1 +3
            var changes = new[] {
                new [] { 1, 0, 0, -1, 0 }, //confusing
                new [] { 1, 1, 0, 0, -1 },
                new [] { -1, 0, 1, 0, 0 },
                new [] { 1, 0, -1, 1, 0 }, //confusing
                new [] { 0, 1, -1, 0, 1 },
            };

            /*
            //hardest, irregular increase, 3 steps, C letter stays inplace
            //2 +12
            //1 +3
            //0  -3 or 4 +3
            var changes = new[] {
                new [] { 1, 0, 0, 0, -1 },
                new [] { 2, 1, 0, 0, -2 },
                new [] { -1, 0, 1, 0, 0 },
                new [] { 1, 0, -1, 1, 0 }, //confusing
                new [] { -1, 0, 0, 0, 1 },
            };
            

            //hard, regular increase, 3 steps, C letter stays inplace
            //2 +12
            //4 -6
            //1 -3
            var changes = new[] {
                new [] { 1, 0, -1, 0, 1 }, //confusing
                new [] { -1, 1, 0, 0, -1 },
                new [] { -1, 0, 1, 0, 0 },
                new [] { 1, 0, -1, 1, 0 }, //confusing
                new [] { 0, -1, 0, 0, 1 },
            };
            
            //very hard, regular increase, 3 steps, C letter moves
            //2 +12
            //3 -3
            //4 -3
            var changes = new[] {
                new [] { 1, 0, -1, 0, 1 }, //confusing
                new [] { -1, 1, 0, 1, 0 }, //confusing
                new [] { -1, 0, 1, 0, 0 },
                new [] { -1, 0, 0, 1, 0 },
                new [] { 0, -1, 0, -1, 1 },
            };

            //extreme, irregular increase, 4 steps, C letter moves
            //3 +3
            //2 +6
            //1 +3
            //4 -3

            var changes = new[] {
                new [] { 1, 0, -1, 0, 1 }, //confusing
                new [] { 0, 1, 0, -1, 0 },
                new [] { -2, 0, 1, 0, 0 },
                new [] { 0, 0, 2, 1, 0 },
                new [] { -1, 0, 0, 0, 1 },
            };
            */
            for(int line = 0; line < linesCount; line++) {
                for(int column = 0; column < 5; column++) {
                    var letter = new Letter {
                        //Rect = GetLetterTargetRect(i , button.Rect)
                        //    .Offset(new Vector2(0, (j - letterCount / 2) * lineHeight)),
                        HitTestVisible = true,
                        Scale = new Vector2(Constants.ScrollLettersLetterScale),
                    };

                    var actualColumn = column;

                    void SetOffsetAndArrange(float offset) {
                        for(int col = 0; col < 5; col++) {
                            offsets![col] = changes![actualColumn][col] * offset;
                        }
                        ArrangeLetters();
                    }

                    float GetOffset(Vector2 delta) => -delta.Y / lineHeight;
                    
                    letter.GetPressState = (starPoint, releaseState) => {
                        return new DragInputState(
                            starPoint, 
                            onDrag: delta => {
                                SetOffsetAndArrange(GetOffset(delta));
                                return true;
                            },
                            onRelease: delta => {
                                var from = GetOffset(delta);
                                var to = (float)Math.Round(from);
                                var animation = new LerpAnimation<float> {
                                    Duration = TimeSpan.FromMilliseconds(300 * (float)Math.Abs(from - to)),
                                    From = from,
                                    To = to,
                                    SetValue = val => SetOffsetAndArrange(val),
                                    Lerp = MathFEx.Lerp,
                                    End = () => {
                                        for(int i = 0; i < 5; i++) {
                                            positions[i] = GetNormalizedPosition(positions[i] + offsets[i]);
                                            offsets[i] = 0;
                                        }
                                        if(positions.Zip(expectedPositions, (x, y) => MathFEx.FloatsEqual(x, y)).All(x => x)) {
                                            playSound(SoundKind.SuccessSwitch);
                                            button.HitTestVisible = true;
                                            for(int line = 0; line < linesCount; line++) {
                                                for(int column = 0; column < 5; column++) {
                                                    var letter = letters[line, column];
                                                    letter.HitTestVisible = false;
                                                    if(!button.Rect.Contains(letter.Rect))
                                                        scene.RemoveElement(letter);
                                                }
                                            }
                                        } else {
                                            playSound(SoundKind.ErrorClick);
                                        }
                                    }
                                }.Start(animations, blockInput: true);
                            },
                            releaseState);
                    
                    };

                    letters[line, column] = letter;
                    scene.AddElement(letter);
                }
            }
            float GetNormalizedPosition(float position)  {
                if(position < 0) position += 26;
                if(position >= 26) position -= 26;
                return position;
            }
            void ArrangeLetters() {
                for(int line = 0; line < linesCount; line++) {
                    for(int column = 0; column < 5; column++) {
                        var letter = letters[line, column];

                        var position = positions[column] + offsets[column];
                        var letterIndex = GetNormalizedPosition(line + (float)Math.Truncate(position) - linesCount / 2);

                        letter.Value = (char)((byte)'A' + (byte)letterIndex);
                        letter.ActiveRatio = "TOUCH"[column] == letter.Value ? 1 : 0;

                        var fractionalOffset = (float)(position - Math.Truncate(position));
                        letter.Rect = GetLetterTargetRect(column, button.Rect)
                            .Offset(new Vector2(0, (line - linesCount / 2 - fractionalOffset) * lineHeight));
                    }
                }
            }
            ArrangeLetters();
            //var letters = CreateLetters((letter, index) => {
            //    letter.Rect = GetLetterTargetRect(index, button.Rect);
            //});
        }

        void Level_ReorderLetters() {
            var button = CreateButton(StartNextLevelAnimation);
            button.HitTestVisible = true;
            button.IsVisible = false;
            scene.AddElement(button);

            var area = new Area();
            var letters = CreateLetters((letter, index) => {
                letter.Rect = GetLetterTargetRect(4 - index, button.Rect);
                letter.HitTestVisible = true;
                letter.GetPressState = (startPoint, releaseState) => {
                    return new DragInputState(
                        startPoint,
                        onDrag: delta => {

                            int directionX = 0, directionY = 0;
                            Direction direction;
                            if(Math.Abs(delta.X) > Math.Abs(delta.Y)) {
                                delta.Y = 0;
                                directionX = Math.Sign(delta.X);
                                direction = directionX > 0 ? Direction.Right : Direction.Left;
                            } else {
                                delta.X = 0;
                                directionY = Math.Sign(delta.Y);
                                direction = directionY > 0 ? Direction.Down : Direction.Up;
                            }

                            if(delta.Length() < GetSnapDistance())
                                return true;

                            if(!area.Move(letter.Value, direction))
                                return true;

                            var from = letter.Rect.Location;
                            var to = letter.Rect.Location + new Vector2(letterHorzStep * directionX, letterDragBoxHeight * directionY);
                            playSound(SoundKind.Hover);
                            var animation = new LerpAnimation<Vector2> {
                                Duration = TimeSpan.FromMilliseconds(150),
                                From = from,
                                To = to,
                                SetValue = val => letter.Rect = letter.Rect.SetLocation(val),
                                Lerp = Vector2.Lerp,
                            }.Start(animations, blockInput: true);
                            return false;
                        },
                        onRelease: delta => {
                        },
                        releaseState);

                };
            });

            var step = button.Rect.Width / 5;
            var pathElement = new PathElement(new[] {
                button.Rect.Location,

                new Vector2(letters[3].Rect.Left, button.Rect.Top),
                new Vector2(letters[3].Rect.Left, letters[3].Rect.Top - letterDragBoxHeight),
                new Vector2(letters[3].Rect.Right, letters[3].Rect.Top - letterDragBoxHeight),
                new Vector2(letters[3].Rect.Right, button.Rect.Top),

                new Vector2(letters[1].Rect.Left, button.Rect.Top),
                new Vector2(letters[1].Rect.Left, letters[1].Rect.Top - letterDragBoxHeight),
                new Vector2(letters[1].Rect.Right, letters[1].Rect.Top - letterDragBoxHeight),
                new Vector2(letters[1].Rect.Right, button.Rect.Top),

                button.Rect.TopRight,
                button.Rect.BottomRight,

                new Vector2(letters[1].Rect.Right, button.Rect.Bottom),
                new Vector2(letters[1].Rect.Right, letters[1].Rect.Bottom + letterDragBoxHeight),
                new Vector2(letters[1].Rect.Left, letters[1].Rect.Bottom + letterDragBoxHeight),
                new Vector2(letters[1].Rect.Left, button.Rect.Bottom),

                new Vector2(letters[3].Rect.Right, button.Rect.Bottom),
                new Vector2(letters[3].Rect.Right, letters[1].Rect.Bottom + letterDragBoxHeight),
                new Vector2(letters[3].Rect.Left, letters[1].Rect.Bottom + letterDragBoxHeight),
                new Vector2(letters[3].Rect.Left, button.Rect.Bottom),

                button.Rect.BottomLeft,
            });
            scene.AddElementBehind(pathElement);

            new WaitConditionAnimation(
                condition: GetAreLettersInPlaceCheck(button.Rect, letters)) {
                End = () => {
                    pathElement.IsVisible = false;
                    button.IsVisible = true;
                    foreach(var item in letters) {
                        item.HitTestVisible = false;
                    }
                    playSound(SoundKind.SuccessSwitch);
                }
            }.Start(animations);
        }

        void MakeDraggableLetter(Letter letter, int index, Button button, Action? onMove = null) {
            letter.HitTestVisible = true;
            letter.GetPressState = Element.GetAnchorAndSnapDragStateFactory(
                letter,
                () => 0,
                () => (letterSize * Constants.LetterSnapDistanceRatio, GetLetterTargetRect(index, button.Rect).Location),
                onElementSnap: element => {
                    element.HitTestVisible = false;
                    OnElementSnap(element);
                },
                onMove: onMove,
                coerceRectLocation: rect => rect.GetRestrictedLocation(scene.Bounds)
            );
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
            playSound(SoundKind.Snap);
        }

        void EnableButtonWhenInPlace(Rect buttonRect, Button dragableButton) {
            new WaitConditionAnimation(
                condition: deltaTime => MathFEx.VectorsEqual(dragableButton.Rect.Location, buttonRect.Location)) {
                    End = () => {
                        dragableButton.IsEnabled = true;
                        dragableButton.GetPressState = GetClickHandler(StartNextLevelAnimation, dragableButton);
                    }
            }.Start(animations);
        }

        Func<TimeSpan, bool> GetAreLettersInPlaceCheck(Rect buttonRect, Letter[] letters) {
            var targetLocations = GetLettersTargetLocations(buttonRect);
            return deltaTime => letters.Select((l, i) => (l, i)).All(x => MathFEx.VectorsEqual(x.l.Rect.Location, targetLocations[x.i]));
        }

        Vector2[] GetLettersTargetLocations(Rect buttonRect) => 
            Enumerable.Range(0, 5)
                .Select(index => GetLetterTargetRect(index, buttonRect).Location)
                .ToArray();

        Rect GetLetterTargetRect(int index, Rect buttonRect, bool squared = false) =>
            Rect.FromCenter(
                buttonRect.Mid + new Vector2((index - 2) * letterHorzStep, 0),
                new Vector2(letterDragBoxWidth, squared ? letterDragBoxWidth : letterDragBoxHeight)
            );

        Button CreateButton(Action click) {
            var button = new Button {
                Rect = GetButtonRect(),
                HitTestVisible = true,
            };
            button.GetPressState = GetClickHandler(click, button);
            return button;
        }

        Func<Vector2, NoInputState, InputState> GetClickHandler(Action click, Button button) {
            return (startPoint, releaseState) => {
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
        }

        Rect GetButtonRect() {
            return new Rect(
                                        scene.width / 2 - buttonWidth / 2,
                                        scene.height / 2 - buttonHeight / 2,
                                        buttonWidth,
                                        buttonHeight
                                    );
        }

        void StartFade(float from, float to, Action end, TimeSpan duration) {
            var element = new FadeOutElement() { Rect = new Rect(0, 0, scene.width, scene.height), Opacity = from };
            var animation = new LerpAnimation<float> {
                Duration = duration,
                From = from,
                To = to,
                SetValue = value => element.Opacity = value,
                Lerp = MathFEx.Lerp,
                End = () => {
                    scene.RemoveElement(element);
                    end();
                }
            }.Start(animations);
            scene.AddElement(element);
        }
        void StartNextLevelAnimation() {
            StartFade(0, 255, () => SetLevel(levelIndex + 1), Constants.FadeOutDuration);
            playSound(SoundKind.Win1);
        }
        void StartNextLevelFalseAnimation() {
            StartFade(0, 255, () => SetLevel(levelIndex), Constants.FadeOutDuration);
            playSound(SoundKind.Win1);
        }
        void StartCthulhuReloadLevelAnimation() {
            StartFade(0, 255, () => SetLevel(levelIndex), Constants.FadeOutCthulhuDuration);
            playSound(SoundKind.Cthulhu);
        }

        Letter[] CreateLetters(Action<Letter, int> setUp, string word = "TOUCH") {
            int index = 0;
            var letters = new Letter[word.Length];
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
        Rect levelNumberElementRect;
        public void SetLevel(int level) {
            levelIndex = Math.Min(level, levels.Length - 1);
            
            scene.ClearElements();
            int digitIndex = 0;
            foreach(var digit in (levelIndex != 10 ? levelIndex : 1).ToString()) {
                var levelNumberElement = new Letter {
                    Value = digit,
                };
                SetUpLevelIndexButton(
                    levelNumberElement,
                    new Vector2(
                        letterSize * Constants.LetterIndexOffsetRatioX + digitIndex * letterDragBoxWidth * Constants.LevelLetterRatio, 
                        letterSize * Constants.LetterIndexOffsetRatioY
                    )
                );
                levelNumberElementRect = levelNumberElement.Rect;
                scene.AddElement(levelNumberElement);
                digitIndex++;
            }
            levels[levelIndex]();
            StartFade(255, 0, () => { }, Constants.FadeOutDuration);
        }
        void SetUpLevelIndexButton(Letter letter, Vector2 location) {
            letter.Rect = new Rect(
                location,
                new Vector2(letterDragBoxWidth * Constants.LevelLetterRatio, letterDragBoxHeight * Constants.LevelLetterRatio)
            );
            letter.ActiveRatio = 0;
            letter.Scale = new Vector2(Constants.LevelLetterRatio);

        }
    }
    public enum Direction { Left, Right, Up, Down }
    public class Area {
        const char X = 'X';
        const char E = ' ';


        char[][] area;
        public Area() {
            area = new char[][] {
                new[] { X, E, X, E, X  },
                new[] { 'H', 'C', 'U', 'O', 'T' },
                new[] { X, E, X, E, X  }
            };
        }
        int Width => area[0].Length;
        int Height => area.Length;

        public bool Move(char letter, Direction direction) {
            var (row, col) = LocateLetter(letter);
            bool TrySwapLetters(bool canApplyDelta, int deltaRow, int deltaCol) {
                if(!canApplyDelta) 
                    return false;
                if(area[row + deltaRow][col + deltaCol] == E) {
                    area[row + deltaRow][col + deltaCol] = letter;
                    area[row][col] = E;
                    return true;
                }
                return false;
            }
            switch(direction) {
                case Direction.Left:
                    return TrySwapLetters(col > 0, 0, -1);
                case Direction.Right:
                    return TrySwapLetters(col < Width - 1, 0, 1);
                case Direction.Up:
                    return TrySwapLetters(row > 0, -1, 0);
                case Direction.Down:
                    return TrySwapLetters(row < Height - 1, 1, 0);
                default:
                    throw new InvalidOperationException();
            }
        }
        (int row, int col) LocateLetter(char letter) {
            for(int row = 0; row < Height; row++) {
                for(int col = 0; col < Width; col++) {
                    if(area[row][col] == letter)
                        return (row, col);
                }
            }
            throw new InvalidOperationException();
        }
    }
    public static class Constants {
        public static float ButtonRelativeWidth => 0.6f;
        public static float ButtonHeightRatio => 1f / 3f;

        public static float LetterHeightRatio => 3f / 4f;
        public static float LetterVerticalOffsetRatio => 0.16f;
        public static float LetterDragBoxHeightRatio => 0.9f;
        public static float LetterDragBoxWidthRatio => 0.57f;
        public static float LetterHorizontalStepRatio => 0.18f;

        public static float LevelLetterRatio => 0.75f;

        //public static Color FadeOutColor = new Color(0, 0, 0);
        public static TimeSpan FadeOutDuration => TimeSpan.FromMilliseconds(500);
        public static TimeSpan FadeOutCthulhuDuration => TimeSpan.FromMilliseconds(3000);
        public static TimeSpan RotateAroundLetterDuration => TimeSpan.FromMilliseconds(500);
        public static TimeSpan InflateButtonDuration => TimeSpan.FromMilliseconds(1500);

        public static float LetterIndexOffsetRatioX => .2f;
        public static float LetterIndexOffsetRatioY => .2f;

        public static float LetterSnapDistanceRatio => .2f;
        public static float ButtonAnchorDistanceRatio => .2f;

        public static float MinButtonInvisibleInterval => 1000;
        public static float MaxButtonInvisibleInterval => 5000;
        public static float MinButtonAppearInterval => 200;
        public static float MaxButtonAppearInterval => 500;
        public static float ButtonAppearIntervalIncrease => 25;

        public static float ButtonOutOfBoundDragRatio => 0.7f;

        public static float FindWordLetterScale => .75f;
        public static float ContainingButtonInflateValue => 2;

        public static float CthulhuWidthScaleRatio = .7f;

        public static float ZeroDigitMaxDragDistance => 0.75f;

        public static float ScrollLettersLetterScale => .9f;

        public static float ButtonBorderWeight => 3f;
        public static float ButtonCornerRadius => 5f;


    }
}

