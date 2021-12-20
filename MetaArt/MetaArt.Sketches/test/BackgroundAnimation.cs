class BackgroundAnimation : SketchBase {
    void setup() {
        size(200, 200);
        background(150);
    }


    int Call = 0;
    void draw() {
        background((byte)(Call % 255));
        Call += 5;
    }
}

