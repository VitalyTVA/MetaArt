using MetaArt;
using SkiaSharp;
using System.Diagnostics;

class Shader {
    void setup() {
        size(640, 360);
    }

    void draw() {
        Canvas.Clear(SKColors.White);

        // create the shader
        var shader = SKShader.CreatePerlinNoiseFractalNoise(0.5f, 0.5f, 4, 0);

        // use the shader
        var paint = new SKPaint {
            //Shader = SKShader.CreateColorFilter(shader, SKColorFilter.CreateBlendMode(SKColors.Black, SKBlendMode.Modulate)),
            //Shader = SKShader.CreateColorFilter(shader, SKColorFilter.CreateBlendMode(SKColors.Black, SKBlendMode.Xor)),
            Shader = SKShader.CreateColorFilter(shader, SKColorFilter.CreateBlendMode(SKColors.Red, SKBlendMode.Xor)),


            //Shader = shader,
            Color = SKColors.Black,
            StrokeWidth = 25,
            Style = SKPaintStyle.Stroke
        };
        Canvas.DrawRect(new SKRect(100, 100, 300, 300), paint);
    }
}
