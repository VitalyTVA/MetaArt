namespace SpringsPhysics;
class Trace {
    void setup() {
        size(600, 600);

        var setups = new[] { 
            //Simulations.createUnbalancedSpringPendulum(),
            Simulations.createDoubleUnbalancedSpringPendulum()
        };
        sims = setups.Select(x => (Physics.create(x), x)).ToArray();
    }

    (Physics, PhysicsSetup)[] sims = null!;


    void draw() {
        background(0, 0, 0, 3);
        stroke(255, 255);
        //noStroke();

        translate(0, 10);
        foreach(var (p, ps) in sims) {
            translate(150, 0);

            for(int i = 0; i < 10; i++) {
                var b = p.bodies.Last();
                push();
                translate(b.position.X, b.position.Y);
                point(0, 0);
                pop();

                p.advance();
            }
        }
    }
}
