namespace Simulate;
class MultipleParticleSystem {
    /**
     * Multiple Particle Systems
     * by Daniel Shiffman.
     *
     * Click the mouse to generate a burst of particles
     * at mouse position.
     *
     * Each burst is one instance of a particle system
     * with Particles and CrazyParticles (a subclass of Particle). 
     * Note use of Inheritance and Polymorphism.
     */

    ArrayList<ParticleSystem> systems = null!;

    void setup() {
        size(640, 360);
        systems = new ArrayList<ParticleSystem>();
    }

    void draw() {
        background(0);
        foreach(ParticleSystem ps in systems) {
            ps.run();
            ps.addParticle();
        }
        if(systems.isEmpty()) {
            fill(255);
            //textAlign(CENTER);
            text("click mouse to add particle systems", width / 2, height / 2);
        }
    }

    void mousePressed() {
        systems.add(new CrazyParticleSystem(1, new PVector(mouseX, mouseY)));
    }
}

