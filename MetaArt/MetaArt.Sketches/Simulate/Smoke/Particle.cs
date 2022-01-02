namespace Simulate;


// A simple Particle class, renders the particle as an image

class Particle {
    PVector loc;
    PVector vel;
    PVector acc;
    float lifespan;
    PImage img;

    public Particle(PVector l, PImage img_) {
        acc = new PVector(0, 0);
        float vx = randomGaussian() * 0.3f; //TODO use randomGaussian
        float vy = randomGaussian() * 0.3f - 1;
        vel = new PVector(vx, vy);
        loc = l;
        lifespan = 100;
        img = img_;
    }

    public void run() {
        update();
        render();
    }

    // Method to apply a force vector to the Particle object
    // Note we are ignoring "mass" here
    public void applyForce(PVector f) {
        acc.add(f);
    }

    // Method to update position
    void update() {
        vel.add(acc);
        loc.add(vel);
        lifespan -= 2.5f;
        acc.mult(0); // clear Acceleration
    }

    // Method to display
    void render() {
        imageMode(CENTER);
        tint(255, lifespan);
        image(img, loc.x, loc.y);
        // Drawing a circle instead
        // fill(255,lifespan);
        // noStroke();
        // ellipse(loc.x,loc.y,img.width,img.height);
    }

    // Is the particle still useful?
    public bool isDead() {
        if(lifespan <= 0.0) {
            return true;
        } else {
            return false;
        }
    }
}