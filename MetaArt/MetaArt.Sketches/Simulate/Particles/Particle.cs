namespace Simulate;

// A simple Particle class

public class Particle {
    protected PVector position;
    protected PVector velocity;
    PVector acceleration;
    protected float lifespan;

    public Particle(PVector l) {
        acceleration = new PVector(0, 0.05f);
        velocity = new PVector(random(-1, 1), random(-2, 0));
        position = l.copy();
        lifespan = 255;
    }

    public void run() {
        update();
        display();
    }

    // Method to update position
    protected virtual void update() {
        velocity.add(acceleration);
        position.add(velocity);
        lifespan -= 1;
    }

    // Method to display
    protected virtual void display() {
        stroke(255, lifespan);
        fill(255, lifespan);
        ellipse(position.x, position.y, 8, 8);
    }

    // Is the particle still useful?
    public bool isDead() {
        if(lifespan < 0.0) {
            return true;
        } else {
            return false;
        }
    }
}

