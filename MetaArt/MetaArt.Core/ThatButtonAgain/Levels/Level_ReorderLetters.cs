using MetaArt.Core;
namespace ThatButtonAgain {
    static class Level_ReorderLetters {
        public static void Load(GameController game) {
            var button = game.CreateButton(() => game.StartNextLevelAnimation()).AddTo(game);
            button.HitTestVisible = true;
            button.IsVisible = false;

            var area = new Area(Area.CreateSwapHShapeArea());
            var letters = game.CreateLetters((letter, index) => {
                letter.Rect = game.GetLetterTargetRect(4 - index, button.Rect);
                letter.HitTestVisible = true;
                letter.GetPressState = (startPoint, releaseState) => {
                    return new DragInputState(
                        startPoint,
                        onDrag: delta => {
                            var direction = DirectionExtensions.GetSwipeDirection(ref delta, game.GetSnapDistance());

                            if(direction == null)
                                return true;

                            if(!area.Move(letter.Value, direction.Value))
                                return true;

                            game.StartLetterDirectionAnimation(letter, direction.Value);
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
                new Vector2(letters[3].Rect.Left, letters[3].Rect.Top - game.letterDragBoxHeight),
                new Vector2(letters[3].Rect.Right, letters[3].Rect.Top - game.letterDragBoxHeight),
                new Vector2(letters[3].Rect.Right, button.Rect.Top),

                new Vector2(letters[1].Rect.Left, button.Rect.Top),
                new Vector2(letters[1].Rect.Left, letters[1].Rect.Top - game.letterDragBoxHeight),
                new Vector2(letters[1].Rect.Right, letters[1].Rect.Top - game.letterDragBoxHeight),
                new Vector2(letters[1].Rect.Right, button.Rect.Top),

                button.Rect.TopRight,
                button.Rect.BottomRight,

                new Vector2(letters[1].Rect.Right, button.Rect.Bottom),
                new Vector2(letters[1].Rect.Right, letters[1].Rect.Bottom + game.letterDragBoxHeight),
                new Vector2(letters[1].Rect.Left, letters[1].Rect.Bottom + game.letterDragBoxHeight),
                new Vector2(letters[1].Rect.Left, button.Rect.Bottom),

                new Vector2(letters[3].Rect.Right, button.Rect.Bottom),
                new Vector2(letters[3].Rect.Right, letters[1].Rect.Bottom + game.letterDragBoxHeight),
                new Vector2(letters[3].Rect.Left, letters[1].Rect.Bottom + game.letterDragBoxHeight),
                new Vector2(letters[3].Rect.Left, button.Rect.Bottom),

                button.Rect.BottomLeft,
            });
            game.scene.AddElementBehind(pathElement);

            new WaitConditionAnimation(
                condition: game.GetAreLettersInPlaceCheck(button.Rect, letters)) {
                End = () => {
                    pathElement.IsVisible = false;
                    button.IsVisible = true;
                    foreach(var item in letters) {
                        item.HitTestVisible = false;
                    }
                    game.playSound(SoundKind.SuccessSwitch);
                }
            }.Start(game);
        }
    }
}

