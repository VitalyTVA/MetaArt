using MetaArt;
using SkiaSharp;
using System.Diagnostics;

class ZBufferShader {
    void setup() {
        size(640, 360);
    }

    void draw() {
        fill(White);
        SKCanvas canvas = Canvas;
        // input values
        float threshold = 1.05f;
        float exponent = 1.5f;

        using var zBufferBitmap = new SKBitmap(640, 360);
        var zBufferCanvas = new SKCanvas(zBufferBitmap);

        using(SKPaint gradientPaint = new SKPaint()) {
            SKRect rect = new SKRect(0, 0, width, height);

            // Create linear gradient from upper-left to lower-right
            gradientPaint.Shader = SKShader.CreateLinearGradient(
                                new SKPoint(rect.Left, rect.Top),
                                new SKPoint(rect.Right, rect.Bottom),
                                new SKColor[] { SKColors.Black, SKColors.White},
                                new float[] { 0, 1 },
                                SKShaderTileMode.Repeat);

            // Draw the gradient on the rectangle
            zBufferCanvas.DrawRect(rect, gradientPaint);
        }


        // image source
        using var blueShirt = SKImage.FromEncodedData(Path.Combine(@"c:\Work\github\MetaArt\MetaArt\MetaArt.Sketches.Skia\Assets\", "Blue_Tshirt.jpg"));
        using var textureShader = blueShirt.ToShader();
        using var zBufferShader = SKImage.FromBitmap(zBufferBitmap).ToShader();

        // shader
        var src = @"
uniform shader color_map;
uniform shader zBuffer;
uniform float scale;
uniform half exp;
uniform float3 in_colors0;
half4 main(float2 p) {
    half4 texColor = sample(color_map, p);
    if (length(abs(in_colors0 - pow(texColor.rgb, half3(exp)))) < scale)
        discard;
    half4 zBufferColor = sample(zBuffer, p);
    if (zBufferColor.rgb[0] < .3)
        discard;
    return texColor;
}";
        using var effect = SKRuntimeEffect.Create(src, out var errorText);

        // input values
        var uniforms = new SKRuntimeEffectUniforms(effect) {
            ["scale"] = threshold,
            ["exp"] = exponent,
            ["in_colors0"] = new[] { 1f, 1f, 1f },
        };

        // shader values
        var children = new SKRuntimeEffectChildren(effect) {
            ["color_map"] = textureShader,
            ["zBuffer"] = zBufferShader
        };

        // create actual shader
        using var shader = effect.ToShader(true, uniforms, children);

        // draw as normal
        //canvas.DrawBitmap(zBufferBitmap, new SKPoint());
        using var paint = new SKPaint { Shader = shader };
        canvas.DrawRect(SKRect.Create(width, height), paint);
    }
}
