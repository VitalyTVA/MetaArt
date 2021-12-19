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
        public void Stop() => scheduler.Complete();
        SingleThreadTaskScheduler scheduler = new();
        public async void Run(Type skecthType) {
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



            var timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(30) };



            timer.Tick += async (o, e) => {
                bitmap.Lock();
                painter.ptr = bitmap.BackBuffer;

                await factory.StartNew(() => {
                    painter.Draw();
                });
                bitmap.AddDirtyRect(new Int32Rect(0, 0, painter.Width, painter.Height));
                bitmap.Unlock();
            };
            timer.Start();
        }

    }
}
