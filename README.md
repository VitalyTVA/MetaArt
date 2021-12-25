# MetaArt

# Links
https://github.com/8/SkiaSharp-Wpf-Example

https://github.com/AvaloniaUI/Avalonia/issues/1756
https://github.com/AvaloniaUI/Avalonia/tree/master/samples/RenderDemo


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