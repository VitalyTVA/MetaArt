namespace Fractals;
class PentigreeFractal {
    /** 
     * Pentigree L-System 
     * by Geraldine Sarmiento. 
     * 
     * This example was based on Patrick Dwyer's L-System class. 
     */


    PentigreeLSystem ps = new();

    void setup() {
        size(640, 360);
        ps.simulate(3);
    }

    void draw() {
        background(0);
        ps.render(advance: 3);
    }
}


