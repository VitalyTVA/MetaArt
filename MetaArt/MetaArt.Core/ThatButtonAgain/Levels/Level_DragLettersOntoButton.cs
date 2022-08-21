using MetaArt.Core;
namespace ThatButtonAgain {
    static class Level_DragLettersOntoButton {
        static readonly (float, float)[] points = new[] {
            (-2.1f, 2.3f),
            (-1.5f, -2.7f),
            (.7f, -1.5f),
            (1.3f, 3.4f),
            (2.3f, -3.4f),
        };
        public static void Load(GameController game) {
            var button = game.CreateButton(() => game.StartNextLevelAnimation()).AddTo(game);
            button.IsEnabled = false;


            var letters = game.CreateLetters((letter, index) => {
                letter.Rect = Rect.FromCenter(
                    new Vector2(button.Rect.MidX + game.letterDragBoxWidth * points[index].Item1, button.Rect.MidY + game.letterDragBoxHeight * points[index].Item2),
                    new Vector2(game.letterDragBoxWidth, game.letterDragBoxHeight)
                ).GetRestrictedRect(game.scene.Bounds);
                game.MakeDraggableLetter(letter, index, button);
            });
            new WaitConditionAnimation(condition: game.GetAreLettersInPlaceCheck(button.Rect, letters)) {
                End = () => button.IsEnabled = true
            }.Start(game);
        }
    }
}

