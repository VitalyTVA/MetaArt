using MetaArt;
using SkiaSharp;
using System.Diagnostics;

class RuntimeEffect {
    void setup() {
        size(640, 360);
    }

    void draw() {
        SKCanvas canvas = Canvas;
        // input values
        float threshold = 1.05f;
        float exponent = 1.5f;

        // image source
        using var blueShirt = SKImage.FromEncodedData(Path.Combine(@"c:\Work\github\MetaArt\MetaArt\MetaArt.Sketches.Skia\Assets\", "Blue_Tshirt.jpg"));
        using var textureShader = blueShirt.ToShader();

        // shader
        var src = @"
uniform shader color_map;
uniform float scale;
uniform half exp;
uniform float3 in_colors0;
half4 main(float2 p) {
    half4 texColor = sample(color_map, p);
    if (length(abs(in_colors0 - pow(texColor.rgb, half3(exp)))) < scale)
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
            ["color_map"] = textureShader
        };

        // create actual shader
        using var shader = effect.ToShader(true, uniforms, children);

        // draw as normal
        canvas.Clear(SKColors.Black);
        using var paint = new SKPaint { Shader = shader };
        canvas.DrawRect(SKRect.Create(400, 400), paint);
    }
}
