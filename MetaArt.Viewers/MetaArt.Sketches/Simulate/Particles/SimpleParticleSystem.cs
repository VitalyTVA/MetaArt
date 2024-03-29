﻿namespace Simulate;
class SimpleParticleSystem {
    /**
 * Simple Particle System
 * by Daniel Shiffman.  
 * 
 * Particles are generated each cycle through draw(),
 * fall with gravity, and fade out over time.
 * A ParticleSystem object manages a variable size (ArrayList) 
 * list of particles. 
 */

    ParticleSystem ps = null!;

    void setup() {
        size(640, 360);
        ps = new ParticleSystem(new PVector(width / 2, 50));
    }

    void draw() {
        background(0);
        ps.addParticle();
        ps.run();
    }
}

