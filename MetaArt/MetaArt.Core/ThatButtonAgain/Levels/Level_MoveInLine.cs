using MetaArt.Core;
namespace ThatButtonAgain {
    static class Level_MoveInLine {
        public static void Load(GameController game) {
            var button = game.CreateButton(() => game.StartNextLevelAnimation()).AddTo(game);
            button.HitTestVisible = false;

            var area = new LetterArea(LetterArea.CreateMoveInLineArea());
            var letters = game.CreateLetters((letter, index) => {
                var (row, col) = area.LocateLetter(letter.Value);
                letter.Rect = game.GetLetterTargetRect(col, button.Rect, row: row - 3);
                letter.HitTestVisible = true;
                letter.GetPressState = (startPoint, releaseState) => {
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
                            return false;
                        },
                        onRelease: delta => {
                        },
                        releaseState);

                };
            });

            foreach(var (row, col, _) in area.GetLetters().Where(x => x.letter == LetterArea.X)) {
                var letter = new Letter {
                    Value = '*',
                    Rect = game.GetLetterTargetRect(col, button.Rect, row: row - 3),
                }.AddTo(game);
            }

            new WaitConditionAnimation(
                condition: game.GetAreLettersInPlaceCheck(button.Rect, letters)) {
                End = () => {
                    button.HitTestVisible = true;
                    foreach(var item in letters) {
                        item.HitTestVisible = false;
                    }
                    game.playSound(SoundKind.SuccessSwitch);
                }
            }.Start(game);
        }
    }
}
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

