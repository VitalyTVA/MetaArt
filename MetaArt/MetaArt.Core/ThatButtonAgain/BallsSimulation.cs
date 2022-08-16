using MetaArt.Core;

namespace ThatButtonAgain {
    public class BallsSimulation {
        public BallsSimulation(float width, float height) {
            this.width = width;
            this.height = height;
        }

        public void AddBalls(params Ball[] balls) {
            this.balls.AddRange(balls);
        }

        const float spring = 0.05f;
        const float gravity = 0.03f;
        const float friction = -0.9f;
        readonly float width;
        readonly float height;
        List<Ball> balls = new();

        public IEnumerable<Ball> GetBalls() => balls;

        public void NextFrame() {
            for(int i = 0; i < balls.Count; i++) {
                var ball = balls[i];
                for(int j = i + 1; j < balls.Count; j++) {
                    var other = balls[j];
                    CollideBalls(ball, other);
                }
                MoveBall(ball);
            }
        }

        static void CollideBalls(Ball ball, Ball other) {
            float dx = other.x - ball.x;
            float dy = other.y - ball.y;
            float distance = MathFEx.Sqrt(dx * dx + dy * dy);
            float minDist = other.diameter / 2 + ball.diameter / 2;
            if(distance < minDist) {
                float angle = MathFEx.Atan2(dy, dx);
                float targetX = ball.x + MathFEx.Cos(angle) * minDist;
                float targetY = ball.y + MathFEx.Sin(angle) * minDist;
                float ax = (targetX - other.x) * spring;
                float ay = (targetY - other.y) * spring;
                ball.vx -= ax;
                ball.vy -= ay;
                other.vx += ax;
                other.vy += ay;
            }
        }

        void MoveBall(Ball ball) {
            ball.vy += gravity;
            ball.x += ball.vx;
            ball.y += ball.vy;
            if(ball.x + ball.diameter / 2 > width) {
                ball.x = width - ball.diameter / 2;
                ball.vx *= friction;
            } else if(ball.x - ball.diameter / 2 < 0) {
                ball.x = ball.diameter / 2;
                ball.vx *= friction;
            }
            if(ball.y + ball.diameter / 2 > height) {
                ball.y = height - ball.diameter / 2;
                ball.vy *= friction;
            } else if(ball.y - ball.diameter / 2 < 0) {
                ball.y = ball.diameter / 2;
                ball.vy *= friction;
            }
        }
    }
}

