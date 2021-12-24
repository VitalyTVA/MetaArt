using SkiaSharp;
using System;
using System.Diagnostics;
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
        MethodInfo? mousePressedMethod;

        protected PainterBase(SketchBase sketch) {
            this.sketch = sketch;
            drawMethod = sketch.GetType().GetMethod("draw", BindingFlags.Instance | BindingFlags.NonPublic);
            setupwMethod = sketch.GetType().GetMethod("setup", BindingFlags.Instance | BindingFlags.NonPublic);
            settingsMethod = sketch.GetType().GetMethod("settings", BindingFlags.Instance | BindingFlags.NonPublic);
            mousePressedMethod = sketch.GetType().GetMethod("mousePressed", BindingFlags.Instance | BindingFlags.NonPublic); 
            sketch.Painter = this;

            SettingsCore();
        }
        Stopwatch stopwatch = new();
        internal void StartStopwatch() => stopwatch.Start();
        protected void MousePressedCore(float? mouseX, float? mouseY) {
            sketch.mouseX = mouseX ?? sketch.mouseX;
            sketch.mouseY = mouseY ?? sketch.mouseY;
            mousePressedMethod?.Invoke(sketch, null);
        }
        protected void DrawCore(float? mouseX, float? mouseY) {
            var currentTime = (int)stopwatch.ElapsedMilliseconds;
            sketch.deltaTime = currentTime - sketch.currentTime;
            sketch.currentTime = currentTime;
            sketch.mouseX = mouseX ?? sketch.mouseX;
            sketch.mouseY = mouseY ?? sketch.mouseY;
            drawMethod?.Invoke(sketch, null);
        }
        void SettingsCore() {
            settingsMethod?.Invoke(sketch, null);
        }
        protected void SetupCore() {
            stopwatch.Start();
            setupwMethod?.Invoke(sketch, null);
        }
    }
}
