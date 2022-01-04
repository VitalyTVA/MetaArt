using MetaArt;
using SkiaSharp;
using System.Diagnostics;

class Font {
    String message = "Tickleg";
    float x, y; // X and Y coordinates of text
    float hr;  // horizontal and vertical radius of the text

    SKTypeface typeface = null!;
    void setup() {
        size(640, 360);

        //hr = textWidth(message) / 2;
        //println(textWidth(message));
        //println(textAscent());
        //println(textDescent());
        //vr = (textAscent() + textDescent()) / 2;
        noStroke();
        x = width / 2;
        y = height / 2;
        var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MetaArt.Sketches.Skia.Assets.SourceCodePro-Regular.ttf");
        typeface = SKTypeface.FromStream(stream);
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
            Typeface = typeface,
        };
        SKRect bounds = new SKRect();
        hr = paint.MeasureText(message, ref bounds) / 2;

        fill(204, 120);
        rect(0, 0, width, height);

        fill(0);
        Canvas.DrawText(message, x, y + (- paint.FontMetrics.Ascent) / 2, paint);

        noFill();
        stroke(255);
        strokeWeight(1);
        line(width / 2 - hr, 0, width / 2 - hr, height);
        line(width / 2 + hr, 0, width / 2 + hr, height);
        line(0, height / 2 + 100 / 2 + paint.FontMetrics.Ascent, width, height / 2 + 100 / 2 + paint.FontMetrics.Ascent);
        line(0, height / 2 + 100 / 2 + paint.FontMetrics.Descent, width, height / 2 + 100 / 2 + paint.FontMetrics.Descent);
    }
}
