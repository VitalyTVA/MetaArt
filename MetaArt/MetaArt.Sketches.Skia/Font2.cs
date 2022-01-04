using MetaArt;
using SkiaSharp;
using System.Diagnostics;

class Font2 {
    SKTypeface typeface = null!;
    SKFont font = null!;

    void setup() {
        size(800, 400);
        var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MetaArt.Sketches.Skia.Assets.SourceCodePro-Regular.ttf");
        typeface = SKTypeface.FromStream(stream);
        font = new SKFont(typeface, 40, 1f, 0f);
    }

    void draw() {
        Canvas.Clear(SKColors.Black);
        var paint = new SKPaint {
            Color = SKColors.White,
            TextSize = 100,
            TextScaleX = 1,
            TextSkewX = 0,
            TextAlign = SKTextAlign.Center,
            IsAntialias = true,
        };
        const string Text = "Test String!!!";
        var textH = (- paint.FontMetrics.Ascent + paint.FontMetrics.Descent);
        SKRect bounds = new SKRect();
        var textW = paint.MeasureText(Text, ref bounds);

        var middle = (bounds.Top + bounds.Bottom) / 2;
        //Canvas.DrawLine(0, height / 2 + bounds.Bottom, width, height / 2 + bounds.Bottom, paint);

        //Canvas.DrawLine(0, height / 2 + bounds.Bottom, width, height / 2 + bounds.Bottom, paint);
        //Canvas.DrawLine(0, height / 2 + bounds.Top, width, height / 2 + bounds.Top, paint);

        //Canvas.DrawLine(0, height / 2 - bounds.Height / 2, width, height / 2 - bounds.Height / 2, paint);
        Canvas.DrawText(Text, width / 2, height / 2 - middle, paint);
    }
}
