# MetaArt

# Links
https://github.com/8/SkiaSharp-Wpf-Example

https://github.com/AvaloniaUI/Avalonia/issues/1756
https://github.com/AvaloniaUI/Avalonia/tree/master/samples/RenderDemo

examples
https://github.com/processing/processing-examples/tree/main/Topics

handling bitmap pixels
https://github.com/MicrosoftDocs/xamarin-docs/blob/live/docs/xamarin-forms/user-interface/graphics/skiasharp/bitmaps/pixel-bits.md

https://docs.microsoft.com/en-us/dotnet/api/skiasharp.skshader?view=skiasharp-2.80.2

calc spline https://github.com/PeterWaher/IoTGateway/blob/1cb3631831f223eab7f145ebe3a789325fdb90e5/Script/Waher.Script.Graphs/Functions/Plots/Plot2DCurvePainter.cs

https://github.com/microsoft/referencesource/tree/master/System.Numerics/System/Numerics

void setup()
{
size(600, 600);
}
void draw() {
        var time = millis();
        background(0);
        for(int i = 0; i < 100000; i++) {
            stroke((byte)random(255),
                   (byte)random(255),
                   (byte)random(255),
                   (byte)random(255)
            );
            strokeWeight(random(1, 10));
            line(
                random((int)width),
                random((int)height),
                random((int)width),
                random((int)height)
            );
        }
        println("" + (millis() - time));
}