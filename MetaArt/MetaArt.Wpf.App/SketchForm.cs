using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetaArt.Wpf {
    public partial class SketchForm : Form {
        Timer t = new Timer();
        Painter painter;
        Rectangle ownerRect;
        public SketchForm(Type skecthType, Rectangle ownerRect) {
            InitializeComponent();

            TopMost = true;

            this.FormBorderStyle = FormBorderStyle.None;

            this.ownerRect = ownerRect;
            painter = new Painter((SketchBase)Activator.CreateInstance(skecthType)!);

            t.Interval = 1000 / 120;
            t.Tick += T_Tick;
            t.Start();

            LocationChanged += SketchForm_LocationChanged;
        }

        private void SketchForm_LocationChanged(object? sender, EventArgs e) {
        }

        private void T_Tick(object? sender, EventArgs e) {
            //skglControl1.Invalidate();
        }


        bool setUp = false;
        //bool draw = false;
        private void skglControl1_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e) {
            //if(draw && painter.NoLoop)
            //    return;
            painter.SKSurface = e.Surface;
            if(!setUp) {
                painter.Setup();
                SetLocation(ownerRect);
                setUp = true;
                skglControl1.Invalidate();
            } else {
                var mouse = PointToClient(MousePosition);
                painter.Draw(isMouseOver ? new System.Windows.Point(mouse.X, mouse.Y) : null);
                //draw = true;
                if(!painter.NoLoop)
                    skglControl1.Invalidate();
            }
            

        }

        public void SetLocation(Rectangle rect) {
            BeginInvoke(() => {
                this.Size = new Size(painter.Width, painter.Height);
                this.Location = rect.Location;
            });
        }

        bool isMouseOver = false;
        protected override void OnMouseHover(EventArgs e) {
            base.OnMouseHover(e);
            isMouseOver = true;
            //skglControl1.GRContext.
        }
        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            isMouseOver = false;
        }

        public Task Stop() {
            return Task.Factory.FromAsync(BeginInvoke(new Action(() => { Close(); })), _ => { });
        }
    }
}
