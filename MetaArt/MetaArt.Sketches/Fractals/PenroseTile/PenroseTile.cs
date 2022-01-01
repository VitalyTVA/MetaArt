namespace Fractals;
class PenroseTile {
    /** 
     * Penrose Tile L-System 
     * by Geraldine Sarmiento.
     *  
     * This example was based on Patrick Dwyer's L-System class. 
     */

    PenroseLSystem ds = new();

    void setup() {
        size(640, 360);
        ds.simulate(4);
    }

    void draw() {
        background(0);
        ds.render(advance: 12);
    }
}


