class MousePressed : SketchBase {
    void setup() {
        size(200, 200);
        background(150);
    }

    void draw() {
        // This is a trick to create a motion blur,
        // Instead of clearing each frame with pure black,
        // we use black with (N/255)% opacity
        background(0, 0, 0, 5);

        // Use the minimum screen size for relative rendering
        var dim = Math.Min(width, height);

        // Disable fill and set up a stroke
        noFill();
        strokeWeight(dim * 0.015f);
        stroke(White);

        // The equation for an arc is like so:
        // (x, y) + (sin(angle), cos(angle)) * radius

        // Get time in seconds
        var time = millis() / 1000f;

        // How fast we will spin around
        var speed = 0.25f;

        // Scale by 2PI, i.e. a full arc/circle
        var angle = time * PI * 2.0f * speed;

        // The center of the screen
        var cx = width / 2;
        var cy = height / 2;

        // Get the XY position on a unit arc using trigonometry
        var u = cos(angle);
        var v = sin(angle);

        // Choose the size of the arc we will draw
        var radius = dim * 0.25f;

        // Get the final position
        var px = cx + u * radius;
        var py = cy + v * radius;

        // This is the radius for the actual shape/ellipse we will draw
        var r = dim * 0.1f;

        // Finally draw the circle
        ellipse(px, py, r, r);

    }

    void mousePressed() {
        fill(Black);
        circle(mouseX, mouseY, 10);
    }
}
