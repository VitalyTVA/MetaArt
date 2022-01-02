namespace Simulate;

// A class to describe a group of Particles
// An ArrayList is used to manage the list of Particles 

class ParticleSystem {

    List<Particle> particles = new();    // An arraylist for all the particles
    PVector origin;                   // An origin point for where particles are birthed
    PImage img;

    public ParticleSystem(int num, PVector v, PImage img_) {
        origin = v;                                   // Store the origin point
        img = img_;
        for(int i = 0; i < num; i++) {
            particles.Add(new Particle(origin, img));         // Add "num" amount of particles to the arraylist
        }
    }

    public void run() {
        for(int i = particles.Count - 1; i >= 0; i--) {
            Particle p = particles[i];
            p.run();
            if(p.isDead()) {
                particles.RemoveAt(i);
            }
        }
    }

    // Method to add a force vector to all particles currently in the system
    public void applyForce(PVector dir) {
        // Enhanced loop!!!
        foreach(Particle p in particles) {
            p.applyForce(dir);
        }
    }

    public void addParticle() {
        particles.Add(new Particle(origin, img));
    }
}
