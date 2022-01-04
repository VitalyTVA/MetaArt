namespace Interaction;
class Reach3 {
    /**
 * Reach 3  
 * based on code from Keith Peters.
 * 
 * The arm follows the position of the ball by
 * calculating the angles with atan2().
 */

    const int numSegments = 8;
    float[] x = new float[numSegments];
    float[] y = new float[numSegments];
    float[] angle = new float[numSegments];
    float segLength = 26;
    float targetX, targetY;

    float ballX = 50;
    float ballY = 50;
    int ballXDirection = 1;
    int ballYDirection = -1;

    void setup() {
        size(640, 360);
        strokeWeight(20.0f);
        stroke(255, 100);
        noFill();
        x[x.Length - 1] = width / 2;     // Set base x-coordinate
        y[x.Length - 1] = height;  // Set base y-coordinate
    }

    void draw() {
        background(0);

        strokeWeight(20);
        ballX = ballX + 1.0f * ballXDirection;
        ballY = ballY + 0.8f * ballYDirection;
        if(ballX > width - 25 || ballX < 25) {
            ballXDirection *= -1;
        }
        if(ballY > height - 25 || ballY < 25) {
            ballYDirection *= -1;
        }
        ellipse(ballX, ballY, 30, 30);

        reachSegment(0, ballX, ballY);
        for(int i = 1; i < numSegments; i++) {
            reachSegment(i, targetX, targetY);
        }
        for(int i = x.Length - 1; i >= 1; i--) {
            positionSegment(i, i - 1);
        }
        for(int i = 0; i < x.Length; i++) {
            segment(x[i], y[i], angle[i], (i + 1) * 2);
        }
    }

    void positionSegment(int a, int b) {
        x[b] = x[a] + cos(angle[a]) * segLength;
        y[b] = y[a] + sin(angle[a]) * segLength;
    }

    void reachSegment(int i, float xin, float yin) {
        float dx = xin - x[i];
        float dy = yin - y[i];
        angle[i] = atan2(dy, dx);
        targetX = xin - cos(angle[i]) * segLength;
        targetY = yin - sin(angle[i]) * segLength;
    }

    void segment(float x, float y, float a, float sw) {
        strokeWeight(sw);
        pushMatrix();
        translate(x, y);
        rotate(a);
        line(0, 0, segLength, 0);
        popMatrix();
    }
}
