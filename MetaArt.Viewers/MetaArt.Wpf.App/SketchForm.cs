using MetaArt.Skia;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media;

namespace MetaArt.Wpf {
    public partial class SketchForm : Form {
        Painter painter;
        System.Drawing.Rectangle ownerRect;
        readonly Action<System.Drawing.Size, int> sketchSizeChanged;
        readonly Action<int> mouseWheel;

        public SketchForm(Type sketchType, object[]? parameters, System.Drawing.Rectangle ownerRect, Action<System.Drawing.Size, int> sketchSizeChanged, Action<int> mouseWheel, Action<PaintFeedback> feedback) {
            InitializeComponent();
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = System.Drawing.Color.LightBlue;
            this.ownerRect = ownerRect;
            this.sketchSizeChanged = sketchSizeChanged;
            this.mouseWheel = mouseWheel;
            painter = new Painter(
                SketchDisplayInfo.CreateSketch(sketchType!, parameters),
                () => skglControl1.Invalidate(), 
                feedback, displayDensity: 1, 
                deviceType: DeviceType.Desktop,
                createSoundFile: CreateSoundFile,
                getValue: name => {
                    return Settings.Default.SketchValues?.FirstOrDefault(x => x.Key == name)?.Value;
                },
                setValue: (name, value) => {
                    if(Settings.Default.SketchValues == null)
                        Settings.Default.SketchValues = new List<KeyValue>();
                    var key = Settings.Default.SketchValues.FirstOrDefault(x => x.Key == name);
                    if(key == null)
                        Settings.Default.SketchValues.Add(new KeyValue { Key = name, Value = value });
                    else
                        key.Value = value;
                    Settings.Default.Save();
                }
            );
            painter.Setup();
            skglControl1.Dock = DockStyle.None;
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

            ctrlHost = new ElementHost();
            ctrlHost.Dock = DockStyle.Top;
            ctrlHost.AutoSize = true;
            ctrlHost.Child = new SketchUIControl(painter);
            Controls.Add(ctrlHost);
        }

        ElementHost ctrlHost;

        class WpfSoundFile : SoundFile {
            readonly SimpleAudioPlayerImplementation player;

            public WpfSoundFile(SimpleAudioPlayerImplementation player) {
                this.player = player;
            }
            public override void play() {
                player.Play();
            }
        }
        static SoundFile CreateSoundFile(Stream stream) {
            //System.Media.SoundPlayer player = new System.Media.SoundPlayer(stream);
            //player.Load();
            //return new WpfSoundFile(player);

            var player = new SimpleAudioPlayerImplementation();
            player.Load(stream);
            return new WpfSoundFile(player);
        }
        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            SetLocation(ownerRect);
        }

        private void SkglControl1_MouseWheel(object? sender, MouseEventArgs e) {
            mouseWheel(-e.Delta);
        }

        private void SkglControl1_KeyPress(object? sender, KeyPressEventArgs e) {
            var modifier = MetaArt.ModifierKeys.None;
            var actualModifier = ModifierKeys;
            var key = e.KeyChar;
            if((actualModifier & Keys.Control) > 0) {
                modifier |= MetaArt.ModifierKeys.Ctrl;
                key = (char)(key + ((char)96));
            }
            if((actualModifier & Keys.Shift) > 0) {
                modifier |= MetaArt.ModifierKeys.Shift;
            }
            painter.OnKeyPress(key, modifier);
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
                ClientSize = ownerRect.Size;// + new Size(0, ctrlHost.Height);
                var sketchSize = new System.Drawing.Size(painter.Width, painter.Height);
                skglControl1.Size = sketchSize;
                sketchSizeChanged(sketchSize, ctrlHost.Height);
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
    }
}
