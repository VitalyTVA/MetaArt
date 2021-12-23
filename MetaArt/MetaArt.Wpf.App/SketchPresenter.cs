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
        public SketchPresenter() {
        }
        public async Task Stop() {
            if(window == null)
                return;
            await window.Stop();
            window = null;
        }
        public void UpdateLocation() {
            var location = PointToScreen(new Point(0, 0));
            window?.SetLocation(location);
        }

        SketchPresenterWindow? window = null;
        public async void Run(Type skecthType) {
            await Stop();

            var location = PointToScreen(new Point(0, 0));

            var thread = new Thread(new ThreadStart(() => {
                window = new SketchPresenterWindow(skecthType);
                window.Left = location.X;
                window.Top = location.Y;
                window.Show();
                Dispatcher.Run();

            }));
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }
    }

    class SketchPresenterWindow : Window {
        Image img = new Image() { Stretch = Stretch.None, VerticalAlignment = VerticalAlignment.Center };
        public SketchPresenterWindow(Type skecthType) {
            Content = img;
            SizeToContent = SizeToContent.WidthAndHeight;
            Topmost = true;
            ShowActivated = false;

            var painter = new Painter((SketchBase)Activator.CreateInstance(skecthType)!);
            WriteableBitmap? bitmap = null;
            void Unlock() {
                bitmap!.AddDirtyRect(new Int32Rect(0, 0, painter.Width, painter.Height));
                bitmap!.Unlock();
            }
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
                    stop!();
            }
            onRender = () => {
                CompositionTarget.Rendering += OnRender;
            };
            stop = () => CompositionTarget.Rendering -= OnRender;
        }
        Action? onRender;
        protected override void OnContentRendered(EventArgs e) {
            base.OnContentRendered(e);
            onRender?.Invoke();
            onRender = null;
        }
        protected override void OnClosed(EventArgs e) {
            stop();
        }
        Action stop;

        public async Task Stop() {
            await Dispatcher.BeginInvoke(new Action(() => { Close(); }));
        }
        public void SetLocation(Point location) {
            Dispatcher.BeginInvoke(new Action(() => {
                Left = location.X;
                Top = location.Y;

            }));
        }
        //public void Hide_() {
        //    Dispatcher.BeginInvoke(new Action(() => {
        //        Hide();
        //    }));
        //}
        //public void Show(Point location) {
        //    Dispatcher.BeginInvoke(new Action(() => {
        //        Show();
        //    }));
        //}
    }
}
