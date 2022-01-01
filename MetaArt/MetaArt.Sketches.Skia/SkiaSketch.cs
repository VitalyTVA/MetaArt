using MetaArt;
using MetaArt.Internal;
using SkiaSharp;
public static class SkiaSketch {
    public static SKSurface Surface => ((MetaArt.Skia.SkiaGraphics)Graphics.GraphicsInstance).Surface;
    public static SKCanvas Canvas => Surface.Canvas;
}
