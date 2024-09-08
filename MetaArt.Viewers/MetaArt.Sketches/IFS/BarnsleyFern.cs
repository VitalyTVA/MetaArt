using System;
using System.Collections.Generic;
using System.Text;

namespace IFS; 
class BarnsleyFern {
    void setup() {
        size(800, 800);
        stroke(255);
        strokeWeight(1);
    }

    float x = 0;
    float y = 0;

    void draw() {
        translate(width / 2, height);
        scale(1, -1);

        int maximum_iterations = 30;

        float t = 0;
        while(t < maximum_iterations) {
            float xn = 0;
            float yn = 0;

            float r = random(1);
            if(r < 0.01) {
                xn = 0;
                yn = 0.16f * y;
                stroke(Colors.Blue);
            } else if(r < 0.86) {
                xn = 0.85f * x + 0.04f * y;
                yn = -0.04f * x + 0.85f * y + 1.6f;
                stroke(Colors.Red);
            } else if(r < 0.93) {
                xn = 0.2f * x - 0.26f * y;
                yn = 0.23f * x + 0.22f * y + 1.6f;
                stroke(Colors.Green);
            } else {
                xn = -0.15f * x + 0.28f * y;
                yn = 0.26f * x + 0.24f * y + 0.44f;
                stroke(Colors.Pink);
            }
            point(round(xn * width / 10), round(yn * height / 10));
            x = xn;
            y = yn;
            t++;
         }
    }
}
