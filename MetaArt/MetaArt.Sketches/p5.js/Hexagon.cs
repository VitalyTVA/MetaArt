//MetaArt version of https://glitch.com/edit/#!/p5-example-hexagon
//p5.js demos by @mattdesl https://p5-demos.glitch.me/

class Hexagon {
    // Set canvas size
    void setup() {
        size(600, 600);
    }

    //// On window resize, update the canvas size
    //void windowResized() {
    //    resizeCanvas(windowWidth, windowHeight); //TODO auto size
    //}

    // Render loop that draws shapes
    void draw() {
        // For consistent sizing regardless of portrait/landscape
        float dim = min(width, height);

        // Black background
        background(0);

        // Stroke only with a specific join style and thickness
        noFill();
        stroke(255);
        strokeJoin(MITER);

        // Get time in seconds
        float time = millis() / 1000f;

        const int rings = 10;
        const int sides = 6;
        float maxRadius = dim * 0.4f;
        for(int i = 0; i < rings; i++) {
            // Get a normalized 't' value that isn't 0
            float t = (i + 1f) / rings;

            // Scale it by max radius
            float radius = t * maxRadius;

            // Min and max line thickness
            float maxThickness = maxRadius / rings * 1;
            float minThickness = maxRadius / rings * 0.001f;

            // Get a value that ping pongs between 0 and 1
            // We offset by t * N to give it some variety
            float pingPong = sin(t * 3 + time) * 0.5f + 0.5f;

            // Compute the actual thickness
            float thickness = lerp(minThickness, maxThickness, pingPong);

            // Draw line
            strokeWeight(thickness);
            polygon(width / 2, height / 2, radius, sides, PI / 2);
        }
    }

    // Draw a basic polygon, handles triangles, squares, pentagons, etc
    void polygon(float x, float y, float radius, float sides = 3, float angle = 0) {
        beginShape();
        for(int i = 0; i < sides; i++) {
            float a = angle + TWO_PI * (i / sides);
            float sx = x + cos(a) * radius;
            float sy = y + sin(a) * radius;
            vertex(sx, sy);
        }
        endShape(CLOSE);
    }
}

