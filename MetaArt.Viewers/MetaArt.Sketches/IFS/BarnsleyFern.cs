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

    void draw() {
        translate(width / 2, 0);

        int maximum_iterations = 30;
        float x = 0;
        float y = 0;
        float t = 0;
        float xn = 0;
        float yn = 0;
        while(t < maximum_iterations) {


            float r = random(1);
            if(r < 0.01) {
                xn = 0;
                yn = 0.16f * y;
            } else if(r < 0.86) {
                xn = 0.85f * x + 0.04f * y;
                yn = -0.04f * x + 0.85f * y + 1.6f;
            } else if(r < 0.93) {
                xn = 0.2f * x - 0.26f * y;
                yn = 0.23f * x + 0.22f * y + 1.6f;
            } else {
                xn = -0.15f * x + 0.28f * y;
                yn = 0.26f * x + 0.24f * y + 0.44f;
            }
            point(round(xn * width / 10), round(yn * height / 10));
            x = xn;
            y = yn;
            t++;
         }
    }
}
