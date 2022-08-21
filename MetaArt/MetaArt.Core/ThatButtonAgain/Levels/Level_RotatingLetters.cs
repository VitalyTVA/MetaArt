using MetaArt.Core;
namespace ThatButtonAgain {
    public static class Level_RotatingLetters {
        /*
//002233 medium - hard
var rotations = new[] {
    new[] { 1, 0, 0, -2, 0 },
    new[] { 0, 2, 0, -1, 1 },
    new[] { -2, 0, 1, 0, 1 },
    new[] { 0, -1, 0, 1, 0 },
    new[] { 1, -2, 0, 0, 1 },
};
//0144 hard
var rotations = new[] {
    new[] { 1, -1, 0, -2, 0 },
    new[] { -1, 1, 0, 0, 0 },
    new[] { 2, 0, -1, 0, -1 },
    new[] { 2, 0, 1, -1, 0 },
    new[] { -1, 0, 1, 0, 1 },
};
//04 medium
var rotations = new[] {
    new[] { 1, 0, 0, -2, 1 },
    new[] { 0, -1, 2, 0, 0 },
    new[] { -2, 0, 1, 0, 0 },
    new[] { -1, 0, 0, 1, 0 },
    new[] { 1, 0, 2, 0, -1 },
};
*/

        //01234
        static readonly int[][] rotations = new[] {
                new[] { 1, 0, 0, -1, 0 },
                new[] { 0, 1, 0, 0, -1 },
                new[] { 0, -1, 1, 0, 0 },
                new[] { 0, 0, 1, -1, 0 },
                new[] { 1, 0, 0, 0, 1 },
            };

        public static void Load(GameController game) {
            var button = game.CreateButton(() => game.StartNextLevelAnimation()).AddTo(game);
            button.HitTestVisible = false;

            void StartRotation(Letter letter, float delta) {
                new LerpAnimation<float> {
                    Duration = TimeSpan.FromMilliseconds(300),
                    From = letter.Angle,
                    To = letter.Angle + delta,
                    SetValue = val => letter.Angle = val,
                    Lerp = MathFEx.Lerp,
                    End = () => {
                        letter.Angle = (letter.Angle + MathFEx.PI * 2) % (MathFEx.PI * 2);
                        VerifyPositiveAngle(letter);
                    }
                }.Start(game, blockInput: true);
            }

            Letter[] letters = null!;
            letters = game.CreateLetters((letter, index) => {
                letter.Rect = game.GetLetterTargetRect(index, button.Rect);
                var onPress = () => {
                    Debug.WriteLine(index.ToString());
                    game.playSound(SoundKind.Rotate);
                    for(int i = 0; i < 5; i++) {
                        int rotation = rotations[index][i];
                        if(rotation != 0) {
                            StartRotation(letters[i], rotation * MathFEx.PI / 2);

                        }
                    }
                };
                letter.GetPressState = Element.GetPressReleaseStateFactory(letter, onPress, () => { });
                letter.HitTestVisible = true;
                letter.Angle = MathFEx.PI;
            });

            new WaitConditionAnimation(
                condition: delta => letters.All(l => {
                    if(l.Value is 'O' or 'H' && (MathFEx.FloatsEqual(MathFEx.PI, l.Angle) || MathFEx.FloatsEqual(-MathFEx.PI, l.Angle))) {
                        VerifyPositiveAngle(l);
                        return true;
                    }
                    return MathFEx.FloatsEqual(0, l.Angle) || MathFEx.FloatsEqual(MathFEx.PI * 2, l.Angle);
                })) {
                End = () => {
                    button.HitTestVisible = true;
                    foreach(var item in letters) {
                        item.HitTestVisible = false;
                    }
                }
            }.Start(game);
        }
        static void VerifyPositiveAngle(Letter letter) {
            //TODO use logging
            Debug.Assert(MathFEx.GreaterOrEqual(letter.Angle, 0), "Letter's angle is negative");
        }
    }
}

