//MetaArt version of https://glitch.com/edit/#!/p5-example-shape
//p5.js demos by @mattdesl https://p5-demos.glitch.me/

/// <summary>Using transform and blend mode.</summary>
class TransformAndBlend : SketchBase {
    // Set canvas size
    void setup() {
        size(400, 400);

        // Disable animation loop since its a static artwork
        noLoop();
    }

    //// On window resize, update the canvas size
    //void windowResized() {
    //    resizeCanvas(windowWidth, windowHeight); //TODO auto size
    //}


    // Render loop that draws shapes with p5
    void draw() {
        // Set the default blend mode
        blendMode(BLEND);

        // Black background
        background(0);

        // Set foreground as white
        fill(White);

        // Set x-or / difference blend mode
        blendMode(DIFFERENCE);

        // Disable stroke
        noStroke();

        // Center of screen
        var x = width / 2;
        var y = height / 2;

        // Fraction of screen dim
        var dim = min(width, height);
        var size = dim * 0.5f; //TODO "f"

        // Make a rectangle centred on the screen
        rectMode(CENTER);
        rect(x, y, size, size);

        // Create a circle slightly offset down and right
        push();
        translate(size / 4, size / 4);
        ellipse(x, y, size, size);
        pop();

        // Create a triangle slightly offset up and left
        push();
        translate(-size / 4, -size / 4);
        triangle(
          x, y - size / 2,
          x + size / 2, y + size / 2,
          x - size / 2, y + size / 2
        );
        pop();
    }
}

