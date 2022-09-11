namespace Simulate;

// A subclass of Particle

class CrazyParticle : Particle {

    // Just adding one new variable to a CrazyParticle
    // It inherits all other fields from "Particle", and we don't have to retype them!
    float theta;

    // The CrazyParticle constructor can call the parent class (super class) constructor
    public CrazyParticle(PVector l) : base(l) {// do everything from the constructor in Particle
        // One more line of code to deal with the new variable, theta
        theta = 0;
    }

    // Notice we don't have the method run() here; it is inherited from Particle

    // This update() method overrides the parent class update() method
    protected override void update() {
        base.update();
        // Increment rotation based on horizontal velocity
        float theta_vel = (velocity.x * velocity.mag()) / 10.0f;
        theta += theta_vel;
    }

    // This display() method overrides the parent class display() method
    protected override void display() {
        // Render the ellipse just like in a regular particle
        base.display();
        // Then add a rotating line
        pushMatrix();
        translate(position.x, position.y);
        rotate(theta);
        stroke(255, lifespan);
        line(0, 0, 25, 0);
        popMatrix();
    }
}

