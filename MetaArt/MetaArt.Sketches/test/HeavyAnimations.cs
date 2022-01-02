using System.Threading;
namespace TestSketches;
class HeavyAnimations {
    void settings() {
        size(400, 400);
    }

    void setup() {
        background(150);
    }


    void draw() {
        Thread.Sleep(500);
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

