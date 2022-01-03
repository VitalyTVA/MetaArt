namespace Simulate;

public class CrazyParticleSystem : ParticleSystem {
    public CrazyParticleSystem(int num, PVector v) : base(v) {
        for(int i = 0; i < num; i++) {
            particles.add(new Particle(origin));    // Add "num" amount of particles to the arraylist
        }
    }
    public override void addParticle() {
        Particle p;
        // Add either a Particle or CrazyParticle to the system
        if((int)(random(0, 2)) == 0) {
            p = new Particle(origin);
        } else {
            p = new CrazyParticle(origin);
        }
        particles.add(p);
    }
}

