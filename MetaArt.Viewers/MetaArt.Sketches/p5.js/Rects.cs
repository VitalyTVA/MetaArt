//MetaArt version of https://glitch.com/edit/#!/example-random-rects
//p5.js demos by @mattdesl https://p5-demos.glitch.me/

class Rects {
    List<(Vector, Vector)> rectangles = new();
    // Set canvas size
    void setup() {
        size(400, 400);

        // Disable animation loop since its a static artwork
        noLoop();

        // Create some random rectangles
        const int rectangleCount = 10;
        for(int i = 0; i < rectangleCount; i++) {
            // Randomly place some rectangles within -1..1 space
            const float shrink = 0.5f;
            Vector position = new Vector(random(-1, 1) * shrink, random(-1, 1) * shrink);
            // Create a random 0..1 scale for the rectangles
            float scale = random(0.5f, 1);
            var size = new Vector(random(0, 1) * scale, random(0, 1) * scale);
            rectangles.Add((position, size));
        }
    }

    //// On window resize, update the canvas size
    //void windowResized() {
    //    resizeCanvas(windowWidth, windowHeight); //TODO auto size
    //}


    // Render loop that draws shapes
    void draw() {
        // Black background
        background(Black);

        // Setup drawing style
        strokeJoin(MITER);
        rectMode(CENTER);
        noFill();
        stroke(255);

        // Get the minimum edge
        float minDim = min(width, height);


        // Draw each rect
        foreach(var (position, size) in rectangles) { 

            // The position and size in -1..1 normalized space
            var (x, y) = position;
            var (w, h) = size;

            // Map -1..1 to screen pixels
            // First we 'push' to save transformation state
            push();

            // Then translate to the center
            translate(width / 2, height / 2);

            // And scale the context by half the size of the screen
            // We use a square ratio so that the lines have even thickness
            scale(minDim / 2, minDim / 2);

            // The stroke weight is specified in 0..1 normalized space
            // It will be multiplied by the scale above
            strokeWeight(0.015f);

            // now draw the rectangle
            rect(x, y, w, h);

            // and restore the transform for the next rectangle
            pop();
        }
    }
}

