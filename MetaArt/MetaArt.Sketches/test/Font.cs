using System.Diagnostics;
namespace TestSketches;
class Font {
    String message = "Tickleg";
    float x, y; // X and Y coordinates of text
    float hr;  // horizontal and vertical radius of the text

    void setup() {
        size(640, 360);

        // Create the font
        textFont(createFont("SourceCodePro-Regular.ttf", 100));
        textAlign(TextAlign.CENTER, TextVerticalAlign.CENTER);

        hr = textWidth(message) / 2;
        noStroke();
        x = width / 2;
        y = height / 2;
    }

    void draw() {
        fill(204, 120);
        rect(0, 0, width, height);

        fill(0);
        text(message, x, y);

        noFill();
        stroke(255);
        strokeWeight(1);
        line(width / 2 - hr, 0, width / 2 - hr, height);
        line(width / 2 + hr, 0, width / 2 + hr, height);
        line(0, height / 2 + 100 / 2 - textAscent(), width, height / 2 + 100 / 2 - textAscent());
        line(0, height / 2 + 100 / 2 + textDescent(), width, height / 2 + 100 / 2 + textDescent());
    }
}