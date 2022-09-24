using MetaArt.Skia;
using Plugin.SimpleAudioPlayer;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MetaArt.XamarinForms {
    public partial class SketchView : ContentView {
        SKGLView view;
        public SketchView() {
            this.view = new SKGLView();
            this.Content = view;
            if(Device.RuntimePlatform == Device.iOS) {
                this.view.HasRenderLoop = true;
            }
            this.view.PaintSurface += (o, e) => {
                if(sketch == null) return;
                if(painter == null) {
                    ShowSketch();
                    e.Surface.Canvas.Clear(SKColors.Black);
                    return;
                }

                painter?.PaintSurface(e.Surface);
            };

            this.view.EnableTouchEvents = true;
            this.view.Touch += (o, e) => {
                if(e.ActionType == SkiaSharp.Views.Forms.SKTouchAction.Pressed) {
                    //painter?.OnMouseOver(e.Location.X, e.Location.Y);
                    painter?.OnMouseDown(e.Location.X, e.Location.Y, isLeft: false);
                }
                if(e.ActionType == SkiaSharp.Views.Forms.SKTouchAction.Moved) painter?.OnMouseOver(e.Location.X, e.Location.Y);
                if(e.ActionType == SkiaSharp.Views.Forms.SKTouchAction.Released) {
                    painter?.OnMouseUp(e.Location.X, e.Location.Y);
                    painter?.OnMouseLeave();
                }
                e.Handled = true;

            };

        }
        public event EventHandler<PaintFeedback>? Feedback;
        object? sketch;
        Painter? painter;
        public void SetSketch(object sketch) {
            this.sketch = sketch;
            view.InvalidateSurface();
        }

        void ShowSketch() { 
            painter?.Dispose();
            painter = new Painter(
                sketch!,
                () => {
                    if(Device.RuntimePlatform == Device.Android) {
                    //this.view.InvalidateSurface();
                    this.Dispatcher.BeginInvokeOnMainThread(this.view.InvalidateSurface);
                    } else {
                    //this.view.InvalidateSurface();
                }
                },
                feedback => {
                    if(Device.RuntimePlatform == Device.iOS) {
                        Feedback?.Invoke(this, feedback);
                    }
                },
                displayDensity: (float)DeviceDisplay.MainDisplayInfo.Density,
                deviceType: DeviceType.Mobile,
                createSoundFile: CreateSoundFile,
                getValue: name => Preferences.Get("Sketch_" + name, default(string)),
                setValue: (name, value) => Preferences.Set("Sketch_" + name, value)
            );
            painter.SetSize((int)view.CanvasSize.Width, (int)view.CanvasSize.Height);
            painter.Setup();

            this.view.InvalidateSurface();
        }
        public void ClearSkecth() {
            DisposePainter();
            this.sketch = null;
        }
        void DisposePainter() {
            painter?.Dispose();
            painter = null;
        }

        class XamarinSoundFile : SoundFile {
            readonly ISimpleAudioPlayer player;

            public XamarinSoundFile(ISimpleAudioPlayer player) {
                this.player = player;
            }
            public override void play() {
                player.Play();
            }
        }
        static SoundFile CreateSoundFile(Stream stream) {
            ISimpleAudioPlayer player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            player.Load(stream);
            return new XamarinSoundFile(player);
        }
    }
}