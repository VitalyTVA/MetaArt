namespace ThatButtonAgain {
    public class LetterArea {
        const char X = 'X';
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


        readonly char[][] area;
        public LetterArea(char[][] area) {
            this.area = area;
        }
        int Width => area[0].Length;
        int Height => area.Length;

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
        (int row, int col) LocateLetter(char letter) {
            for(int row = 0; row < Height; row++) {
                for(int col = 0; col < Width; col++) {
                    if(area[row][col] == letter)
                        return (row, col);
                }
            }
            throw new InvalidOperationException();
        }
    }
}

