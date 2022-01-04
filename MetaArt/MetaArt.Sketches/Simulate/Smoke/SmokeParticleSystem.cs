namespace Simulate;

// A class to describe a group of Particles
// An ArrayList is used to manage the list of Particles 

class SmokeParticleSystem {

    List<SmokeParticle> particles = new();    // An arraylist for all the particles
    PVector origin;                   // An origin point for where particles are birthed
    PImage img;

    public SmokeParticleSystem(int num, PVector v, PImage img_) {
        origin = v.copy();                                   // Store the origin point
        img = img_;
        for(int i = 0; i < num; i++) {
            particles.Add(new SmokeParticle(origin, img));         // Add "num" amount of particles to the arraylist
        }
    }

    public void run() {
        for(int i = particles.Count - 1; i >= 0; i--) {
            SmokeParticle p = particles[i];
            p.run();
            if(p.isDead()) {
                particles.RemoveAt(i);
            }
        }
    }

    // Method to add a force vector to all particles currently in the system
    public void applyForce(PVector dir) {
        // Enhanced loop!!!
        foreach(SmokeParticle p in particles) {
            p.applyForce(dir);
        }
    }

    public void addParticle() {
        particles.Add(new SmokeParticle(origin, img));
    }
}
