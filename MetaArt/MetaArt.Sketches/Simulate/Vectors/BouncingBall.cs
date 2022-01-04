namespace Vectors;
class BouncingBall {
    /**
     * Bouncing Ball with Vectors 
     * by Daniel Shiffman.  
     * 
     * Demonstration of using vectors to control motion 
     * of a body. This example is not object-oriented
     * See AccelerationWithVectors for an example of how 
     * to simulate motion using vectors in an object.
     */

    PVector location = new PVector(100, 100);  // Location of shape
    PVector velocity = new PVector(1.5f, 2.1f);  // Velocity of shape
    PVector gravity = new PVector(0, 0.2f);   // Gravity acts at the shape's acceleration

    void setup() {
        size(640, 360);
    }

    void draw() {
        background(0);

        // Add velocity to the location.
        location.add(velocity);
        // Add gravity to velocity
        velocity.add(gravity);

        // Bounce off edges
        if((location.x > width) || (location.x < 0)) {
            velocity.x = velocity.x * -1;
        }
        if(location.y > height) {
            // We're reducing velocity ever so slightly 
            // when it hits the bottom of the window
            velocity.y = velocity.y * -0.95f;
            location.y = height;
        }

        // Display circle at location vector
        stroke(255);
        strokeWeight(2);
        fill(127);
        ellipse(location.x, location.y, 48, 48);
    }
}

