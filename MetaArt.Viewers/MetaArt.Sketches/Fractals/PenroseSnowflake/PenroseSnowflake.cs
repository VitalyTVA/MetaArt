﻿namespace Fractals;
class PenroseSnowflake {
    /** 
     * Penrose Snowflake L-System 
     * by Geraldine Sarmiento. 
     * 
     * This example was based on Patrick Dwyer's L-System class. 
     */

    PenroseSnowflakeLSystem ps = new();

    void setup() {
        size(640, 360);
        stroke(255);
        noFill();
        ps.simulate(4);
    }

    void draw() {
        background(0);
        ps.render(advance: 3);
    }
}


