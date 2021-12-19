using SkiaSharp;
using System;
using System.Linq;

namespace MetaArt {
    public abstract class PainterBase {
        public abstract SKCanvas Canvas { get; }
        public abstract void SetSize(int width, int height);

        public readonly SketchBase sketch;

        protected PainterBase(SketchBase sketch) {
            this.sketch = sketch;
            sketch.Painter = this;
        }
    }
}
