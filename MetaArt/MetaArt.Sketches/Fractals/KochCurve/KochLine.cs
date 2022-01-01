namespace Fractals;

// The Nature of Code
// Daniel Shiffman
// http://natureofcode.com

// Koch Curve
// A class to describe one line segment in the fractal
// Includes methods to calculate midPVectors along the line according to the Koch algorithm
class KochLine {

    // Two PVectors,
    // a is the "left" PVector and 
    // b is the "right PVector
    public PVector Start { get; private set; }
    public PVector End { get; private set; }

    public KochLine(PVector start, PVector end) {
        Start = start;
        End = end;
    }


    // This is easy, just 1/3 of the way
    public PVector KochLeft() {
        PVector v = PVector.sub(End, Start);
        v.div(3);
        v.add(Start);
        return v;
    }

    // More complicated, have to use a little trig to figure out where this PVector is!
    public PVector KochMiddle() {
        PVector v = PVector.sub(End, Start);
        v.div(3);

        PVector p = Start;
        p.add(v);

        v.rotate(-radians(60));
        p.add(v);

        return p;
    }

    // Easy, just 2/3 of the way
    public PVector KochRight() {
        PVector v = PVector.sub(Start, End);
        v.div(3);
        v.add(End);
        return v;
    }
}

