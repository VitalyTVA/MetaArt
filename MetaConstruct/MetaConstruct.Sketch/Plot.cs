using MetaArt.Core;
using System.Diagnostics;
using System.Numerics;

namespace MetaConstruct;

public class Plot {

    void setup() {
        if(deviceType() == DeviceType.Desktop)
            size(400, 700);
        fullRedraw();
    }


    void draw() {
        stroke(White);
        line(0, 0, 100, 100);
    }
}

