//MetaArt version of https://glitch.com/edit/#!/p5-example-circle
//p5.js demos by @mattdesl https://p5-demos.glitch.me/

/// <summary>A simple circle.</summary>
class Circle : SketchBase {
    // Set canvas size
    void setup() {
        size(400, 400);
    }
    // Render loop that draws shapes with
    void draw() {
        // Fill in the background
        background(Black);

        // Get center of page
        var x = width / 2;
        var y = height / 2;

        // Find smallest dimension and scale it down
        var diameter = min(width, height) * 0.5f;

        // Set drawing style
        fill(White);
        noStroke();

        // Draw a circle
        circle(x, y, diameter);
    }
}

