using MetaArt;
using SkiaSharp;
public abstract class SkiaSketchBase : SketchBase {
    protected SKSurface Surface => ((MetaArt.Skia.SkiaGraphics)Graphics).Surface;
    protected SKCanvas Canvas => Surface.Canvas;
}
