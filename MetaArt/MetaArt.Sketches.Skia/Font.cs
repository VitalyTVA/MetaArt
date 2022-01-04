using MetaArt;
using SkiaSharp;
using System.Diagnostics;

class Font {
    void setup() {
        size(800, 400);
    }

    void draw() {
        Canvas.DrawColor(SKColors.White);
        var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MetaArt.Sketches.Skia.Assets.SourceCodePro-Regular.ttf");
        using var typeface = SKTypeface.FromStream(stream);
        using(SKPaint p = new SKPaint { IsAntialias = true, StrokeWidth = 2, Typeface = typeface, TextSize = 150, TextAlign = SKTextAlign.Center }) {
            float left = 40;
            float midCanvasY = height / 2;
            float zeroHeight = 0;
            const string Text = "Test String!!!";
            for(int i = 0; i < 10; i++) {
                string text = "" + Text[i];
                SKRect textBounds = SKRect.Empty;
                p.Color = SKColors.Black;
                p.Style = SKPaintStyle.Fill;
                p.MeasureText(text, ref textBounds);
                if(i == 0)
                    zeroHeight = textBounds.Height;
                Console.WriteLine("" + i + ": " + textBounds.Height);
                SKPoint textLoc = new SKPoint(left, midCanvasY);
                Canvas.DrawText(text, textLoc.X, textLoc.Y + textBounds.Height / 2, p);
                left += textBounds.Width;
            }
            float topZeroY = midCanvasY - zeroHeight / 2;
            float bottomZeroY = midCanvasY + zeroHeight / 2;
            p.Color = SKColors.Red;
            p.Style = SKPaintStyle.Stroke;
            Canvas.DrawLine(0, topZeroY, width, topZeroY, p);
            Canvas.DrawLine(0, midCanvasY, width, midCanvasY, p);
            Canvas.DrawLine(0, bottomZeroY, width, bottomZeroY, p);
        }
    }
}
