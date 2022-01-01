//MetaArt version of https://glitch.com/edit/#!/p5-example-rings
//p5.js demos by @mattdesl https://p5-demos.glitch.me/

class Rings {
    // Set canvas size
    List<(float spinSpeed, float diameter, float arcLength, float arcAngle)> rings = new();
    void setup() {
        size(600, 600);

        int count = floor(random(10, 20));
        for(int i = 0; i < count; i++) {
            float diameter = ((i + 1f) / count);
            float arcLength = random(PI * 0.05f, TWO_PI);
            float arcAngle = random(-TWO_PI, TWO_PI);
            float spinSpeed = random(-1, 1);
            rings.Add((spinSpeed, diameter, arcLength, arcAngle));
        }
    }

    //// On window resize, update the canvas size
    //void windowResized() {
    //    resizeCanvas(windowWidth, windowHeight); //TODO auto size
    //}

    // Render loop that draws shapes
    void draw() {
        background(Black);

        float minDim = min(width, height);

        noFill();
        strokeWeight(minDim * 0.015f);
        strokeCap(StrokeCap.ROUND);
        stroke(White);

        float d = minDim;
        d -= d * 0.25f;
        foreach(var (spinSpeed, diameter, arcLength, arcAngle) in rings) {
            float spin = millis() / 1000f * spinSpeed;
            arc(
              width / 2,
              height / 2,
              diameter * d,
              diameter * d,
              spin + arcAngle,
              spin + arcAngle + arcLength
            );
        }
    }
}

