﻿using MetaArt.Core;
namespace ThatButtonAgain {
    static class Level_Capital {
        public static void Load_16xClick(GameController game) {
            SetupCapitalLettersSwitchLevel(game, 0b10000, (value, index) => value + 1);
        }
        public static void Load_Mod2Vectors(GameController game) {
            var vectors = new[] {
                0b11001,//--
                0b01010,
                0b10100,//--
                0b10010,//--
                0b00101
            };
            SetupCapitalLettersSwitchLevel(game, 0b00000, (value, index) => value ^ vectors[index]);
        }

        static void SetupCapitalLettersSwitchLevel(GameController game, int initialValue, Func<int, int, int> changeValue) {
            Letter[] letters = null!;

            var button = game.CreateButton(() => game.StartNextLevelAnimation()).AddTo(game);
            button.HitTestVisible = false;

            int value = initialValue;
            const int target = 0b11111;
            void SetLetters() {
                for(int i = 0; i < 5; i++) {
                    bool isCapitalLetter = (value & 1 << 4 - i) > 0;
                    letters[i].Value = (isCapitalLetter ? "TOUCH" : "touch")[i];
                }
            };

            letters = game.CreateLetters((letter, index) => {
                letter.HitTestVisible = true;
                letter.Rect = game.GetLetterTargetRect(index, button.Rect);
                letter.GetPressState = (startPoint, releaseState) => {
                    return new TapInputState(
                        button,
                        () => {
                            game.playSound(SoundKind.Tap);
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
    }
}

