namespace Simulate;
/**
 * Forces (Gravity and Fluid Resistence) with Vectors 
 * by Daniel Shiffman.  
 * 
 * Demonstration of multiple force acting on bodies (Mover class)
 * Bodies experience gravity continuously
 * Bodies experience fluid resistance when in "water"
 */

class Mover {

    // position, velocity, and acceleration 
    public PVector position;
    public PVector velocity;
    PVector acceleration;

    // Mass is tied to size
    public float mass;

    public Mover(float m, float x, float y) {
        mass = m;
        position = new PVector(x, y);
        velocity = new PVector(0, 0);
        acceleration = new PVector(0, 0);
    }

    // Newton's 2nd law: F = M * A
    // or A = F / M
    public void applyForce(PVector force) {
        // Divide by mass 
        PVector f = PVector.div(force, mass);
        // Accumulate all forces in acceleration
        acceleration.add(f);
    }

    public void update() {
        // Velocity changes according to acceleration
        velocity.add(acceleration);
        // position changes by velocity
        position.add(velocity);
        // We must clear acceleration each frame
        acceleration.mult(0);
    }

    // Draw Mover
    public void display() {
        stroke(255);
        strokeWeight(2);
        fill(255, 200);
        ellipse(position.x, position.y, mass * 16, mass * 16);
    }

    // Bounce off bottom of window
    public void checkEdges() {
        if(position.y > height) {
            velocity.y *= -0.9f;  // A little dampening when hitting the bottom
            position.y = height;
        }
    }
}

