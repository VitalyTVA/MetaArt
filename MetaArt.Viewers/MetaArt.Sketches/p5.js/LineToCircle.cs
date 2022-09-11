//MetaArt version of https://glitch.com/edit/#!/p5-example-line-circle
//p5.js demos by @mattdesl https://p5-demos.glitch.me/

class LineToCircle {
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
        background(Black);

        // Stroke only with a specific join style and thickness
        noFill();
        stroke(White);
        strokeJoin(BEVEL);
        strokeWeight(dim * 0.015f);

        // Get time in seconds
        float time = millis() / 1000f;

        // Get a value that ping-pongs from 0 ... 1
        float pingPong = sin(time * 0.75f - PI / 2) * 0.5f + 0.5f;

        // Get a number of points, using pow() to skew to one end
        float points = lerp(2, 100, pow(pingPong, 2.5f));

        // Size of shape
        float radius = dim / 3;

        // Draw shape with an angle offset
        float angle = pingPong * TWO_PI;
        polygon(width / 2, height / 2, radius, points, angle);
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

