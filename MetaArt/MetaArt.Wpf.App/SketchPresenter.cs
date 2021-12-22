using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MetaArt.Wpf {
    public class SketchPresenter : Border {
        Image img = new Image() { Stretch = Stretch.None, VerticalAlignment = VerticalAlignment.Center };
        public SketchPresenter() {
            Child = img;
        }
        SingleThreadTaskScheduler scheduler = new();
        public void Stop() {
            StopRender();
            scheduler.Complete();
        }

        void StopRender() {
            stop?.Invoke();
            stop = null;
        }
        Action? stop;
        public async void Run(Type skecthType) {
            StopRender();

            var painter = new Painter((SketchBase)Activator.CreateInstance(skecthType)!);
            WriteableBitmap? bitmap = null;
            void Unlock() {
                bitmap!.AddDirtyRect(new Int32Rect(0, 0, painter.Width, painter.Height));
                bitmap!.Unlock();
            }
            if(painter.Async) {
                var factory = new TaskFactory(
                    CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                    TaskContinuationOptions.None, scheduler);
                bitmap = painter.Bitmap;

                img.Source = bitmap;
                await factory.StartNew(() => {
                    painter.Setup();
                });
                Unlock();

                bool shouldStop = false;
                stop = () => shouldStop = true;
                while(!shouldStop) {
                    var sw = new Stopwatch();
                    sw.Start();
                    bitmap.Lock();
                    painter.ptr = bitmap.BackBuffer;
                    await factory.StartNew(() => {
                        painter.Draw();
                    });
                    Unlock();
                    sw.Stop();
                    var sleep = (int)Math.Max(0, 1000f / 60 - sw.ElapsedMilliseconds);
                    await Task.Delay(sleep);
                    if(painter.NoLoop)
                        break;
                }
            } else {
                painter.Setup();
                bitmap = painter.Bitmap;
                Unlock();
                img.Source = bitmap;
                void OnRender(object? o, EventArgs e) {
                    bitmap.Lock();
                    painter.ptr = bitmap.BackBuffer;

                    painter.Draw();
                    Unlock();

                    if(painter.NoLoop)
                        StopRender();
                }
                CompositionTarget.Rendering += OnRender;
                stop = () => CompositionTarget.Rendering -= OnRender;
            }
        }
    }
}
