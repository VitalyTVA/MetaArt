using MetaArt.Core;
namespace ThatButtonAgain {
    public static class Level_BouncyBalls {
        public static void Load(GameController game) {
            //var balls = new Ball[12];
            //for(int i = 0; i < balls.Length; i++) {
            //    balls[i] = new Ball(MathFEx.Random(0, scene.width), MathFEx.Random(0, scene.height), MathFEx.Random(30, 70));
            //}

            bool win = false;
            var button = game.CreateButton(() => {
                win = true;
                game.StartNextLevelAnimation();
            }).AddTo(game);
            button.IsEnabled = false;
            button.Rect = button.Rect.Offset(new Vector2(0, -button.Rect.Width / 2));

            var hitBallLocation = new Vector2(button.Rect.MidX, game.height - button.Rect.Width * .7f);
            var spring = new Spring { From = hitBallLocation, To = hitBallLocation }.AddTo(game);

            var letters = game.CreateLetters((letter, index) => {
                letter.Rect = game.GetLetterTargetRect(index, button.Rect);
                letter.Scale = new Vector2(.65f);
            });
            letters[1].IsVisible = false;

            Ball oBall = null!;
            var diameter = game.letterDragBoxWidth * .7f;
            var simulation = new BallsSimulation(size: null, gravity: 0, onHit: (b1, b2) => {
                if(b1 == oBall || b2 == oBall) {
                    b1.Element().Broken = true;
                    b2.Element().Broken = true;
                    game.playSound(SoundKind.BrakeBall);
                } else {
                    game.playSound(SoundKind.Tap);
                }
            });
            List<Ball> balls = new();
            Ball CreateBall(Vector2 center) {
                var ball = new Ball(new BallElement { Rect = Rect.FromCenter(center, new Vector2(diameter)) }, diameter) {
                    x = center.X,
                    y = center.Y,
                };
                balls.Add(ball);
                ball.Element().AddTo(game);
                return ball;
            }

            foreach(var item in letters) {
                simulation.AddBall(CreateBall(item.Rect.Mid));
            }

            oBall = balls[1];
            var oBallLocation = oBall.Element().Rect.Location;

            void SetLocation(Ball ball, Vector2 location) {
                ball.x = location.X;
                ball.y = location.Y;
                ball.Element().Rect = Rect.FromCenter(location, ball.Element().Rect.Size);
            }

            float maxSpringLength = game.buttonWidth * 0.5f;

            void CreateHitBall() {
                var hitBall = CreateBall(hitBallLocation);
                hitBall.Element().HitTestVisible = true;
                var startLocation = hitBall.Element().Rect.Mid;
                hitBall.Element().GetPressState = (starPoint, releaseState) => {
                    return new DragInputState(
                        starPoint,
                        onDrag: delta => {
                            var deltaLength = delta.Length();
                            if(MathFEx.Greater(deltaLength, 0))
                                delta *= MathFEx.Min(maxSpringLength, deltaLength) / deltaLength;
                            SetLocation(hitBall, startLocation + delta);
                            spring.To = hitBall.Element().Rect.Mid;
                            return true;
                        },
                        onRelease: delta => {
                            spring.To = spring.From;
                            if(delta.Length() > game.GetSnapDistance()) {
                                delta = -delta * .15f;
                                hitBall.vx = delta.X;
                                hitBall.vy = delta.Y;
                                hitBall.Element().GetPressState = null;
                                simulation.AddBall(hitBall);
                            } else {
                                SetLocation(hitBall, startLocation);
                            }
                        },
                        releaseState);

                };
            }

            CreateHitBall();

            var toRemove = new List<(Ball, BallElement)>();
            new DelegateAnimation(deltaTime => {
                simulation.NextFrame();
                foreach(var ball in balls) {
                    SetLocation(ball, new Vector2(ball.x, ball.y));
                    if(!ball.Element().Rect.Intersects(new Rect(0, 0, game.width, game.height)))
                        toRemove.Add((ball, ball.Element()));
                }
                foreach(var (ball, element) in toRemove) {
                    balls.Remove(ball);
                    game.scene.RemoveElement(ball.Element());
                    simulation.RemoveBall(ball);
                }
                toRemove.Clear();
                var reloadArea = Rect.FromCenter(hitBallLocation, new Vector2(maxSpringLength + diameter) * 2);
                if(!balls.Any(x => reloadArea.Intersects(x.Element().Rect)))
                    CreateHitBall();
                button.IsEnabled = !balls.Any(x => x != oBall && button.Rect.Intersects(x.Element().Rect));
                if(!MathFEx.VectorsEqual(oBallLocation, oBall.Element().Rect.Location)) {
                    game.StartReloadLevelAnimation();
                    return false;
                }
                return !win;
            }).Start(game);
        }
    }
}

