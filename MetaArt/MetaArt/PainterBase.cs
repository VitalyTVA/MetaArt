using MetaArt.Internal;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MetaArt {
    public abstract class PainterBase {
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
        public Graphics Graphics { get; }

        MethodInfo? drawMethod;
        MethodInfo? setupwMethod;
        MethodInfo? settingsMethod;
        MethodInfo? mousePressedMethod;
        MethodInfo? mouseMovedMethod;

        protected PainterBase(SketchBase sketch, Graphics graphics) {
            this.sketch = sketch;
            Graphics = graphics;
            drawMethod = sketch.GetType().GetMethod("draw", BindingFlags.Instance | BindingFlags.NonPublic);
            setupwMethod = sketch.GetType().GetMethod("setup", BindingFlags.Instance | BindingFlags.NonPublic);
            settingsMethod = sketch.GetType().GetMethod("settings", BindingFlags.Instance | BindingFlags.NonPublic);
            mousePressedMethod = sketch.GetType().GetMethod("mousePressed", BindingFlags.Instance | BindingFlags.NonPublic);
            mouseMovedMethod = sketch.GetType().GetMethod("mouseMoved", BindingFlags.Instance | BindingFlags.NonPublic); 
            sketch.Painter = this;

            SettingsCore();
        }
        Stopwatch stopwatch = new();
        internal void StartStopwatch() => stopwatch.Start();
        protected void MousePressedCore(float mouseX, float mouseY) {
            if(mousePressedMethod == null)
                return;
            SetMouse(mouseX, mouseY);
            mousePressedMethod.Invoke(sketch, null);
        }
        protected void MouseMovedCore(float mouseX, float mouseY) {
            if(mouseMovedMethod == null)
                return;
            SetMouse(mouseX, mouseY);
            mouseMovedMethod.Invoke(sketch, null);
        }

        private void SetMouse(float? mouseX, float? mouseY) {
            sketch.pmouseX = sketch.mouseX;
            sketch.pmouseY = sketch.mouseY;
            sketch.mouseX = mouseX ?? sketch.mouseX;
            sketch.mouseY = mouseY ?? sketch.mouseY;
        }

        protected void DrawCore(float? mouseX, float? mouseY) {
            var currentTime = (int)stopwatch.ElapsedMilliseconds;
            sketch.deltaTime = currentTime - sketch.currentTime;
            sketch.currentTime = currentTime;
            SetMouse(mouseX, mouseY);
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
