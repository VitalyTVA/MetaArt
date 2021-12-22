using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace MetaArt.Wpf {
    public class SketchPresenter : Border {
        Image img = new Image() { Stretch = Stretch.None, VerticalAlignment = VerticalAlignment.Center };
        public SketchPresenter() {
            Child = img;
        }
        public void Stop() {
            StopRender();
            scheduler.Complete();
        }

        private void StopRender() {
            stop?.Invoke();
            stop = null;
        }

        SingleThreadTaskScheduler scheduler = new();

        //DispatcherTimer? timer;

        Action? stop;
        class StateValue {
            public volatile State State = State.None;
        }
        public async void Run(Type skecthType) {
            StopRender();
            StateValue state = new();
            var factory = new TaskFactory(
                CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                TaskContinuationOptions.None, scheduler);


            var size = img.RenderSize;
            var painter = await factory.StartNew(() => {
                var sk = (SketchBase)Activator.CreateInstance(skecthType)!;
                var painter = new Painter(sk);
                painter.Setup();
                return painter;
            });

            var bitmap = painter.Bitmap;
            var field = typeof(DispatcherObject).GetField("_dispatcher", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
            field.SetValue(bitmap, this.Dispatcher);

            img.Source = bitmap;

            int renderCount = 0;
            async void OnRender(object? o, EventArgs e) {
                renderCount++;
                if(state.State == State.Ready) {
                    state.State = State.None;
                    painter.UnlockBitmap();
                    //img.InvalidateVisual();
                    if(painter.NoLoop) {
                        CompositionTarget.Rendering -= OnRender;
                        return;
                    }
                } else if(state.State == State.InProgress) {
                    return;
                }

                bitmap.Lock();
                painter.ptr = bitmap.BackBuffer;

                state.State = State.InProgress;
                await factory.StartNew(() => {
                    painter.Draw();
                    state.State = State.Ready;
                });
            }
            CompositionTarget.Rendering += OnRender;
            stop = () => {
                painter.NoLoop = true;
            };
        }
        enum State { None, InProgress, Ready }
    }
}
