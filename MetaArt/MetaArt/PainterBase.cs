using MetaArt.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MetaArt {
    public record struct PaintFeedback(TimeSpan DrawTime);
    public abstract class PainterBase : IDisposable
    {
        public void SetSize(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        int width = 100;
        int height = 100;
        public int Width { get => width; }
        public int Height { get => height; }
        public int Millis() => (int)stopwatch.ElapsedMilliseconds;

        public readonly object sketch;
        public Assembly Assembly => sketch.GetType().Assembly;

        private bool noLoop;
        public bool NoLoop
        {
            get => noLoop; set
            {
                if(noLoop == value) return;
                noLoop = value;
                if(!noLoop)
                    invalidate();
            }
        }
        public bool HasDraw => drawMethod != null;
        public Graphics Graphics { get; }
        protected bool fullRedraw = false;
        public void FullRedraw() => fullRedraw = true;

        MethodInfo? drawMethod;
        MethodInfo? setupwMethod;
        MethodInfo? settingsMethod;
        MethodInfo? mousePressedMethod;
        MethodInfo? mouseReleasedMethod;
        MethodInfo? mouseDraggedMethod;
        MethodInfo? mouseMovedMethod;
        MethodInfo? keyPressedMethod;

        internal readonly float displayDensity;

        internal readonly DeviceType deviceType;
        internal readonly Func<Stream, SoundFile> createSoundFile;
        internal readonly Func<string, string?> getValue;
        internal readonly Action<string, string?> setValue;
        internal readonly Func<string, string, string?> saveDialog;
        internal readonly Func<string, string?> openDialog;

        protected PainterBase(
            object sketch,
            Graphics graphics,
            Action invalidate,
            Action<PaintFeedback> feedback,
            float displayDensity,
            DeviceType deviceType,
            Func<Stream, SoundFile> createSoundFile,
            Func<string, string?> getValue,
            Action<string, string?> setValue,
            Func<string, string, string?> saveDialog,
            Func<string, string?> openDialog
        ) {
            this.invalidate = invalidate;
            this.feedback = feedback;
            Graphics = graphics;
            Sketch.Painter = this;
            this.displayDensity = displayDensity;
            this.deviceType = deviceType;

            this.sketch = sketch;
            drawMethod = GetSkecthMethod(sketch.GetType(), "draw");
            setupwMethod = GetSkecthMethod(sketch.GetType(), "setup");
            settingsMethod = GetSkecthMethod(sketch.GetType(), "settings");
            mousePressedMethod = GetSkecthMethod(sketch.GetType(), "mousePressed");
            mouseReleasedMethod = GetSkecthMethod(sketch.GetType(), "mouseReleased");
            mouseDraggedMethod = GetSkecthMethod(sketch.GetType(), "mouseDragged");
            mouseMovedMethod = GetSkecthMethod(sketch.GetType(), "mouseMoved");
            keyPressedMethod = GetSkecthMethod(sketch.GetType(), "keyPressed");

            SettingsCore();

            this.createSoundFile = createSoundFile;
            this.getValue = getValue;
            this.setValue = setValue;
            this.saveDialog = saveDialog;
            this.openDialog = openDialog;
        }

        internal static MethodInfo GetSkecthMethod(Type type, string name)
        {
            while(type != null) {
                var method = type.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
                if(method != null)
                    return method;
                type = type.BaseType;
            }
            return null!;
        }

        protected readonly Action invalidate;
        private readonly Action<PaintFeedback> feedback;
        protected Queue<Action> preRenderQueue = new();
        protected Vector? pos { get; private set; }
        public void OnMouseDown(float x, float y, bool isLeft)
        {
            isMousePressed = true;
            this.isLeft = isLeft;
            preRenderQueue.Enqueue(() =>
            {
                MousePressedCore(x, y);
            });
            invalidate();
        }
        public void OnMouseUp(float x, float y)
        {
            isMousePressed = false;
            preRenderQueue.Enqueue(() =>
            {
                MouseReleasedCore(x, y);
            });
            invalidate();
        }
        public void OnMouseOver(float x, float y)
        {
            pos = new Vector(x, y);
            var pressedValue = isMousePressed;
            preRenderQueue.Enqueue(() =>
            {
                if (pressedValue)
                    MouseDraggedCore(x, y);
                else
                    MouseMovedCore(x, y);
            });
            invalidate();
        }
        public void OnMouseLeave()
        {
            pos = null;
            invalidate();
        }
        public virtual void OnKeyPress(char key, ModifierKeys modifier)
        {
            preRenderQueue.Enqueue(() =>
            {
                KeyPressedCore(key);
            });
            invalidate();
        }

        Stopwatch stopwatch = new();
        private bool disposedValue;

        void MouseMovedCore(float mouseX, float mouseY)
        {
            if (mouseMovedMethod == null)
                return;
            SetMouse(mouseX, mouseY);
            mouseMovedMethod.Invoke(sketch, null);
        }
        void MouseDraggedCore(float mouseX, float mouseY)
        {
            if (mouseDraggedMethod == null)
                return;
            SetMouse(mouseX, mouseY);
            mouseDraggedMethod.Invoke(sketch, null);
        }

        void MousePressedCore(float mouseX, float mouseY)
        {
            if (mousePressedMethod == null)
                return;
            SetMouse(mouseX, mouseY);
            mousePressedMethod.Invoke(sketch, null);
        }
        void MouseReleasedCore(float mouseX, float mouseY)
        {
            if (mouseReleasedMethod == null)
                return;
            SetMouse(mouseX, mouseY);
            mouseReleasedMethod.Invoke(sketch, null);
        }
        protected virtual void KeyPressedCore(char key)
        {
            if (keyPressedMethod == null)
                return;
            this.key = key;
            keyPressedMethod.Invoke(sketch, null);
        }

        private void SetMouse(float? mouseX, float? mouseY)
        {
            pmouseX = this.mouseX;
            pmouseY = this.mouseY;
            this.mouseX = mouseX ?? this.mouseX;
            this.mouseY = mouseY ?? this.mouseY;
        }

        int lastDrawTime = 0;
        public int DeltaTime { get; private set; }
        public int FrameCount { get; private set; }
        protected void DrawCore(float? mouseX, float? mouseY)
        {
            try {
                var currentTime = (int)stopwatch.ElapsedMilliseconds;
                DeltaTime = currentTime - lastDrawTime;
                lastDrawTime = currentTime;
                SetMouse(mouseX, mouseY);
                var drawTicks = stopwatch.ElapsedTicks;
                drawMethod?.Invoke(sketch, null);
                FrameCount++;
                feedback(new PaintFeedback(TimeSpan.FromTicks(stopwatch.ElapsedTicks - drawTicks)));
            } catch(Exception e) {
                var ignore = e;
                throw;
            }
        }
        void SettingsCore()
        {
            settingsMethod?.Invoke(sketch, null);
        }
        protected void SetupCore()
        {
            stopwatch.Start();
            setupwMethod?.Invoke(sketch, null);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Sketch.ClearPainter();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        internal float mouseX;
        internal float mouseY;
        internal float pmouseX;
        internal float pmouseY;
        internal char key { get; set; }
        internal bool isMousePressed { get; set; }
        internal bool isLeftMousePressed => isMousePressed && isLeft;
        internal bool isRightMousePressed => isMousePressed && !isLeft;
        internal bool isLeft { get; set; }

        ColorMode _colorMode = ColorMode.RGB;
        float maxColorValue1 = 255;
        float maxColorValue2 = 255;
        float maxColorValue3 = 255;
        float maxColorValueA = 255;

        internal Color color(float v1, float v2, float v3, float? a = null)
        {
            a = a ?? maxColorValueA;
            return _colorMode switch
            {
                ColorMode.RGB => new Color(
                    (byte)Sketch.map(v1, 0, maxColorValue1, 0, 255),
                    (byte)Sketch.map(v2, 0, maxColorValue2, 0, 255),
                    (byte)Sketch.map(v3, 0, maxColorValue3, 0, 255),
                    (byte)Sketch.map(a.Value, 0, maxColorValueA, 0, 255)
                ),
                ColorMode.HSB => FromHsv(v1, v2, v3, a.Value),
                _ => throw new NotImplementedException(),
            };
        }
        internal Color color(float v, float? a = null)
        {
            a = a ?? maxColorValueA;
            return _colorMode switch
            {
                ColorMode.RGB => new Color(
                    (byte)Sketch.map(v, 0, maxColorValue1, 0, 255),
                    (byte)Sketch.map(v, 0, maxColorValue2, 0, 255),
                    (byte)Sketch.map(v, 0, maxColorValue3, 0, 255),
                    (byte)Sketch.map(a.Value, 0, maxColorValueA, 0, 255)
                ),
                ColorMode.HSB => FromHsv(0, 0, v, a.Value),
                _ => throw new NotImplementedException(),
            };
        }
        Color FromHsv(float h, float s, float v, float a)
        {
            h /= maxColorValue1;
            s /= maxColorValue2;
            v /= maxColorValue3;
            a /= maxColorValueA;
            float red = v;
            float green = v;
            float blue = v;
            if (Math.Abs(s) > 0.001f)
            {
                h *= 6f;
                if (Math.Abs(h - 6f) < 0.001f)
                {
                    h = 0f;
                }
                int num = (int)h;
                float num2 = v * (1f - s);
                float num3 = v * (1f - s * (h - (float)num));
                float num4 = v * (1f - s * (1f - (h - (float)num)));
                switch (num)
                {
                    case 0:
                        red = v;
                        green = num4;
                        blue = num2;
                        break;
                    case 1:
                        red = num3;
                        green = v;
                        blue = num2;
                        break;
                    case 2:
                        red = num2;
                        green = v;
                        blue = num4;
                        break;
                    case 3:
                        red = num2;
                        green = num3;
                        blue = v;
                        break;
                    case 4:
                        red = num4;
                        green = num2;
                        blue = v;
                        break;
                    default:
                        red = v;
                        green = num2;
                        blue = num3;
                        break;
                }
            }
            return new Color((byte)(255 * red), (byte)(255 * green), (byte)(255 * blue), (byte)(255 * a));
        }

        internal void colorMode(ColorMode mode, float max)
        {
            _colorMode = mode;
            maxColorValue1 = max;
            maxColorValue2 = max;
            maxColorValue3 = max;
            maxColorValueA = max;
        }
        internal void colorMode(ColorMode mode, float max1, float max2, float max3)
        {
            _colorMode = mode;
            maxColorValue1 = max1;
            maxColorValue2 = max2;
            maxColorValue3 = max3;
        }

        Random rnd = new();
        internal double NextDouble() => rnd.NextDouble();
        internal void RandomSeed(int seed) => rnd = new(seed);

        Pixels? pixelsContainer;

        internal Color[] pixels => pixelsContainer!.PixelsArray;

        internal void loadPixels()
        {
            if (pixelsContainer != null)
                throw new InvalidOperationException(); //TODO infomative exception
            pixelsContainer = Graphics.loadPixels();
        }
        internal void updatePixels()
        {
            pixelsContainer!.UpdatePixelsAndDispose();
            pixelsContainer = null;
        }

        protected internal abstract void uiCommand(Action exectute, string caption, (char key, ModifierKeys modifier)? shortCut);
        protected internal abstract UICaption uiCaption(string caption);
        protected internal abstract void uiChoice<T>((string caption, T value)[] source, Action<(string caption, T value)> changed);
    }
    public class UICaption {
        public UICaption(Action onChanged) {
            this.onChanged = onChanged;
        }

        string? text;
        readonly Action onChanged;

        public string? Text {
            get => text;
            set {
                if(text == value)
                    return;
                text = value;
                onChanged();
            }
        }
    }
}
