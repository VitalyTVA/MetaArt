//MetaArt version of https://glitch.com/edit/#!/p5-example-algorithm
//p5.js demos by @mattdesl https://p5-demos.glitch.me/

class Substrate : SketchBase {
    const int initialLines = 100;
    const int maxLines = 1000;

    List<Line> lines = new();

    // Set canvas size
    void setup() {
        size(600, 600);
        for(int i = 0; i < initialLines && i < maxLines; i++) {
            // Default to randomly within -1..1 space
            var origin = new Point(random(-1, 1), random(-1, 1));

            // Default to a random direction
            var direction = randomUnitVector();

            lines.Add(createLine(origin, direction));
        }
    }

    Line createLine(Point origin, Point direction) {
        // We will use normalized coordinates
        // And then scale them to the screen size
        return new Line {
            Origin = origin, // starting position
            Position = origin, // initially start at origin
            Direction = direction,
            Speed = random(0.1f, 0.25f),
            Moving = true 
        };
    }

    void stepLine(Line line, float deltaTime) {
        // Ignore stopped lines
        if(!line.Moving) return;

        // Line start position
        float x0 = line.Origin.X;
        float y0 = line.Origin.Y;

        // New line end position
        float x1 = line.Position.X + line.Direction.X * line.Speed * deltaTime;
        float y1 = line.Position.Y + line.Direction.Y * line.Speed * deltaTime;

        // If we hit another...
        Line? hitLine = null;

        // Check intersections against others
        for(int i = 0; i < lines.Count; i++) {
            var other = lines[i];
    
            // ignore self
            if(other == line) continue;
    
            // if the lines have collided already, skip
            if(line.Hits.Contains(other) || other.Hits.Contains(line)) {
                continue;
            }

            var hit = intersectLineSegments(
                // this line A->B
                new Point (x0, y0),
                new Point(x1, y1),
                // other line A->B
                new Point(other.Origin.X, other.Origin.Y),
                new Point(other.Position.X, other.Position.Y)
            );

            // We hit another line, make sure we didn't go further than it
            if(hit != null) {
                // Clamp position to the intersection point so it doesn't go beyond
                x1 = hit.Value.X;
                y1 = hit.Value.Y;

                hitLine = other;
                break;
            }

            // Line has reached outside of frame, stop it
            bool outsideBounds = x1 > 1 || x1 < -1 || y1 > 1 || y1 < -1;
            if(outsideBounds) {
                line.Moving = false;
                break;
            }
        }

        line.Position = new Point(x1, y1);

        if(hitLine != null) {
            // Mark this line as stopped
            line.Moving = false;

            // Mark the lines as hit so they don't check again
            line.Hits.Add(hitLine);
            hitLine.Hits.Add(line);

            if (lines.Count < maxLines) {
                // Produce a new line at perpendicular
                var producedLine = produceLine(line);

                // Make sure the line knows we already hit these two
                producedLine.Hits.Add(line);
                producedLine.Hits.Add(hitLine);

                // Also make sure the hit line knows not to check it
                hitLine.Hits.Add(producedLine);

                // Add to list
                lines.Add(producedLine);
            }
        }
    }

    Line produceLine(Line line) {
        // Select a random point along the line and create a new
        // line extending in a perpendicular angle
        float t = random(0.15f, 0.85f);
        float px = lerp(line.Origin.X, line.Position.X, t);
        float py = lerp(line.Origin.Y, line.Position.Y, t);

        // Random perpendicular orientation
        float sign = random(0, 1) > 0.5 ? 1 : -1;

        // Get a perpendicular to the line
        Point direction = new(-line.Direction.Y * sign, line.Direction.X * sign);

        return createLine(new Point(px, py), direction);
    }

    //// On window resize, update the canvas size
    //void windowResized() {
    //    resizeCanvas(windowWidth, windowHeight); //TODO auto size
    //}

    // Render loop that draws shapes
    void draw() {
        background(Black);

        // Step all the lines first
        // Use a fixed delta-time
        float dt = 1f / 24;
        foreach(var line in lines.ToArray()) {
            stepLine(line, dt);
        }

        // Now draw all the lines
        float dim = min(width, height);
        noFill();
        stroke(White);

        push();
        translate(width / 2, height / 2);
        scale(dim, dim);
        strokeWeight(0.01f);
        foreach(var l in lines) {
            var (x0, y0) = l.Origin;
            var (x1, y1) = l.Position;
            line(x0, y0, x1, y1);
        }
        pop();
    }


    // -----------------
    // ... Utilities ...
    // -----------------

    Point randomUnitVector() {
        float radius = 1; // unit circle
        float theta = random(0, 1) * 2.0f * PI;
        return new Point(radius * cos(theta), radius * sin(theta));
    }

    Point? intersectLineSegments(Point p1, Point p2, Point p3, Point p4) {
        float t = intersectLineSegmentsFract(p1, p2, p3, p4);
        if(t >= 0 && t <= 1) {
            return new Point(p1.X + t * (p2.X - p1.X), p1.Y + t * (p2.Y - p1.Y));
        }
        return null;
    }

    float intersectLineSegmentsFract(Point p1, Point p2, Point p3, Point p4) {
        // Reference:
        // https://github.com/evil-mad/EggBot/blob/master/inkscape_driver/eggbot_hatch.py
        float d21x = p2.X - p1.X;
        float d21y = p2.Y - p1.Y;
        float d43x = p4.X - p3.X;
        float d43y = p4.Y - p3.Y;

        // denominator
        float d = d21x * d43y - d21y * d43x;
        if(d == 0) return -1;

        float nb = (p1.Y - p3.Y) * d21x - (p1.X - p3.X) * d21y;
        float sb = nb / d;
        if(sb < 0 || sb > 1) return -1;

        float na = (p1.Y - p3.Y) * d43x - (p1.X - p3.X) * d43y;
        float sa = na / d;
        if(sa < 0 || sa > 1) return -1;
        return sa;
    }
    class Line {
        public Point Origin { get; init; }
        public Point Position { get; set; }
        public Point Direction { get; init; }
        public float Speed { get; init; }
        public List<Line> Hits { get; init; } = new();
        public bool Moving { get; set; }
    }
}

