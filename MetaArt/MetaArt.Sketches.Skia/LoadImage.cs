using MetaArt;
using SkiaSharp;
using System.Diagnostics;

class LoadImage {
    void setup() {
        size(640, 360);
    }

    void draw() {
        Canvas.Clear(SKColors.Black);  
        var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MetaArt.Sketches.Skia.Assets.texture.png");
        using var image = SKImage.FromEncodedData(stream);
        Canvas.DrawImage(image, 100, 100, new SKPaint() { Color = SKColors.Black.WithAlpha(70) });
    }
}
