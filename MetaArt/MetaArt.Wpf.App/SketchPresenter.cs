using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MetaArt.Wpf {
    public class SketchPresenter : Border {
        ScrollViewer scrollViewer = new ScrollViewer { 
            CanContentScroll = true, 
            VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Visible,
        };
        public SketchPresenter() {
            scrollViewer.Content = new ScrollableSketch(this);
            Child = scrollViewer;
        }
        public async Task Stop() {
            if(form == null)
                return;
            await form.Stop();
            form = null;
        }

        public void UpdateLocation() {
            if(form == null)
                return;
            form.SetLocation(GetLocation());
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateLocation();
            ChangeOffset(0, 0);
        }

        private System.Drawing.Rectangle GetLocation() {
            var p = PointToScreen(new System.Windows.Point(0, 0));

            var presenter = Presenter;

            return new System.Drawing.Rectangle(
                (int)p.X, 
                (int)p.Y, 
                (int)presenter.ActualWidth, 
                (int)presenter.ActualHeight
            );
            //PresentationSource source = PresentationSource.FromVisual(this);

            //double dpiX = 1, dpiY = 1;
            //if(source != null) {
            //    dpiX = source.CompositionTarget.TransformToDevice.M11;
            //    dpiY = source.CompositionTarget.TransformToDevice.M22;
            //}
            //var w = Window.GetWindow(this);
            //var p =  new Point((w.Left + w.Width) * dpiX, w.Top * dpiY);
            //return new System.Drawing.Point((int)p.X, (int)p.Y);
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject {
            for(int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++) {
                var child = VisualTreeHelper.GetChild(depObj, i);

                if(child != null && child is T)
                    yield return (T)child;
                if(child != null) {
                    foreach(T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }

        SketchForm? form = null;
        public async void Run(Type skecthType) {
            await Stop();

            offset = System.Drawing.Point.Empty;

            var rect = GetLocation();

            var thread = new Thread(new ThreadStart(() => {
                form = new SketchForm(
                    skecthType, 
                    rect, size => {
                        Dispatcher.BeginInvoke(new Action(() => {
                            sketchSize = size;
                            scrollViewer.InvalidateScrollInfo();
                        }));
                    },
                    delta => {
                        Dispatcher.BeginInvoke(new Action(() => {
                            ChangeOffset(0, delta);
                            scrollViewer.InvalidateScrollInfo();
                        }));
                    });
                form.ShowDialog();
            }));
            thread.SetApartmentState(ApartmentState.STA);
            //thread.IsBackground = true;
            thread.Start();
        }

        System.Drawing.Size sketchSize;
        ScrollContentPresenter Presenter => FindVisualChildren<ScrollContentPresenter>(scrollViewer).Single();
        System.Drawing.Point offset;
        void ChangeOffset(int dx, int dy) {
            offset.X += dx;
            //if(offset.X < 0) offset.X = 0;
            offset.X = (int)Math.Max(0, Math.Min(sketchSize.Width - Presenter.ActualWidth, offset.X));

            offset.Y += dy;
            //if(offset.Y < 0) offset.Y = 0;
            offset.Y = (int)Math.Max(0, Math.Min(sketchSize.Height - Presenter.ActualHeight, offset.Y));

            SetOffset();
        }

        private void SetOffset() {
            form?.SetOffset(offset);
        }

        class ScrollableSketch : IScrollInfo {
            private SketchPresenter owner;

            public ScrollableSketch(SketchPresenter sketchPresenter) {
                this.owner = sketchPresenter;
            }

            public bool CanHorizontallyScroll { get; set; }
            public bool CanVerticallyScroll { get; set; }

            double IScrollInfo.ExtentHeight => owner.sketchSize.Height;

            double IScrollInfo.ExtentWidth => owner.sketchSize.Width;

            double IScrollInfo.HorizontalOffset => owner.offset.X;

            public ScrollViewer ScrollOwner { get; set; } = null!;

            double IScrollInfo.VerticalOffset => owner.offset.Y;

            double IScrollInfo.ViewportHeight => owner.Presenter.ActualHeight;

            double IScrollInfo.ViewportWidth => owner.Presenter.ActualWidth;

            const int lineDelta = 10;
            const int wheelDelta = 4;
            void IScrollInfo.LineDown() {
                owner.ChangeOffset(0, lineDelta);
            }

            void IScrollInfo.LineLeft() {
                owner.ChangeOffset(-lineDelta, 0);
            }

            void IScrollInfo.LineRight() {
                owner.ChangeOffset(lineDelta, 0);
            }

            void IScrollInfo.LineUp() {
                owner.ChangeOffset(0, -lineDelta);
            }

            Rect IScrollInfo.MakeVisible(Visual visual, Rect rectangle) {
                return Rect.Empty;
            }

            void IScrollInfo.MouseWheelDown() {
                owner.ChangeOffset(0, wheelDelta);
                owner.scrollViewer.InvalidateScrollInfo();
            }

            void IScrollInfo.MouseWheelLeft() {
                owner.ChangeOffset(-wheelDelta, 0);
                owner.scrollViewer.InvalidateScrollInfo();
            }

            void IScrollInfo.MouseWheelRight() {
                owner.ChangeOffset(wheelDelta, 0);
                owner.scrollViewer.InvalidateScrollInfo();
            }

            void IScrollInfo.MouseWheelUp() {
                owner.ChangeOffset(0, -wheelDelta);
                owner.scrollViewer.InvalidateScrollInfo();
            }

            void IScrollInfo.PageDown() {
            }

            void IScrollInfo.PageLeft() {
            }

            void IScrollInfo.PageRight() {
            }

            void IScrollInfo.PageUp() {
            }

            void IScrollInfo.SetHorizontalOffset(double offset) {
                owner.offset.X = (int)offset;
                owner.SetOffset();
                //owner.ChangeOffset((int)offset, 0);
            }

            void IScrollInfo.SetVerticalOffset(double offset) {
                owner.offset.Y = (int)offset;
                owner.SetOffset();
                //owner.ChangeOffset(0, (int)offset);
            }
        }
    }

}
