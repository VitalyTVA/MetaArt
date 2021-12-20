//MetaArt version of https://glitch.com/edit/#!/p5-example-shape
//p5.js demos by @mattdesl https://p5-demos.glitch.me/

class Shape : SketchBase {
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
        // Black background
        background(0);

        // Turn off shape filling
        noFill();

        // Set the 'join style' of lines to be round
        strokeJoin(ROUND);

        // Set the stroke color as white
        stroke(White);

        // Get the minimum edge of the canvas
        var dim = min(width, height);

        // And use that edge to make the stroke thickness relative
        strokeWeight(dim * 0.015f); //TODO avoid using "f"?

        // Center of screen
        var x = width / 2;
        var y = height / 2;

        // Fraction of screen dim
        var size = dim * 0.5f; //TODO avoid using "f"?

        // Make a rectangle centred on the screen
        rectMode(CENTER);
        rect(x, y, size, size);

        // Create a circle slightly offset down and right
        ellipse(x, y, size, size);

        // Create a triangle slightly offset up and left
        triangle(
          x,
          y - size / 2,
          x + size / 2,
          y + size / 2,
          x - size / 2,
          y + size / 2
        );
    }
}

