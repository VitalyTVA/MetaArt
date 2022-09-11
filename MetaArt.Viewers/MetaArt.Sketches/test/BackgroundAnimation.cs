namespace TestSketches;
class BackgroundAnimation {
    void setup() {
        size(200, 200);
    }


    int Call = 0;
    void draw() {
        background((byte)(Call % 255));
        Call += 5;
    }
}

