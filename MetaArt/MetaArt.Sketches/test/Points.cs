namespace TestSketches;
class Points {
    void setup() {
        size(400, 400);
        background(0);
        stroke(255);
    }

    void draw() {
        background(0);
        strokeWeight(5);
        strokeCap(StrokeCap.ROUND);
        point(50f, 50f);

        strokeCap(StrokeCap.PROJECT);
        point(60f, 50f);

        strokeCap(StrokeCap.SQUARE);
        point(70f, 50f);

        set(80f, 50f, color(255));

        strokeWeight(1);
        strokeCap(StrokeCap.ROUND);
        point(50f, 100f);

        strokeCap(StrokeCap.PROJECT);
        point(60f, 100f);

        strokeCap(StrokeCap.SQUARE);
        point(70f, 100f);

        set(80f, 100f, color(255));

        strokeWeight(1f);
        strokeCap(StrokeCap.ROUND);
        point(50f, 150f);

        strokeCap(StrokeCap.PROJECT);
        point(60f, 150f);

        strokeCap(StrokeCap.SQUARE);
        point(70f, 150f);
    }
}
