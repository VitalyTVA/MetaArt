namespace Vectors;

/**
 * Acceleration with Vectors 
 * by Daniel Shiffman.  
 * 
 * Demonstration of the basics of motion with vector.
 * A "Mover" object stores location, velocity, and acceleration as vectors
 * The motion is controlled by affecting the acceleration (in this case towards the mouse)
 */


class Mover {
    // The Mover tracks location, velocity, and acceleration 
    PVector location;
    PVector velocity;
    // The Mover's maximum speed
    float topspeed;

    public Mover() {
        // Start in the center
        location = new PVector(width / 2, height / 2);
        velocity = new PVector(0, 0);
        topspeed = 5;
    }

    public void update() {

        // Compute a vector that points from location to mouse
        PVector mouse = new PVector(mouseX, mouseY);
        PVector acceleration = PVector.sub(mouse, location);
        // Set magnitude of acceleration
        acceleration.setMag(0.2f);

        // Velocity changes according to acceleration
        velocity.add(acceleration);
        // Limit the velocity by topspeed
        velocity.limit(topspeed);
        // Location changes by velocity
        location.add(velocity);
    }
    public void display() {
        stroke(255);
        strokeWeight(2);
        fill(127);
        ellipse(location.x, location.y, 48, 48);
    }
}


