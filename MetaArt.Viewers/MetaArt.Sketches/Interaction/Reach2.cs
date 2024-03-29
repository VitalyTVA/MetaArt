﻿namespace Interaction;
class Reach2 {
    /**
 * Reach 2  
 * based on code from Keith Peters.
 * 
 * The arm follows the position of the mouse by
 * calculating the angles with atan2(). 
 */

    const int numSegments = 10;
    float[] x = new float[numSegments];
    float[] y = new float[numSegments];
    float[] angle = new float[numSegments];
    float segLength = 26;
    float targetX, targetY;

    void setup() {
        size(640, 360);
        strokeWeight(20.0f);
        stroke(255, 100);
        x[x.Length - 1] = width / 2;     // Set base x-coordinate
        y[x.Length - 1] = height;  // Set base y-coordinate
    }

    void draw() {
        background(0);

        reachSegment(0, mouseX, mouseY);
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
