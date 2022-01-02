using MetaArt;
using System.Diagnostics;

namespace TestSketches;
class LoadImage {
    PImage image = null!;
    void setup() {
        size(640, 360);
        image = loadImage("texture.png");
    }

    void draw() {
        background(0);
        image(image, 100, 100);
    }
}
