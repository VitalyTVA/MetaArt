using MetaArt.Core;
namespace ThatButtonAgain {
    static class Level_16Game {
        public static void Load_3x3(GameController game) {
            LoadCore(game, 3);
        }
        public static void Load_4x4(GameController game) {
            LoadCore(game, 4);
        }
        static void LoadCore(GameController game, int size) {
            var button = game.CreateButton(() => game.StartNextLevelAnimation()).AddTo(game);
            button.IsEnabled = false;
            button.Rect = button.Rect.Offset(new Vector2(0, -1.5f * game.letterDragBoxHeight));

            var buttonLetters = game.CreateLetters((letter, index) => {
                letter.Rect = game.GetLetterTargetRect(index, button.Rect);
                letter.IsVisible = false;
            });
            var game16 = new Game16<Letter>(size);

            Rect GetLetterRect(int row, int col) {
                float size = 1f * game.letterSize;
                return Rect.FromCenter(
                    new Vector2(
                        game.width / 2 + size * (col + .5f - game16.size / 2f),
                        game.height / 2 + button.Rect.Height / 2 + size * row
                    ),
                    new Vector2(size)
                );
            }

            void SpawnNewLetter() {
                var letter = new Letter {
                    Opacity = 0,
                };
                var spawn = game16.SpawnNew(letter);
                if(spawn == null) {
                    GameOver();
                } else {
                    letter.Rect = GetLetterRect(spawn.Value.row, spawn.Value.col);
                    letter.Value = ToLetter(spawn.Value.value);
                    letter.Style = ElementExtensions.ToStyle(letter.Value);

                    letter.AddTo(game);
                    new LerpAnimation<float> {
                        From = 0,
                        To = 1,
                        Duration = TimeSpan.FromMilliseconds(200),
                        Lerp = MathFEx.Lerp,
                        SetValue = value => letter.Opacity = value
                    }.Start(game);
                }
                foreach(var item in buttonLetters) {
                    item.IsVisible = false;
                }
                foreach(var value in game16.GetDistinctValues()) {
                    var index = value switch {
                        Value.One => 0,
                        Value.Two => 1,
                        Value.Four => 2,
                        Value.Eight => 3,
                        Value.Sixteen => 4,
                        _ => throw new InvalidOperationException(),
                    };
                    buttonLetters[index].IsVisible = true;
                }
            }
            SpawnNewLetter();
            SpawnNewLetter();

            var inputHandler = new InputHandlerElement {
                Rect = new Rect(0, button.Rect.Bottom, game.width, game.height),
                GetPressState = (startPoint, releaseState) => {
                    if(button.IsEnabled) {
                        game.playSound(SoundKind.ErrorClick);
                        return releaseState;
                    }
                    return new DragInputState(
                        startPoint,
                        onDrag: delta => {
                            var direction = DirectionExtensions.GetSwipeDirection(ref delta, game.GetSnapDistance());

                            if(direction == null)
                                return true;
                            game.playSound(direction.Value.GetSound());
                            var moves = game16.Swipe(direction.Value);
                            var duration = TimeSpan.FromMilliseconds(100);
                            var celebrated = false;
                            foreach(var move in moves) {
                                if(move.value == null) {
                                    game.scene.RemoveElement(move.element);
                                } else {
                                    if(move.value.Value > Value.Sixteen) {
                                        GameOver();
                                        break;
                                    }
                                    if(move.merged && !celebrated) {
                                        game.playSound(SoundKind.Merge);
                                        celebrated = true;
                                    }
                                    var newValue = move.value.Value;
                                    var animation = new LerpAnimation<Vector2> {
                                        Duration = duration,
                                        From = move.element.Rect.Location,
                                        To = GetLetterRect(move.row, move.col).Location,
                                        SetValue = val => move.element.Rect = move.element.Rect.SetLocation(val),
                                        Lerp = Vector2.Lerp,
                                        End = () => {
                                            move.element.Value = ToLetter(newValue);
                                            move.element.Style = ElementExtensions.ToStyle(move.element.Value);

                                        },
                                    }.Start(game, blockInput: true);
                                }
                            }
                            WaitConditionAnimation.WaitTime(duration, () => {
                                SpawnNewLetter();
                                if(buttonLetters.All(x => x.IsVisible)) {
                                    game.playSound(SoundKind.SuccessSwitch);
                                    button.IsEnabled = true;
                                } else {
                                }
                            }).Start(game);
                            return false;
                        },
                        onRelease: delta => { },
                        releaseState
                    );
                }

            }.AddTo(game);

            void GameOver() {
                game.playSound(SoundKind.BrakeBall);
                game.StartReloadLevelAnimation();
            }
        }

        enum Value {
            One = 1,
            Two = 2,
            Four = 4,
            Eight = 8,
            Sixteen = 16,
        }
        static char ToLetter(Value value) =>
            value switch {
                Value.One => 'T',
                Value.Two => 'O',
                Value.Four => 'U',
                Value.Eight => 'C',
                Value.Sixteen => 'H',
                _ => throw new InvalidOperationException(),
            };

        sealed class Game16<T> {
            internal record struct Cell(Value value, T element);
            internal record struct Move(int row, int col, T element, Value? value, bool merged);

            public readonly int size;
            public int lastIndex => size - 1;
            Cell?[,] values;
            Random random = new Random(0);

            public Game16(int size) {
                this.size = size;
                values = new Cell?[size, size];
            }

            internal IEnumerable<Move> Swipe(Direction direction) {
                var moves = new List<Move>(size);
                var deletes = new List<T>(size);
                void CollectMoves(int row, int col, int newRow, int newCol) {
                    var cell = values[row, col];
                    values[row, col] = null;
                    if(cell != null) {
                        if(moves.Any() && moves.Last().value == cell.Value.value && !moves.Last().merged) {
                            var last = moves.Last();
                            moves![moves.Count - 1] = last with {
                                value = (Value)((int)last.value!.Value << 1),
                                merged = true,
                            };
                            deletes!.Add(cell.Value.element);
                        } else {
                            moves!.Add(new Move(newRow, newCol, cell.Value.element, cell.Value.value, merged: false));
                        }
                    }
                };
                IEnumerable<Move> YieldMoves() {
                    foreach(var item in moves) {
                        values[item.row, item.col] = new Cell(item.value!.Value, item.element);
                        yield return item;
                    }
                    foreach(var item in deletes) {
                        yield return new Move(-1, -1, item, null, false);
                    }
                    moves.Clear();
                    deletes.Clear();
                }
                if(direction is Direction.Right or Direction.Left) {
                    for(int row = 0; row < size; row++) {
                        var forward = direction is Direction.Left;
                        for(int col = forward ? 0 : lastIndex; forward ? col < size : col >= 0; col += (forward ? 1 : -1)) {
                            CollectMoves(row, col, row, forward ? moves.Count : lastIndex - moves.Count);
                        }
                        foreach(var item in YieldMoves()) {
                            yield return item;
                        }
                    }
                } else {
                    for(int col = 0; col < size; col++) {
                        var forward = direction is Direction.Up;
                        for(int row = forward ? 0 : lastIndex; forward ? row < size : row >= 0; row += (forward ? 1 : -1)) {
                            CollectMoves(row, col, forward ? moves.Count : lastIndex - moves.Count, col);
                        }
                        foreach(var item in YieldMoves()) {
                            yield return item;
                        }
                    }
                }
            }

            public (int row, int col, Value value)? SpawnNew(T element) {
                var emptyCells = new List<(int row, int col)>(size * size);
                for(int row = 0; row < size; row++) {
                    for(int col = 0; col < size; col++) {
                        if(values[row, col] == null)
                            emptyCells.Add((row, col));
                    }
                }
                if(!emptyCells.Any())
                    return null;
                int index = random.Next(0, emptyCells.Count);
                var newValue = random.Next(0, 2) == 1 ? Value.One : Value.Two;
                values[emptyCells[index].row, emptyCells[index].col] = new Cell(newValue, element);
                return (emptyCells[index].row, emptyCells[index].col, newValue);
            }

            public List<Value> GetDistinctValues() {
                var result = new List<Value>();
                for(int row = 0; row < size; row++) {
                    for(int col = 0; col < size; col++) {
                        var value = values[row, col]?.value;
                        if(value != null && !result.Contains(value.Value))
                            result.Add(value.Value);
                    }
                }
                return result;
            }
        }
    }
}

