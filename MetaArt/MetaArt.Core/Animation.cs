namespace MetaArt.Core {
    public interface IAnimation {
        bool Next(TimeSpan deltaTime);
        void End();
    }
    public sealed class WaitConditionAnimation : IAnimation {
        readonly Func<bool> condition;
        readonly Action end;

        public WaitConditionAnimation(Func<bool> condition, Action end) {
            this.condition = condition;
            this.end = end;
        }
        void IAnimation.End() {
            end();
        }
        bool IAnimation.Next(TimeSpan deltaTime) {
            return !condition();
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
        public Func<(T from, T to), float, T> Lerp { get; init; } = null!;
        public Action<T> SetValue { get; init; } = null!;

        protected override T LerpCore(T from, T to, float amount) => Lerp((from, to), amount);
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
        readonly List<IAnimation> animations;
        //readonly Action<bool> updateState;
        //public bool HasAnimations => animations.Any();
        public AnimationsController(IAnimation[] animations/*, Action<bool> updateState*/) {
            this.animations = animations.ToList();
            //this.updateState = updateState;
        }
        public void AddAnimation(IAnimation animation) {
            animations.Add(animation);
            //UpdateState();
        }

        //private void UpdateState() {
        //    updateState(animations.Any());
        //}

        public void Next(TimeSpan deltaTime) {
            foreach(var animation in animations.ToArray()) {
                bool finished = !animation.Next(deltaTime);
                if(finished) {
                    animations.Remove(animation);
                    animation.End();
                }
            }
        }
    }
}
