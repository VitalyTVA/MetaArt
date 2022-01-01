namespace Fractals;
// Koch Curve
// A class to manage the list of line segments in the snowflake pattern

class KochFractal {
    PVector start;       // A PVector for the start
    PVector end;         // A PVector for the end
    List<KochLine> lines = new();   // A list to keep track of all the lines
    public int Count { get; private set; }

    public KochFractal(int width, int height) {
        start = new PVector(0, height - 20);
        end = new PVector(width, height - 20);
        Restart();
    }

    public void Draw() {
        foreach(KochLine l in lines) {
            line(l.Start.X, l.Start.Y, l.End.X, l.End.Y);
        }
    }

    public void NextLevel() {
        // For every line that is in the arraylist
        // create 4 more lines in a new arraylist
        lines = Iterate(lines);
        Count++;
    }

    public void Restart() {
        Count = 0;      // Reset count
        lines.Clear();  // Empty the array list
        lines.Add(new KochLine(start, end));  // Add the initial line (from one end PVector to the other)
    }

    // This is where the **MAGIC** happens
    // Step 1: Create an empty arraylist
    // Step 2: For every line currently in the arraylist
    //   - calculate 4 line segments based on Koch algorithm
    //   - add all 4 line segments into the new arraylist
    // Step 3: Return the new arraylist and it becomes the list of line segments for the structure

    // As we do this over and over again, each line gets broken into 4 lines, which gets broken into 4 lines, and so on. . . 
    List<KochLine> Iterate(List<KochLine> before) {
        List<KochLine> now = new();    // Create emtpy list
        foreach(KochLine l in before) {
            // Calculate 5 koch PVectors (done for us by the line object)
            PVector a = l.Start;
            PVector b = l.KochLeft();
            PVector c = l.KochMiddle();
            PVector d = l.KochRight();
            PVector e = l.End;
            // Make line segments between all the PVectors and add them
            now.Add(new KochLine(a, b));
            now.Add(new KochLine(b, c));
            now.Add(new KochLine(c, d));
            now.Add(new KochLine(d, e));
        }
        return now;
    }
}
