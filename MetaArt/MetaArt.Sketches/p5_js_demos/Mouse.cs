//Simple interpolation with mouse movement.
//MetaArt version of https://glitch.com/edit/#!/p5-example-mouse
//p5.js demos by @mattdesl https://p5-demos.glitch.me/

class Mouse : SketchBase {
    float px;
    float py;

    // Set canvas size
    void setup() {
        size(400, 400);

        // Initial object position
        px = width / 2;
        py = height / 2;

        // Also use this as the initial mouse position
        // i.e. before the mouse has interacted with the canvas
        mouseX = width / 2;
        mouseY = height / 2;
    }

    //// On window resize, update the canvas size
    //void windowResized() {
    //    resizeCanvas(windowWidth, windowHeight); //TODO auto size
    //}

    // Render loop that draws shapes
    void draw() {
        // For consistent sizing regardless of portrait/landscape
        var dim = min(width, height);

        // Black background
        background(Black);

        // Get delta time in seconds
        var dt = deltaTime / 1000f;

        // Spring toward mouse position
        var power = 1f;
        px = spring(px, mouseX, power, dt);
        py = spring(py, mouseY, power, dt);

        // Draw the circle
        fill(White);
        var r0 = dim * 0.1f;
        ellipse(px, py, r0, r0);

        // Draw the mouse as a circle
        noFill();
        stroke(White);
        strokeWeight(dim * 0.015f);
        var r1 = r0 * 2;
        ellipse(mouseX, mouseY, r1, r1);
    }
    // Springs A toward B with a power, accepting deltaTime
    float spring(float a, float b, float power, float dt) {
        return lerp(a, b, 1 - exp(-power * dt));
    }
}

