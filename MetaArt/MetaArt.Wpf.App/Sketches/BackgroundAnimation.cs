class BackgroundAnimation : SketchBase {
    public override void setup() {
        size(200, 200);
        background(150);
    }


    int Call = 0;
    public override void draw() {
        background((byte)(Call % 255));
        Call += 5;
    }
}

