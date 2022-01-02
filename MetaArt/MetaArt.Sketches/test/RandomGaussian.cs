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
}
