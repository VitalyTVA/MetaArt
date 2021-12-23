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
            window?.SetLocation(GetLocation());
        }

        public void Hide_() => window?.Hide_();
        public void Show_() => window?.Show_(GetLocation());

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateLocation();
        }

        private Rect GetLocation() {
            return new Rect(PointToScreen(new Point(0, 0)), new Size(ActualWidth, ActualHeight));
        }

        SketchPresenterWindow? window = null;
        public async void Run(Type skecthType) {
            await Stop();

            var location = GetLocation();

            var thread = new Thread(new ThreadStart(() => {
                window = new SketchPresenterWindow(skecthType, location);
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
        Painter painter;
        public SketchPresenterWindow(Type skecthType, Rect ownerRect) {
            Content = img;
            SizeToContent = SizeToContent.WidthAndHeight;
            Topmost = true;
            ShowActivated = false;
            WindowStyle = WindowStyle.None;
            ShowInTaskbar = false;
            ResizeMode = ResizeMode.NoResize;

            painter = new Painter((SketchBase)Activator.CreateInstance(skecthType)!);
            WriteableBitmap? bitmap = null;
            void Unlock() {
                bitmap!.AddDirtyRect(new Int32Rect(0, 0, painter.Width, painter.Height));
                bitmap!.Unlock();
            }
            painter.Setup();
            SetLocationCore(ownerRect);
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
        public void SetLocation(Rect ownerRect) {
            Dispatcher.BeginInvoke(new Action(() => {
                SetLocationCore(ownerRect);

            }));
        }
        void SetLocationCore(Rect ownerRect) {
            Left = ownerRect.X + (ownerRect.Width - painter.Width) / 2;
            Top = ownerRect.Y + (ownerRect.Height - painter.Height ) / 2;
        }
        public void Hide_() {
            Dispatcher.BeginInvoke(new Action(() => {
                Hide();
            }));
        }
        public void Show_(Rect ownerRect) {
            Dispatcher.BeginInvoke(new Action(() => {
                Show();
            }));
        }
    }
}
