using SkiaSharp;
using System;
using System.Linq;
using System.Reflection;

namespace MetaArt {
    public abstract class PainterBase {
        public abstract SKCanvas Canvas { get; }
        public virtual void SetSize(int width, int height) {
            this.width = width;
            this.height = height;
        }

        int width = 100;
        int height = 100;
        public int Width { get => width; }
        public int Height { get => height; }


        public readonly SketchBase sketch;

        public bool NoLoop { get; set; }

        MethodInfo? drawMethod;
        MethodInfo? setupwMethod;
        MethodInfo? settingsMethod;

        protected PainterBase(SketchBase sketch) {
            this.sketch = sketch;
            drawMethod = sketch.GetType().GetMethod("draw", BindingFlags.Instance | BindingFlags.NonPublic);
            setupwMethod = sketch.GetType().GetMethod("setup", BindingFlags.Instance | BindingFlags.NonPublic);
            settingsMethod = sketch.GetType().GetMethod("settings", BindingFlags.Instance | BindingFlags.NonPublic);
            sketch.Painter = this;

            SettingsCore();
        }

        protected void DrawCore() { 
            drawMethod?.Invoke(sketch, null);
        }
        void SettingsCore() {
            settingsMethod?.Invoke(sketch, null);
        }
        protected void SetupCore() {
            sketch.StartStopwatch();
            setupwMethod?.Invoke(sketch, null);
        }
    }
}
