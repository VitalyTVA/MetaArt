using MetaArt.Core;
namespace ThatButtonAgain {
    static class Level_FindWord {
        public static void Load(GameController game) {
            bool winLevel = false;
            var button = game.CreateButton(() => {
                if(winLevel)
                    game.StartNextLevelAnimation();
                else {
                    game.scene.ClearElements();
                    game.scene.AddElement(new SvgElement(SvgKind.Cthulhu) {
                        Rect = Rect.FromCenter(
                            new Vector2(game.width / 2, game.height / 2),
                            new Vector2(game.width * Constants.CthulhuWidthScaleRatio)
                        )
                    });
                    game.StartCthulhuReloadLevelAnimation();
                }
            }).AddTo(game);
            button.Rect = Rect.Empty;
            button.HitTestVisible = false;

            var buttonRect = game.GetButtonRect();

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
                        Rect = game.GetLetterTargetRect(i - (letterCount - 5) / 2, buttonRect, squared: true, row: j - letterCount / 2),
                        HitTestVisible = true,
                        Scale = new Vector2(Constants.FindWordLetterScale),
                        ActiveRatio = 0,
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
                                game.scene,
                                letter,
                                element => {
                                    if(element is Letter l && !hovered.Any(x => x.Item1 == l)) {
                                        var (i, j) = GetLetterPosition(l);
                                        if(hovered.Count == 0
                                        || hovered.Count == 1
                                        || (j == hovered.Last().Item3 + 1 || j == hovered.Last().Item3 - 1) && i == hovered[0].Item2 && i == hovered.Last().Item2
                                        || (i == hovered.Last().Item2 + 1 || i == hovered.Last().Item2 - 1) && j == hovered[0].Item3 && j == hovered.Last().Item3
                                        ) {
                                            hovered.Add((l, i, j));
                                            l.ActiveRatio = 1;
                                            game.playSound(SoundKind.Hover);
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
                                    bool win = result.SequenceEqual("TOUCH".Select(x => x));
                                    bool fail = result.SequenceEqual("CTHULHU".Select(x => x));
                                    if(win || fail) {
                                        winLevel = win;
                                        for(int i = 0; i < letterCount; i++) {
                                            for(int j = 0; j < letterCount; j++) {
                                                if(!hovered.Any(x => x.Item1 == letters[i, j])) {
                                                    game.scene.RemoveElement(letters[i, j]);
                                                } else {
                                                    letters[i, j].HitTestVisible = false;
                                                }
                                            }
                                        }
                                        button.HitTestVisible = true;
                                    } else {
                                        hovered.ForEach(x => x.Item1.ActiveRatio = 0);
                                        button.Rect = Rect.Empty;
                                    }
                                },
                                releaseState: releaseState
                            );
                        };
                    letters[i, j] = letter;
                    letter.AddTo(game);
                }
            }
        }
    }
}

