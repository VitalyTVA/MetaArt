namespace MetaArt.Core {
    public interface IAnimation {
        bool Next(TimeSpan deltaTime);
        void End();
    }
    public sealed class Animation<T, TTarget> : IAnimation {
        public TimeSpan Duration { get; init; }
        public T From { get; init; } = default!;
        public T To { get; init; } = default!;
        public TTarget Target { get; init; } = default!;
        public Action<TTarget, T> SetValue { get; init; } = null!;
        public Func<(T from, T to), float, T> Lerp { get; init; } = null!;
        public Action? OnEnd { get; init; }

        TimeSpan time = TimeSpan.Zero;
        public bool Next(TimeSpan deltaTime) {
            time += deltaTime;
            float amount = MathFEx.Min(1, (float)(time.TotalMilliseconds / Duration.TotalMilliseconds));
            var value = Lerp((From, To), amount);
            SetValue(Target, value);
            return amount < 1;
        }
        public void End() => OnEnd?.Invoke();
    }
    public class AnimationsController {
        readonly List<IAnimation> animations;
        //readonly Action<bool> updateState;
        public bool HasAnimations => animations.Any();
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
