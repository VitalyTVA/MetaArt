using MetaArt.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MetaArt.Skia {
    public sealed class Painter : PainterBase {
        Queue<Action> preRenderQueue = new();
        Queue<Action<SKSurface>> afterRenderQueue = new();

        Point? pos;
        public void OnMouseDown(float x, float y) {
            preRenderQueue.Enqueue(() => {
                MousePressed(new Point(x, y));
            });
            invalidate();
        }
        public void OnMouseOver(float x, float y) {
            pos = new Point(x, y);
            preRenderQueue.Enqueue(() => {
                MouseMoved(new Point(x, y));
            });
            invalidate();
        }
        public void OnMouseLeave() {
            pos = null;
            invalidate();
        }
        public void MakeSnapshot(Action<byte[]> saveData) {
            afterRenderQueue.Enqueue(surface => {
                using(SKImage image = surface.Snapshot())
                using(SKData data = image.Encode(SKEncodedImageFormat.Png, 100)) {
                    saveData(data.ToArray());
                }
            });
            invalidate();
        }

        bool setUp = false;
        SKImage? draw;
        bool drawn = false;

        public void PaintSurface(SKSurface surface) {
            if(draw != null) {
                surface.Canvas.DrawImage(draw, 0, 0);
                if((NoLoop && drawn) || !HasDraw)
                    return;
            }
            SKSurface = surface;
            if(!setUp) {
                Setup();
                if(draw != null)
                    draw.Dispose();
                draw = surface.Snapshot();
                setSize(new Size(Width, Height));
                setUp = true;
                invalidate();
            } else {
                Draw();
                drawn = true;
                if(draw != null)
                    draw.Dispose();
                draw = surface.Snapshot();
                if(!NoLoop)
                    invalidate();
            }
        }

        SKSurface? sKSurface;
        SKSurface SKSurface {
            get => sKSurface!; 
            set {
                sKSurface = value;
                ((SkiaGraphics)Graphics).Canvas = SKSurface.Canvas;
            }
        }

        readonly Action invalidate;
        readonly Action<Size> setSize;

        public Painter(Type sketchType, Action invalidate, Action<Size> setSize) 
            : base((SketchBase)Activator.CreateInstance(sketchType), new SkiaGraphics()) {
            this.invalidate = invalidate;
            this.setSize = setSize;
        }

        void ClearSurface() {
            sKSurface = null;
        }
        void MousePressed(Point mouse) {
            MousePressedCore((float)mouse.X, (float)mouse.Y);
            //ClearSurface();
        }
        void MouseMoved(Point mouse) {
            MouseMovedCore((float)mouse.X, (float)mouse.Y);
            //ClearSurface();
        }
        void Setup() {
            SetupCore();
            ClearSurface();
        }
        void Draw() {
            while(preRenderQueue.Count > 0) {
                preRenderQueue.Dequeue().Invoke();
            }
            DrawCore(pos?.X, pos?.Y);
            while(afterRenderQueue.Count > 0) {
                afterRenderQueue.Dequeue().Invoke(SKSurface);
            }
            ClearSurface();
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if(disposing) { 
                draw?.Dispose();
                draw = null;
            }
        }
    }
}
