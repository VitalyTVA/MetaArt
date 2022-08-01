using MetaArt.Skia;
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
        Painter painter;
        System.Drawing.Rectangle ownerRect;
        readonly Action<System.Drawing.Size> sketchSizeChanged;
        readonly Action<int> mouseWheel;

        public SketchForm(Type sketchType, System.Drawing.Rectangle ownerRect, Action<System.Drawing.Size> sketchSizeChanged, Action<int> mouseWheel, Action<PaintFeedback> feedback) {
            InitializeComponent();
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = System.Drawing.Color.LightBlue;
            this.ownerRect = ownerRect;
            this.sketchSizeChanged = sketchSizeChanged;
            this.mouseWheel = mouseWheel;
            painter = new Painter(sketchType!, skglControl1.Invalidate, feedback, displayDensity: 1, deviceType: DeviceType.Desktop);
            painter.Setup();
            skglControl1.MouseMove += SkglControl1_MouseMove;
            skglControl1.MouseLeave += SkglControl1_MouseLeave;
            skglControl1.KeyDown += SkglControl1_KeyDown;
            skglControl1.KeyPress += SkglControl1_KeyPress;
            skglControl1.MouseDown += SkglControl1_MouseDown;
            skglControl1.MouseUp += SkglControl1_MouseUp;
            skglControl1.MouseWheel += SkglControl1_MouseWheel;
            MouseWheel += SkglControl1_MouseWheel;
            //skglControl1.Visible = false;
            Width = Screen.PrimaryScreen.WorkingArea.Width;
            Height = Screen.PrimaryScreen.WorkingArea.Height;
        }

        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            SetLocation(ownerRect);
        }

        private void SkglControl1_MouseWheel(object? sender, MouseEventArgs e) {
            mouseWheel(-e.Delta);
        }

        private void SkglControl1_KeyPress(object? sender, KeyPressEventArgs e) {
            painter.OnKeyPress(e.KeyChar);
        }

        private void SkglControl1_MouseLeave(object? sender, EventArgs e) {
            painter.OnMouseLeave();
        }

        private void SkglControl1_MouseMove(object? sender, EventArgs e) {
            var mouse = skglControl1.PointToClient(MousePosition);
            painter.OnMouseOver(mouse.X, mouse.Y);
        }

        private void SkglControl1_MouseDown(object? sender, MouseEventArgs e) {
            var mouse = skglControl1.PointToClient(MousePosition);
            painter.OnMouseDown(mouse.X, mouse.Y, e.Button == MouseButtons.Left);
        }
        private void SkglControl1_MouseUp(object? sender, MouseEventArgs e) {
            var mouse = skglControl1.PointToClient(MousePosition);
            painter.OnMouseUp(mouse.X, mouse.Y);
        }

        private void SkglControl1_KeyDown(object? sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.S && e.Modifiers == Keys.Control) {
                painter.MakeSnapshot(data => File.WriteAllBytes("c:\\temp\\1.png", data));
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

        private void skglControl1_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e) {
            painter.PaintSurface(e.Surface);
        }

        public void SetLocation(System.Drawing.Rectangle ownerRect) {
            BeginInvoke(() => {
                Location = ownerRect.Location;
                ClientSize = ownerRect.Size;
                var sketchSize = new System.Drawing.Size(painter.Width, painter.Height);
                skglControl1.Size = sketchSize;
                sketchSizeChanged(sketchSize);
                TopMost = true;
                TopMost = false;
            });
        }
        internal void SetOffset(System.Drawing.Point offset) {
            BeginInvoke(() => {
                skglControl1.Location = new System.Drawing.Point(-offset.X, -offset.Y);
            });
        }

        protected override void OnClosing(CancelEventArgs e) {
            painter.Dispose();
            base.OnClosing(e);
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
