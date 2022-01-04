namespace Motion;
class MovingOnCurves {
    /**
     * Moving On Curves. 
     * 
     * In this example, the circles moves along the curve y = x^4.
     * Click the mouse to have it move to a new position.
     */

    float beginX = 20.0f;  // Initial x-coordinate
    float beginY = 10.0f;  // Initial y-coordinate
    float endX = 570.0f;   // Final x-coordinate
    float endY = 320.0f;   // Final y-coordinate
    float distX;          // X-axis distance to move
    float distY;          // Y-axis distance to move
    float exponent = 4f;   // Determines the curve
    float x = 0.0f;        // Current x-coordinate
    float y = 0.0f;        // Current y-coordinate
    float step = 0.01f;    // Size of each step along the path
    float pct = 0.0f;      // Percentage traveled (0.0 to 1.0)

    void setup() {
        size(640, 360);
        noStroke();
        distX = endX - beginX;
        distY = endY - beginY;
    }

    void draw() {
        fill(0, 2);
        rect(0, 0, width, height);
        pct += step;
        if(pct < 1.0) {
            x = beginX + (pct * distX);
            y = beginY + (pow(pct, exponent) * distY);
        }
        fill(255);
        ellipse(x, y, 20, 20);
    }

    void mousePressed() {
        pct = 0.0f;
        beginX = x;
        beginY = y;
        endX = mouseX;
        endY = mouseY;
        distX = endX - beginX;
        distY = endY - beginY;
    }
}
