using System;
using System.Collections.Generic;
using System.Text;

namespace IFS;
class SerpinskyTriangle
{
    void setup()
    {
        size(800, 800);
        stroke(255);
        strokeWeight(1);
    }

    float x = 0;
    float y = 0;
    void draw()
    {
        translate(0, height);
        scale(1, -1);

        int maximum_iterations = 30;
        float t = 0;
        while(t < maximum_iterations) {
            float xn = 0;
            float yn = 0;
            float r = random(1);
            if(r < 1 / 3f) {
                xn = x / 2;
                yn = y / 2;
                stroke(Colors.Red);
            } else if(r < 2 / 3f) {
                xn = (x + width) / 2;
                yn = y / 2;
                stroke(Colors.Green);
            } else {
                xn = (x + width / 2) / 2;
                yn = (y + height) / 2;
                stroke(Colors.Blue);
            }
            point(round(xn), round(yn));
            x = xn;
            y = yn;
            t++;
        }
    }
}
