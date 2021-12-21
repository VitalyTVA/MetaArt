//MetaArt version of https://glitch.com/edit/#!/p5-example-loop
//p5.js demos by @mattdesl https://p5-demos.glitch.me/

/// <summary>Rotating shapes around their center.</summary>
class Rotate : SketchBase {
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
        background(Black);

        // Use the minimum screen size for relative rendering
        var dim = Math.Min(width, height);

        // Set up stroke and disable fill
        strokeJoin(ROUND);
        strokeWeight(dim * 0.015f);
        stroke(White);
        noFill();

        // Get time in seconds
        var time = millis() / 1000f;

        // How long we want the loop to be (of one full rotation)
        var duration = 7;

        // Get a 'playhead' from 0..1
        // We use modulo to keep it within 0..1
        var playhead = time / duration % 1;

        // Get the rotation of a full circle
        var rotation = playhead * PI * 2;

        // Center of screen
        var x = width / 2;
        var y = height / 2;

        // Size of rectangle, relative to screen size
        var size = dim * 0.5f;

        // Save transforms
        push();

        // To rotate around the center,
        // we have to first translate to center
        translate(x, y);

        // Then we can rotate around the center
        rotate(rotation);

        // Now we can draw at (0, 0) because we are still translated
        rectMode(CENTER);
        rect(0, 0, size, size);

        // Restore transforms
        pop();
    }
}

