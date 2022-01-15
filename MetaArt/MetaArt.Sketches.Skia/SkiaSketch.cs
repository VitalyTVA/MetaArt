using MetaArt;
using MetaArt.Internal;
using MetaArt.Skia;
using SkiaSharp;
public static class SkiaSketch {
    public static SkiaGraphics Graphics => (SkiaGraphics)MetaArt.Internal.Graphics.GraphicsInstance;
    public static SKSurface Surface => Graphics.Surface;
    public static SKCanvas Canvas => Surface.Canvas;
}
