class PMouse {
    void setup() {
        size(600, 600);
        background(150);

        stroke(White);
        strokeWeight(2);
    }

    void draw() {
        line(pmouseX, pmouseY, mouseX, mouseY);
    }
}
