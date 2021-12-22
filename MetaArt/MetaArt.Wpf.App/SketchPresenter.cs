using System;
using System.Diagnostics;
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

            stop = () => {
                painter.NoLoop = true;
            };
            while(true) {
                var sw = new Stopwatch();
                sw.Start();
                bitmap.Lock();
                painter.ptr = bitmap.BackBuffer;

                state.State = State.InProgress;
                await factory.StartNew(() => {
                    painter.Draw();
                    state.State = State.Ready;
                });
                tcs = new TaskCompletionSource();
                onRender = () => painter.UnlockBitmap();
                InvalidateVisual();
                await tcs.Task;
                //painter.UnlockBitmap();
                var sleep = (int)Math.Max(0, 1000f / 61 - sw.ElapsedMilliseconds);
                await Task.Delay(sleep);
                if(painter.NoLoop) {
                    //CompositionTarget.Rendering -= OnRender;
                    break;
                }
            }
            //CompositionTarget.Rendering += OnRender;
        }
        Action? onRender;
        TaskCompletionSource? tcs;
        protected override void OnRender(DrawingContext dc) {
            base.OnRender(dc);
            onRender?.Invoke();
            tcs?.SetResult();
            onRender = null;
            tcs = null;
        }
        enum State { None, InProgress, Ready }
    }
}
