using MetaArt;
using System.Diagnostics;

namespace TestSketches;
class RandomGaussian {
    void setup() {
        size(400, 400);
        stroke(White);
        for(int y = 0; y < 400; y++) {
            float x = randomGaussian() * 60;
            line(200, y, 200 + x, y);
        }
    }
    static float randomGaussian() {
        const float mean = 0f;
        const float stdDev = 1;
        float u1 = 1 - (float)random(0, 1); //uniform(0,1] random doubles
        float u2 = 1 - (float)random(0, 1);
        float randStdNormal = sqrt(-2 * log(u1) * sin(2.0f * PI * u2)); //random normal(0,1)
        float randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        var sign = Math.Sign(random(0, 1) - .5);
        return randNormal * sign;
    }
}
