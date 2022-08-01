namespace Input;
class MousePressed {
    /**
     * Mouse Press. 
     * 
     * Move the mouse to position the shape. 
     * Press the mouse button to invert the color. 
     */


    void setup() {
        size(640, 360);
        noSmooth();
        fill(126);
    }

    void draw() {
        if(frameCount == 0) {
            background(102);
        }
        if(isMousePressed) {
            stroke(255);
        } else {
            stroke(0);
        }
        line(mouseX - 66, mouseY, mouseX + 66, mouseY);
        line(mouseX, mouseY - 66, mouseX, mouseY + 66);
    }
}
