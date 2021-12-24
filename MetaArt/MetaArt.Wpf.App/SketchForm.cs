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
            ShowInTaskbar = false;

            this.FormBorderStyle = FormBorderStyle.None;

            this.ownerRect = ownerRect;
            painter = new Painter((SketchBase)Activator.CreateInstance(skecthType)!);

            t.Interval = 1000 / 120;
            t.Tick += T_Tick;
            t.Start();

            LocationChanged += SketchForm_LocationChanged;

            Click += SketchForm_Click;
            skglControl1.MouseDown += SkglControl1_MouseDown;
            //skglControl1.Visible = false;
        }

        private void SkglControl1_MouseDown(object? sender, MouseEventArgs e) {
            mouseDown = true;
            skglControl1.ForcePaint();
        }

        private void SketchForm_Click(object? sender, EventArgs e) {
            //throw new NotImplementedException();
        }

        private void SketchForm_LocationChanged(object? sender, EventArgs e) {
        }

        private void T_Tick(object? sender, EventArgs e) {
            //skglControl1.Invalidate();
        }


        bool setUp = false;
        //bool draw = false;
        //Point? mousePos;
        bool mouseDown;
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
                bool over = new Rectangle(Point.Empty, Size).Contains(mouse);
                if(mouseDown) {
                    painter.MousePressed(over ? new System.Windows.Point(mouse.X, mouse.Y) : null);
                    mouseDown = false;
                } else {
                    painter.Draw(over ? new System.Windows.Point(mouse.X, mouse.Y) : null);
                    //draw = true;
                }
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

        //bool isMouseOver = false;
        protected override void OnMouseHover(EventArgs e) {
            base.OnMouseHover(e);
            //isMouseOver = true;
            //skglControl1.ForcePaint();
            //skglControl1.GRContext.
        }
        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            //isMouseOver = false;
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            //skglControl1.ForcePaint();
        }
        public Task Stop() {
            return Task.Factory.FromAsync(BeginInvoke(new Action(() => { Close(); })), _ => { });
        }
    }
    class MyControl : SKGLControl {
        Graphics? g;
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            g = e.Graphics;
        }
        public void ForcePaint() {
            base.OnPaint(new PaintEventArgs(g!, Rectangle.Empty));
        }
    }

}
