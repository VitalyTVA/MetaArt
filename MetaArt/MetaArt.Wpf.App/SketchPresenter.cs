using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MetaArt.Wpf {
    public class SketchPresenter : Border {
        public SketchPresenter() {
        }
        public async Task Stop() {
            if(form == null)
                return;
            await form.Stop();
            form = null;
        }
        public void UpdateLocation() {
            form?.SetLocation(GetLocation());
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
            base.OnRenderSizeChanged(sizeInfo);
            //UpdateLocation();
        }

        private System.Drawing.Point GetLocation() {
            var p = PointToScreen(new System.Windows.Point(0, 0));
            return new System.Drawing.Point((int)p.X, (int)p.Y);
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

        SketchForm? form = null;
        public async void Run(Type skecthType) {
            await Stop();

            var location = GetLocation();

            var thread = new Thread(new ThreadStart(() => {
                form = new SketchForm(skecthType, location);
                form.ShowDialog();
            }));
            thread.SetApartmentState(ApartmentState.STA);
            //thread.IsBackground = true;
            thread.Start();
        }

        //internal void BringToFront() {
        //    form?.BeginInvoke(() => form?.BringToFront());
        //}
    }
}
