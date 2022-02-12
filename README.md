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

newell algorithm
http://compgraph.tpu.ru/priority.htm#:~:text=%D0%90%D0%BB%D0%B3%D0%BE%D1%80%D0%B8%D1%82%D0%BC%D1%8B%2C%20%D0%B8%D1%81%D0%BF%D0%BE%D0%BB%D1%8C%D0%B7%D1%83%D1%8E%D1%89%D0%B8%D0%B5%20%D1%81%D0%BF%D0%B8%D1%81%D0%BE%D0%BA%20%D0%BF%D1%80%D0%B8%D0%BE%D1%80%D0%B8%D1%82%D0%B5%D1%82%D0%BE%D0%B2%2C%20%D0%BF%D1%8B%D1%82%D0%B0%D1%8E%D1%82%D1%81%D1%8F,%D0%BD%D0%B0%20%D1%80%D0%B0%D1%81%D1%81%D1%82%D0%BE%D1%8F%D0%BD%D0%B8%D0%B8%20%D0%BE%D1%82%20%D1%82%D0%BE%D1%87%D0%BA%D0%B8%20%D0%BD%D0%B0%D0%B1%D0%BB%D1%8E%D0%B4%D0%B5%D0%BD%D0%B8%D1%8F.

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