namespace Fractals;
class KochCurve {
    /**
     * Koch Curve
     * by Daniel Shiffman.
     * 
     * Renders a simple fractal, the Koch snowflake. 
     * Each recursive level is drawn in sequence. 
     */

    KochFractal k = null!;

    void setup() {
        size(640, 360);
        stroke(White);
        frameRate(1);  // Animate slowly
        k = new KochFractal(width, height);
    }

    void draw() {
        background(0);

        // Draws the snowflake!
        k.Draw();

        // Iterate
        k.NextLevel();
        // Let's not do it more than 5 times. . .
        if(k.Count > 5) {
            k.Restart();
        }
    }
}
