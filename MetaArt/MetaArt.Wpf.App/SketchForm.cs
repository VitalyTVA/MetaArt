using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetaArt.Wpf {
    public partial class SketchForm : Form {
        Timer t = new Timer();
        Painter painter;
        Point ownerLocation;
        public SketchForm(Type skecthType, Point ownerLocation) {
            InitializeComponent();

            //TopMost = true;
            ShowInTaskbar = false;

            this.FormBorderStyle = FormBorderStyle.None;

            this.ownerLocation = ownerLocation;
            painter = new Painter((SketchBase)Activator.CreateInstance(skecthType)!);

            t.Interval = 1000 / 120;
            t.Tick += T_Tick;
            t.Start();

            LocationChanged += SketchForm_LocationChanged;

            Click += SketchForm_Click;
            skglControl1.MouseDown += SkglControl1_MouseDown;
            skglControl1.KeyDown += SkglControl1_KeyDown;
            //skglControl1.Visible = false;
            Width = Screen.PrimaryScreen.WorkingArea.Width;
            Height = Screen.PrimaryScreen.WorkingArea.Height;
        }

        private void SkglControl1_KeyDown(object? sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.S) {
                save = true;
            }
        }

        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                //hide from alt+tab
                // turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle |= 0x80;
                return cp;
            }
        }

        private void SkglControl1_MouseDown(object? sender, MouseEventArgs e) {
            mouseDown = true;
            skglControl1.Invalidate();
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
        SKImage? draw;
        //Point? mousePos;
        bool mouseDown;
        bool save;
        private void skglControl1_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e) {
            if(draw != null) {
                e.Surface.Canvas.DrawImage(draw, 0, 0);
                return;
            }
            painter.SKSurface = e.Surface;
            if(!setUp) {
                painter.Setup();
                SetLocation(ownerLocation);
                setUp = true;
                skglControl1.Invalidate();
            } else {
                var mouse = skglControl1.PointToClient(MousePosition);
                bool over = new Rectangle(Point.Empty, skglControl1.Size).Contains(mouse);
                if(mouseDown) {
                    painter.MousePressed(over ? new System.Windows.Point(mouse.X, mouse.Y) : null);
                    mouseDown = false;
                }
                //else {
                    painter.Draw(over ? new System.Windows.Point(mouse.X, mouse.Y) : null);
                    if(painter.NoLoop)
                        draw = e.Surface.Snapshot();
                //}
                if(save) {
                    using(SKImage image = e.Surface.Snapshot())
                    using(SKData data = image.Encode(SKEncodedImageFormat.Png, 100)) {
                        File.WriteAllBytes("c:\\temp\\1.png", data.ToArray());
                    }
                    //using(System.IO.MemoryStream mStream = new System.IO.MemoryStream(data.ToArray())) {
                    //    var bmp = new Bitmap(mStream, false);
                    //    bmp.SetResolution(300, 300);
                    //    bmp.Save("c:\\temp\\1.png", ImageFormat.Png);
                    //}

                    save = false;
                }
                if(!painter.NoLoop)
                    skglControl1.Invalidate();
            }
        }

        public void SetLocation(Point p) {
            BeginInvoke(() => {
                Location = p; 
                ClientSize = new Size(painter.Width, painter.Height);
                TopMost = true;
                TopMost = false;
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
