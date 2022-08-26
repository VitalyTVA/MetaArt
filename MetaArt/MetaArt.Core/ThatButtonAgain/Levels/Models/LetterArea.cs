﻿namespace ThatButtonAgain {
    public class LetterArea {
        public const char X = 'X';
        const char E = ' ';

        public static char[][] CreateSwapHShapeArea() =>
            new char[][] {
                new[] { X, E, X, E, X  },
                new[] { 'H', 'C', 'U', 'O', 'T' },
                new[] { X, E, X, E, X  }
            };
        public static char[][] CreateArrowDirectedLetters() =>
            new char[][] {
                new[] { E, E, 'T', E, E  },
                new[] { E, E, 'O', E, E  },
                new[] { E, E, 'U', E, E  },
                new[] { E, E, 'C', E, E  },
                new[] { E, E, 'H', E, E  },
            };
        public static char[][] CreateMoveInLineArea() =>
            new char[][] {
                new[] { E, E, E, X, E  },
                new[] { E, 'H', E, 'C', E  },
                new[] { X, E, X, E, E  },
                new[] { 'U', E, E, E, E  },
                new[] { E, E, E, E, X  },
                new[] { E, E, E, E, E  },
                new[] { 'O', E, E, E, 'T'  },
            };

        readonly char[][] area;
        public LetterArea(char[][] area) {
            this.area = area;
        }
        int Width => area[0].Length;
        int Height => area.Length;

        public int MoveLine(char letter, Direction direction) {
            int count = 0;
            while(Move(letter, direction)) count++;
            return count;
        }

        public bool Move(char letter, Direction direction) {
            var (row, col) = LocateLetter(letter);
            bool TrySwapLetters(bool canApplyDelta, int deltaRow, int deltaCol) {
                if(!canApplyDelta)
                    return false;
                if(area[row + deltaRow][col + deltaCol] == E) {
                    area[row + deltaRow][col + deltaCol] = letter;
                    area[row][col] = E;
                    return true;
                }
                return false;
            }
            switch(direction) {
                case Direction.Left:
                    return TrySwapLetters(col > 0, 0, -1);
                case Direction.Right:
                    return TrySwapLetters(col < Width - 1, 0, 1);
                case Direction.Up:
                    return TrySwapLetters(row > 0, -1, 0);
                case Direction.Down:
                    return TrySwapLetters(row < Height - 1, 1, 0);
                default:
                    throw new InvalidOperationException();
            }
        }

        public (int row, int col) LocateLetter(char letter) {
            var (row, col, _) = GetLetters().First(x => x.letter == letter);
            return (row, col);
        }

        public IEnumerable<(int row, int col, char letter)> GetLetters() {
            for(int row = 0; row < Height; row++) {
                for(int col = 0; col < Width; col++) {
                    var letter = area[row][col];
                    if(letter != E)
                        yield return (row, col, letter);
                }
            }
        }
    }
}

