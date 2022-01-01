//MetaArt version of https://glitch.com/edit/#!/p5-example-lewitt
//p5.js demos by @mattdesl https://p5-demos.glitch.me/

// Recreating Sol LeWitt Wall Drawing #130
// "Grid and arcs from four corners."

class Lewitt {
    // Set canvas size
    void setup() {
        size(600, 600);

        // Disable animation loop since its a static artwork
        noLoop();
    }

    //// On window resize, update the canvas size
    //void windowResized() {
    //    resizeCanvas(windowWidth, windowHeight); //TODO auto size
    //}


    // Render loop that draws shapes
    void draw() {
        // Black background
        background(Black);

        // We will use some relative units
        float minRadius = min(width, height);
        float maxRadius = max(width, height);

        // Choose a line thickness for all lines
        float thickness = minRadius * 0.005f;

        // Choose a spacing for the grid
        float spacing = minRadius * 0.125f;

        // Compute the # of lines needed to fill the space
        float lineCount = floor(maxRadius / (thickness + spacing));

        // We can compute the size of each square in the grid like so
        float squareSize = width / (lineCount - 1);

        strokeWeight(thickness);
        stroke(White);
        noFill();

        // Draw the grid first
        for(int i = 0; i < lineCount; i++) {
            // Get a t value to map the value from one range to another
            float t = lineCount <= 1 ? 0.5f : i / (lineCount - 1);

            // Map it to pixels but keep the line thickness in mind
            // so that it fits entirely within the canvas
            float y = lerp(thickness / 2, height - thickness / 2, t);
            float x = lerp(thickness / 2, width - thickness / 2, t);
            line(0, y, width, y);
            line(x, 0, x, height);
        }

        // Which corners to use for arcs
        (float, float)[] corners = new[] {
            // Top left
            (0f, 0f),
            // Bottom right
            (width, height),
            // Top right
            (width, 0f),
            // Bottom left
            (0f, height),
            // Can also emit arcs from center
            // This is not in Sol LeWitt's instructions,
            // but it does appear on the final wall drawing
            (width / 2, height / 2)
        };

        // Draw arcs from each corner
        foreach(var (cx, cy) in corners) {
            // Draw the arcs
            for(int i = 0; i < lineCount; i++) {
                // We can choose how many rings will fit in each square 
                int ringsPerSquare = 2;

                // Get the radius of each circle, making
                // sure to avoid (i=0) as it would not draw anything
                float r = (i + 1) * (squareSize / ringsPerSquare);

                // Scale radius by 2 to get diameter, and draw circle
                circle(cx, cy, r * 2);
            }
        }
    }
}

