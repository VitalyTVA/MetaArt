using MetaArt.Core;
namespace ThatButtonAgain {
    static class Level_Reorder {
        public static void Load_MoveInLine(GameController game) {
            var area = new LetterArea(LetterArea.CreateMoveInLineArea());
            LoadCore(
                game,
                area,
                letter => {
                    return (Vector2 startPoint, NoInputState releaseState) => {
                        return new DragInputState(
                        startPoint,
                        onDrag: delta => {
                            var direction = DirectionExtensions.GetSwipeDirection(ref delta, game.GetSnapDistance());

                            if(direction == null)
                                return true;

                            var count = area.MoveLine(letter.Value, direction.Value);
                            if(count == 0)
                                return true;

                            game.StartLetterDirectionAnimation(letter, direction.Value, count);
                            game.playSound(direction.Value.GetSound());
                            return false;
                        },
                        onRelease: delta => {
                        },
                        releaseState);

                    };
                },
                onWin: () => { }
            );
            #region solution
            /*
            'U' Down
            'C' Down
            'C' Left
            'C' Up
            'T' Up
            'T' Left
            'T' Up
            'T' Left
            'T' Down
            'O' Right
            'O' Up
            'O' Left
            'U' Down
            'U' Right
            'U' Up
            'U' Left
            'O' Up
            'U' Up
            'H' Up
            'C' Up
            'H' Right
            'H' Down
            'H' Right
            'C' Right
            'H' Down
            'T' Right
            'C' Down
            'T' Left
            'T' Up
             */
            #endregion
        }
        public static void Load_MoveAll(GameController game) {
            var area = new LetterArea(LetterArea.CreateMoveAllArea());

            var inputHandler = new InputHandlerElement();

            var letters = LoadCore(
                game,
                area,
                letter => default(Func<Vector2, NoInputState, InputState>),
                onWin: () => game.scene.RemoveElement(inputHandler)
            );

            SetupInputHandler(game, area, inputHandler, direction => {
                var moves = area.MoveAll(direction);
                foreach(var (row, col, letter) in moves) {
                    game.StartLetterDirectionAnimation(letters.Single(x => x.Value == letter), direction);
                }
                game.playSound(direction.GetSound());
            });
            #region solution
/*
Up
Up
Left
Down
Left
Down
Up
Up
Left
Down
Left
Left
Up
Down
Right
Right
Right
Up
Right
Right
Up
Left
Down
Left
Down
Down
Down
Down
Up
Right
Up
Up
Up
Down
Left
Left
Down
 */
            #endregion
        }
        public static void Load_Sokoban(GameController game) {
            var area = new LetterArea(LetterArea.CreateSokobanArea());

            var inputHandler = new InputHandlerElement();

            var letters = LoadCore(
                game,
                area,
                letter => default(Func<Vector2, NoInputState, InputState>),
                onWin: () => game.scene.RemoveElement(inputHandler)
            );

            var robotLocation = area.LocateLetter(LetterArea.Robot);
            var robotLetter = new Letter {
                Value = LetterArea.Robot,
                Rect = game.GetLetterTargetRect(robotLocation.col - area.Width / 2 + 2, game.GetButtonRect(), row: robotLocation.row - area.Height / 2),
                Style = LetterStyle.Accent5,
            }.AddTo(game);

            SetupInputHandler(game, area, inputHandler, direction => {
                var robot = area.LocateLetter(LetterArea.Robot);
                var offset = direction.GetOffset();
                var letter = area.GetLetter(robot.row + offset.row, robot.col + offset.col);

                var moveLetter = letter != null ? area.Move(letter.Value, direction) : false;
                var moveRobot = area.Move(LetterArea.Robot, direction);

                if(moveLetter) {
                    game.StartLetterDirectionAnimation(letters.Single(x => x.Value == letter), direction);
                }
                if(moveRobot) {
                    game.StartLetterDirectionAnimation(robotLetter, direction);
                    game.playSound(direction.GetSound());
                }
            });
            #region solution
            /*
             TODO ;)
             */
            #endregion
        }

        static void SetupInputHandler(GameController game, LetterArea area, InputHandlerElement inputHandler, Action<Direction> onSwipe) {
            inputHandler.Rect = game.GetContainingRect(area);
            inputHandler.GetPressState = (startPoint, releaseState) => {
                return new DragInputState(
                    startPoint,
                    onDrag: delta => {
                        var direction = DirectionExtensions.GetSwipeDirection(ref delta, game.GetSnapDistance());

                        if(direction == null)
                            return true;

                        onSwipe(direction.Value);
                        return false;
                    },
                    onRelease: delta => { },
                    releaseState
                );
            };
            inputHandler.AddTo(game);
        }

        static Letter[] LoadCore(
            GameController game,
            LetterArea area,
            Func<Letter, Func<Vector2, NoInputState, InputState>?> getPressStateHandler,
            Action onWin
        ) {
            var button = game.CreateButton(() => game.StartNextLevelAnimation()).AddTo(game);
            button.HitTestVisible = false;
            button.Rect = game.GetContainingRect(area);
            button.Filled = false;

            var rowsOffset = area.Height / 2;
            var colsOffset = area.Width / 2 - 2;
            Letter[] letters = null!;
            letters = game.CreateLetters((letter, index) => {
                var (row, col) = area.LocateLetter(letter.Value);
                letter.Rect = game.GetLetterTargetRect(col - colsOffset, button.Rect, row: row - rowsOffset);
                letter.HitTestVisible = true;
                letter.GetPressState = getPressStateHandler(letter);
                letter.ActiveRatio = 0;
            });

            foreach(var (row, col, _) in area.GetLetters().Where(x => x.letter == LetterArea.X)) {
                var letter = new Letter {
                    Value = '*',
                    Rect = game.GetLetterTargetRect(col - colsOffset, button.Rect, row: row - rowsOffset),
                    //Style = LetterStyle.Accent4,
                }.AddTo(game);
            }

            var condition = game.GetAreLettersInPlaceCheck(button.Rect, letters);
            new WaitConditionAnimation(
                condition: deltaTime => {
                    game.ActivateInplaceLetters(letters);
                    return condition(deltaTime);
                }) {
                End = () => {
                    button.Rect = game.GetButtonRect();
                    button.Filled = true;
                    button.HitTestVisible = true;
                    foreach(var item in letters) {
                        item.HitTestVisible = false;
                    }
                    game.playSound(SoundKind.SuccessSwitch);
                    onWin();
                }
            }.Start(game);

            return letters;
        }
    }
}

