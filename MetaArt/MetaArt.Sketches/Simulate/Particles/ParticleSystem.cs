namespace Simulate;
//// A class to describe a group of Particles
//// An ArrayList is used to manage the list of Particles 

public class ParticleSystem {
    protected ArrayList<Particle> particles;
    protected PVector origin;

    public ParticleSystem(PVector position) {
        origin = position.copy();
        particles = new ArrayList<Particle>();
    }

    public virtual void addParticle() {
        particles.add(new Particle(origin));
    }

    public void run() {
        for(int i = particles.size() - 1; i >= 0; i--) {
            Particle p = particles.get(i);
            p.run();
            if(p.isDead()) {
                particles.remove(i);
            }
        }
    }
}

