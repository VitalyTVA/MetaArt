using MetaArt.Core;
namespace ThatButtonAgain {
    static class Level_RotatingArrow {
        public static void Load(GameController game) {
            var button = game.CreateButton(() => game.StartNextLevelAnimation()).AddTo(game);
            button.HitTestVisible = false;

            var direction = Direction.Right;

            var arrow = new Letter {
                Value = '>',
                Rect = game.GetLetterTargetRect(2, button.Rect),
                ActiveRatio = 0,
                Angle = direction.ToAngle(),
            }.AddTo(game);
            arrow.Rect = arrow.Rect.SetLocation(new Vector2(arrow.Rect.Left, game.levelNumberElementRect.Top));

            var area = new Area(Area.CreateArrowDirectedLetters());
            //HHTTHHTTCCOO
            var letters = game.CreateLetters((letter, index) => {
                letter.Rect = game.GetLetterTargetRect(2, button.Rect, row: index - 2);
                letter.HitTestVisible = true;
                letter.GetPressState = Element.GetPressReleaseStateFactory(
                    letter,
                    onPress: () => {
                        if(!area.Move(letter.Value, direction)) {
                            game.playSound(SoundKind.Tap);
                            return;
                        }
                        //Debug.WriteLine(letter.Value);
                        game.StartLetterDirectionAnimation(letter, direction);
                        direction = direction.RotateCounterClockwize();
                        arrow.Angle = direction.ToAngle();
                    },
                    onRelease: () => { }
                );
            });


            new WaitConditionAnimation(condition: game.GetAreLettersInPlaceCheck(button.Rect, letters)) {
                End = () => {
                    button.HitTestVisible = true;
                    foreach(var item in letters) {
                        item.HitTestVisible = false;
                    }
                }
            }.Start(game);
        }
    }
}

