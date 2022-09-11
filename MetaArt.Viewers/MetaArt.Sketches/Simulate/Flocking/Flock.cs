namespace Simulate;
// The Flock (a list of Boid objects)

class Flock {
    ArrayList<Boid> boids; // An ArrayList for all the boids

    public Flock() {
        boids = new ArrayList<Boid>(); // Initialize the ArrayList
    }

    public void run() {
        foreach(Boid b in boids) {
            b.run(boids);  // Passing the entire list of boids to each boid individually
        }
    }

    public void addBoid(Boid b) {
        boids.add(b);
    }

}


