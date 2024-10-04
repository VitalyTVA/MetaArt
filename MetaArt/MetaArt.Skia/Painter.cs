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


        public Painter(
            object sketch,
            Action invalidate,
            Action<PaintFeedback> feedback,
            float displayDensity,
            DeviceType deviceType,
            Func<Stream, SoundFile> createSoundFile,
            Func<string, string?> getValue,
            Action<string, string?> setValue,
            Func<string, string, string?> saveDialog,
            Func<string, string?> openDialog
        ) : base(
            sketch, 
            new SkiaGraphics(), 
            invalidate, 
            feedback, 
            displayDensity, 
            deviceType, 
            createSoundFile,
            getValue,
            setValue,
            saveDialog,
            openDialog
        ) { }

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

        public List<UIElementInfo> UIElements { get; } = new();
        protected override void uiCommand(Action exectute, string caption, (char key, ModifierKeys modifier)? shortCut) {
            UIElements.Add(new UICommandInfo(exectute, caption, shortCut));
        }
        protected override UICaption uiCaption(string caption) {
            var uiCaption = new UICaption(() => UIElementChanged?.Invoke(this, EventArgs.Empty));
            UIElements.Add(new UICaptionInfo(caption, uiCaption));
            return uiCaption;
        }
        protected override void uiChoice<T>(ChoiceElement<T>[] source, Action<ChoiceElement<T>> changed) {
            UIElements.Add(new UIChoiceInfo(source.Select(x => new ChoiceElement<object>(x.Caption, x.Value!)).ToArray(), x => changed(new ChoiceElement<T>(x.Caption, (T)x.Value))));
        }
        public void OnChoiceChanged(Action changed) {
            preRenderQueue.Enqueue(changed);
            invalidate();
        }


        public event EventHandler? UIElementChanged;

        public override void OnKeyPress(char key, ModifierKeys modifier) {
            var command = UIElements.OfType<UICommandInfo>().FirstOrDefault(x => x.ShortCut == (key, modifier));
            command?.Execute();
            base.OnKeyPress(key, modifier);
        }
    }
    public abstract record UIElementInfo;
    public record UICommandInfo(Action Execute, string Caption, (char key, ModifierKeys modifier)? ShortCut) : UIElementInfo;
    public record UICaptionInfo(string Caption, UICaption uiCaption) : UIElementInfo;
    public record UIChoiceInfo(ChoiceElement<object>[] Source, Action<ChoiceElement<object>> Changed) : UIElementInfo;
}
