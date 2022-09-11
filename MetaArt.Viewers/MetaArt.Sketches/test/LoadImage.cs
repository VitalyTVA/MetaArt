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

        imageMode(CORNER);
        tint(255, 255);
        image(image, 100, 100);

        tint(255, 100);
        image(image, 150, 100);

        imageMode(CENTER);
        tint(255, 255);
        image(image, 200, 100);

    }
}
