namespace Input;
class Milliseconds : SketchBase {
    /**
     * Milliseconds. 
     * 
     * A millisecond is 1/1000 of a second. 
     * Processing keeps track of the number of milliseconds a program has run.
     * By modifying this number with the modulo(%) operator, 
     * different patterns in time are created.  
     */

    float scaleVal;

    void setup() {
        size(640, 360);
        noStroke();
        scaleVal = width / 20;
    }

    void draw() {
        for(int i = 0; i < scaleVal; i++) {
            colorMode(RGB, (i + 1) * scaleVal * 10);
            var fillVal = millis() % ((i + 1) * scaleVal * 10);
            fill(fillVal);
            rect(i * scaleVal, 0, scaleVal, height);
        }
    }
}
