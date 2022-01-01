//MetaArt version of https://glitch.com/edit/#!/p5-example-line
//p5.js demos by @mattdesl https://p5-demos.glitch.me/

class Lines {
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
        float dim = max(width, height);

        // Black background
        background(Black);

        // Stroke only with a specific join style and thickness
        noFill();
        stroke(255);
        strokeCap(StrokeCap.ROUND);
        strokeWeight(dim * 0.015f);

        float gridSize = 10;
        float margin = dim * 0.1f;
        float innerWidth = width - margin * 2;
        float cellSize = innerWidth / gridSize;

        float time = millis() / 1000f;

        for(int y = 0; y < gridSize; y++) {
            for(int x = 0; x < gridSize; x++) {
                float u = gridSize <= 1 ? 0.5f : x / (gridSize - 1);
                float v = gridSize <= 1 ? 0.5f : y / (gridSize - 1);

                float px = lerp(margin, width - margin, u);
                float py = lerp(margin, height - margin, v);

                float rotation = sin(time + u * PI * 0.25f) * PI;
                float lineSize = sin(time + v * PI) * 0.5f + 0.5f;
                segment(px, py, cellSize * lineSize, rotation);
            }
        }
    }

    // Draw a line segment centred at the given point
    void segment(float x, float y, float length, float angle = 0) {
        float r = length / 2;
        float u = cos(angle);
        float v = sin(angle);
        line(x - u * r, y - v * r, x + u * r, y + v * r);
    }
}

