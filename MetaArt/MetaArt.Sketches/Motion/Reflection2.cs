namespace Motion;
class Reflection2 {
    /**
     * Non-orthogonal Collision with Multiple Ground Segments 
     * by Ira Greenberg. 
     * 
     * Based on Keith Peter's Solution in
     * Foundation Actionscript Animation: Making Things Move!
     */

    // An orb object that will fall and bounce around
    Orb orb = new Orb(50, 50, 3);

    public static PVector gravity = new PVector(0, 0.05f);
    // The ground is an array of "Ground" objects
    const int segments = 40;
    Ground[] ground = new Ground[segments];

    void setup() {
        size(640, 360);
        orb = new Orb(50, 50, 3);

        // Calculate ground peak heights 
        float[] peakHeights = new float[segments + 1];
        for(int i = 0; i < peakHeights.Length; i++) {
            peakHeights[i] = random(height - 40, height - 30);
        }

        /* Float value required for segment width (segs)
         calculations so the ground spans the entire 
         display window, regardless of segment number. */
        float segs = segments;
        for(int i = 0; i < segments; i++) {
            ground[i] = new Ground(width / segs * i, peakHeights[i], width / segs * (i + 1), peakHeights[i + 1]);
        }
    }

    void draw() {
        // Background
        noStroke();
        fill(0, 15);
        rect(0, 0, width, height);

        // Move and display the orb
        orb.move();
        orb.display();
        // Check walls
        orb.checkWallCollision();

        // Check against all the ground segments
        for(int i = 0; i < segments; i++) {
            orb.checkGroundCollision(ground[i]);
        }

        // Draw ground
        fill(127);
        beginShape();
        for(int i = 0; i < segments; i++) {
            vertex(ground[i].x1, ground[i].y1);
            vertex(ground[i].x2, ground[i].y2);
        }
        vertex(ground[segments - 1].x2, height);
        vertex(ground[0].x1, height);
        endShape(CLOSE);
    }
}

class Ground {
    public float x1, y1, x2, y2;
    public float x, y, len, rot;

    // Constructor
    public Ground(float x1, float y1, float x2, float y2) {
        this.x1 = x1;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;
        x = (x1 + x2) / 2;
        y = (y1 + y2) / 2;
        len = dist(x1, y1, x2, y2);
        rot = atan2((y2 - y1), (x2 - x1));
    }
}

class Orb {
    // Orb has positio and velocity
    PVector position;
    PVector velocity;
    float r;
    // A damping of 80% slows it down when it hits the ground
    float damping = 0.8f;

    public Orb(float x, float y, float r_) {
        position = new PVector(x, y);
        velocity = new PVector(.5f, 0);
        r = r_;
    }

    public void move() {
        // Move orb
        velocity.add(Reflection2.gravity);
        position.add(velocity);
    }

    public void display() {
        // Draw orb
        noStroke();
        fill(200);
        ellipse(position.x, position.y, r * 2, r * 2);
    }

    // Check boundaries of window
    public void checkWallCollision() {
        if(position.x > width - r) {
            position.x = width - r;
            velocity.x *= -damping;
        } else if(position.x < r) {
            position.x = r;
            velocity.x *= -damping;
        }
    }

    public void checkGroundCollision(Ground groundSegment) {

        // Get difference between orb and ground
        float deltaX = position.x - groundSegment.x;
        float deltaY = position.y - groundSegment.y;

        // Precalculate trig values
        float cosine = cos(groundSegment.rot);
        float sine = sin(groundSegment.rot);

        /* Rotate ground and velocity to allow 
         orthogonal collision calculations */
        float groundXTemp = cosine * deltaX + sine * deltaY;
        float groundYTemp = cosine * deltaY - sine * deltaX;
        float velocityXTemp = cosine * velocity.x + sine * velocity.y;
        float velocityYTemp = cosine * velocity.y - sine * velocity.x;

        /* Ground collision - check for surface 
         collision and also that orb is within 
         left/rights bounds of ground segment */
        if(groundYTemp > -r &&
          position.x > groundSegment.x1 &&
          position.x < groundSegment.x2) {
            // keep orb from going into ground
            groundYTemp = -r;
            // bounce and slow down orb
            velocityYTemp *= -1.0f;
            velocityYTemp *= damping;
        }

        // Reset ground, velocity and orb
        deltaX = cosine * groundXTemp - sine * groundYTemp;
        deltaY = cosine * groundYTemp + sine * groundXTemp;
        velocity.x = cosine * velocityXTemp - sine * velocityYTemp;
        velocity.y = cosine * velocityYTemp + sine * velocityXTemp;
        position.x = groundSegment.x + deltaX;
        position.y = groundSegment.y + deltaY;
    }
}

