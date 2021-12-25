//MetaArt version of https://glitch.com/edit/#!/p5-example-shape
//p5.js demos by @mattdesl https://p5-demos.glitch.me/

class Repeat : SketchBase {
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


    // Render loop that draws shapes
    void draw() {
        // For consistent sizing regardless of portrait/landscape
        var dim = Math.Min(width, height);

        // Black background
        background(0);

        // Stroke only with a specific join style and thickness
        noFill();
        stroke(255);
        strokeWeight(dim * 0.0075f);

        // # of elements we wish to draw
        var count = 5;

        // Margin from edge of screen
        var margin = dim * 0.2f;

        // The size with margin in consideration
        var innerWidth = (width - margin * 2);

        // Compute the diameter of each circle
        var diameter = innerWidth / (count - 1);

        // Draw each circle
        for(var i = 0; i < count; i++) {
            // Get a 0..1 value across our loop
            var t = count <= 1 ? 0.5f : i / (count - 1f);

            // The x position is linearly spaced across the inner width
            var x = lerp(margin, width - margin, t);

            // The y position is centred vertically
            var y = height / 2;

            ellipse(x, y, diameter, diameter);
        }
    }
}

