namespace MetaArt.Core {
    public interface IAnimation {
        bool Next(TimeSpan deltaTime);
        void End();
    }
    public sealed class WaitConditionAnimation : IAnimation {
        public static WaitConditionAnimation WaitTime(TimeSpan time, Action end) {
            var totalTime = TimeSpan.Zero;
            return new WaitConditionAnimation(deltaTime => {
                totalTime += deltaTime;
                return totalTime > time;
            }, end);
        }

        readonly Func<TimeSpan, bool> condition;
        readonly Action end;

        public WaitConditionAnimation(Func<TimeSpan, bool> condition, Action end) {
            this.condition = condition;
            this.end = end;
        }
        void IAnimation.End() {
            end();
        }
        bool IAnimation.Next(TimeSpan deltaTime) {
            return !condition(deltaTime);
        }
    }

    public abstract class LinearAnimationBase<T> : IAnimation {
        public TimeSpan Duration { get; init; }
        public T From { get; init; } = default!;
        public T To { get; init; } = default!;
        public Action? OnEnd { get; init; }

        TimeSpan time = TimeSpan.Zero;
        public bool Next(TimeSpan deltaTime) {
            time += deltaTime;
            float amount = MathFEx.Min(1, (float)(time.TotalMilliseconds / Duration.TotalMilliseconds));
            var value = LerpCore(From, To, amount);
            SetValueCore(value);
            return amount < 1;
        }
        public void End() => OnEnd?.Invoke();

        protected abstract T LerpCore(T from, T to, float amount);
        protected abstract void SetValueCore(T value);
    }

    public sealed class LerpAnimation<T> : LinearAnimationBase<T> {
        public Func<T, T, float, T> Lerp { get; init; } = null!;
        public Action<T> SetValue { get; init; } = null!;

        protected override T LerpCore(T from, T to, float amount) => Lerp(from, to, amount);
        protected override void SetValueCore(T value) => SetValue(value);
    }

    public sealed class RotateAnimation : LinearAnimationBase<float> {
        public float Radius { get; init; } = default!;
        public Vector2 Center { get; init; } = default!;
        public Action<Vector2> SetLocation { get; init; } = null!;

        protected override float LerpCore(float from, float to, float amount) => MathFEx.Lerp(from, to, amount);
        protected override void SetValueCore(float value) {
            SetLocation(Center + Radius * new Vector2(MathFEx.Cos(value), MathFEx.Sin(value)));
        }
    }

    public class AnimationsController {
        public bool AllowInput => !blockInputAnimations.Any();
        readonly List<IAnimation> animations = new();
        readonly List<IAnimation> blockInputAnimations = new();
        public AnimationsController() {
            this.animations = animations.ToList();
        }
        public void AddAnimation(IAnimation animation, bool blockInput = false) {
            animations.Add(animation);
            if(blockInput)
                blockInputAnimations.Add(animation);
        }
        public void RemoveAnimation(IAnimation animation) {
            animations.Remove(animation);
            blockInputAnimations.Remove(animation);
        }

        public void Next(TimeSpan deltaTime) {
            foreach(var animation in animations.ToArray()) {
                bool finished = !animation.Next(deltaTime);
                if(finished) {
                    RemoveAnimation(animation);
                    animation.End();
                }
            }
        }

        public void AddAnimations(IEnumerable<IAnimation> animations) {
            foreach(var item in animations) {
                AddAnimation(item);
            };
        }
    }
}
