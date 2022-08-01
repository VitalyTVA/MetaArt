namespace TestSketches;
class PMouse {
    void setup() {
        size(600, 600);
        stroke(White);
        strokeWeight(2);
    }

    void draw() {
        if(frameCount == 0) {
            background(150);
        }
        line(pmouseX, pmouseY, mouseX, mouseY);
    }
}
