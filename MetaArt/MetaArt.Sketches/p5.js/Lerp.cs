//MetaArt version of https://glitch.com/edit/#!/p5-example-lerp
//p5.js demos by @mattdesl https://p5-demos.glitch.me/

class Lerp {
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
        // For consistent sizing regardless of portrait/landscape
        var dim = Math.Min(width, height);

        // Black background
        background(Black);

        // Stroke only with a specific join style and thickness
        noFill();
        stroke(255);
        strokeWeight(dim * 0.015f);

        // Get time in seconds
        var time = millis() / 1000f;

        // Loop duration
        var duration = 4f;

        // Get a 'playhead' from 0..1
        // We use modulo to keep it within 0..1
        var playhead = time / duration % 1;

        // A "start" position as XY coordinate
        var start = (width * 0.25f, height / 2);

        // An "end" position as XY coordinate
        var end = (width * 0.75f, height / 2);

        // Get a value that goes from -1..1 based on playhead
        // We use 2PI to create a seamless loop
        var t0 = sin(playhead * PI * 2);

        // Now we map the -1..1 to 0..1 values
        t0 = t0 * 0.5f + 0.5f;

        // Now interpolate X and Y positions separately from
        // 'start' to 'end' coordinates
        var x = lerp(start.Item1, end.Item1, t0);
        var y = lerp(start.Item2, end.Item2, t0);

        // As an exercise, you could also try animating the
        // circle radius and stroke weight
        var r = dim * 0.3f;
        ellipse(x, y, r, r);
    }
}

