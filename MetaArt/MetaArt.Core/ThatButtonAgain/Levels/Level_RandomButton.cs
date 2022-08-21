using MetaArt.Core;
namespace ThatButtonAgain {
    static class Level_RandomButton {
        public static void Load(GameController game) {
            AnimationBase appearAnimation = null!;
            AnimationBase disappearAnimation = null!;
            var button = game.CreateButton(() => {
                game.animations.RemoveAnimation(appearAnimation);
                game.animations.RemoveAnimation(disappearAnimation);
                game.StartNextLevelAnimation();
            }).AddTo(game);
            var letters = game.CreateLetters((letter, index) => {
                letter.Rect = game.GetLetterTargetRect(index, button.Rect);
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
                appearAnimation = WaitConditionAnimation.WaitTime(
                    TimeSpan.FromMilliseconds(GetWaitTime()),
                    () => {
                        SetVisibility(true);
                        disappearAnimation = WaitConditionAnimation.WaitTime(
                            TimeSpan.FromMilliseconds(appearInterval),
                            () => {
                                appearInterval = Math.Min(appearInterval + Constants.ButtonAppearIntervalIncrease, Constants.MaxButtonAppearInterval);
                                SetVisibility(false);
                                StartWaitButton();
                            }
                        ).Start(game);
                    }
                ).Start(game);
            };

            StartWaitButton();

        }
    }
}

