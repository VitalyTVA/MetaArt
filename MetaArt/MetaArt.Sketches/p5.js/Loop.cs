//MetaArt version of https://glitch.com/edit/#!/p5-example
//p5.js demos by @mattdesl https://p5-demos.glitch.me/

class Loop : SketchBase {
    // Set canvas size
    void setup() {
        size(400, 400);
    }

    //// On window resize, update the canvas size
    //void windowResized() {
    //    resizeCanvas(windowWidth, windowHeight); //TODO auto size
    //}


    // Render loop that draws shapes
    void draw() {
        // Black background
        background(0);

        // Center of screen
        var px = width / 2;
        var py = height / 2;

        // We will scale everything to the minimum edge of the canvas
        var minDim = min(width, height);

        // Size is a fraction of the screen
        var size = minDim * 0.5f;

        // Get time in seconds
        var time = millis() / 1000f;

        // How long we want the loop to be (of one full cycle)
        var duration = 5;

        // Get a 'playhead' from 0..1
        // We use modulo to keep it within 0..1
        var playhead = time / duration % 1;

        // Get an animated value from 0..1
        // We use playhead * 2PI to get a full rotation
        var anim = sin(playhead * PI * 2) * 0.5f + 0.5f;

        // Create an animated thickness for the stroke that
        var thickness = minDim * 0.1f * anim;

        // Turn off fill
        noFill();

        // Turn on stroke and make it white
        stroke(White);

        // Apply thickness
        strokeWeight(thickness);

        // Draw a circle centred at (px, py)
        ellipse(px, py, size, size);
    }
}

