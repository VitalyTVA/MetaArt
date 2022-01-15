namespace Meta;
class Inversion {
    void settings() {
        
    }
    void setup() {
        size(600, 600);

        stroke(White);
        strokeWeight(2);
        noFill();
    }

    float lastPressedX;
    float lastPressedY;
    float r = 100;
    void draw() {
        translate(width / 2, height / 2);
        circle(0, 0, r  * 2);
        if(isMousePressed) {
            mouseX -= width / 2;
            mouseY -= height / 2;
            if(lastPressedX != 0 && lastPressedY != 0) {

                line(lastPressedX, lastPressedY, mouseX, mouseY);
                var current = invertVector(mouseX, mouseY);
                var last = invertVector(lastPressedX, lastPressedY);

                line(last.x, last.y, current.x, current.y);
            }
            lastPressedX = mouseX;
            lastPressedY = mouseY;
        } else {
            lastPressedX = 0;
            lastPressedY = 0;
        }
    }
    PVector invertVector(float x, float y) {
        var v = new PVector(x, y);
        var len = r * r / v.Length;
        v.setMag(len);
        return v;
    }
}
