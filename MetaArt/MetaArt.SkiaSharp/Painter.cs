using MetaArt.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MetaArt.Skia {
    public sealed class Painter : PainterBase {
        Queue<Action<SKSurface>> afterRenderQueue = new();


        public void MakeSnapshot(Action<byte[]> saveData) {
            afterRenderQueue.Enqueue(surface => {
                using(SKImage image = surface.Snapshot())
                using(SKData data = image.Encode(SKEncodedImageFormat.Png, 100)) {
                    saveData(data.ToArray());
                }
            });
            invalidate();
        }

        SKImage? draw;
        bool drawn = false;

        public void PaintSurface(SKSurface surface) {
            if(draw != null) {
                surface.Canvas.DrawImage(draw, 0, 0);
                if((NoLoop && drawn) || !HasDraw)
                    return;
            }
            surface.Canvas.Translate(.5f, .5f);
            SKSurface = surface;
            Draw();
            drawn = true;
            if(draw != null)
                draw.Dispose();
            draw = null;
            if(!fullRedraw)
                draw = surface.Snapshot();
            if(!NoLoop)
                invalidate();
        }

        SKSurface? sKSurface;
        SKSurface SKSurface {
            get => sKSurface!; 
            set {
                sKSurface = value;
                ((SkiaGraphics)Graphics).Surface = SKSurface;
            }
        }


        public Painter(Type sketchType, Action invalidate, Action<PaintFeedback> feedback, float displayDensity, DeviceType deviceType) 
            : base(sketchType, new SkiaGraphics(), invalidate, feedback, displayDensity, deviceType) {
        }

        void ClearSurface() {
            sKSurface = null;
        }
        public void Setup() {
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
