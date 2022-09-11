namespace SpringsPhysics;
class Springs {
    void setup() {
        size(800, 800);

        var setups = new[] { 
            Simulations.createUnbalancedSpringPendulum(),
            Simulations.createDoubleUnbalancedSpringPendulum(),
            Simulations.createBalancedRotatingBoxes(),
            Simulations.createPushPullSwing(),
        };
        sims = setups.Select(x => (Physics.create(x), x)).ToArray();
    }

    (Physics, PhysicsSetup)[] sims = null!;


    void draw() {
        background(0);
        fill(255);
        stroke(0, 255, 0);
        rectMode(RectMode.CENTER);

        translate(0, 200);
        foreach(var (p, ps) in sims) {
            translate(150, 0);

            foreach(BoxBody b in p.bodies) {
                push();
                translate(b.position.X, b.position.Y);
                rotate(b.angle);
                rect(0, 0, b.size.X, b.size.Y);
                pop();
            }

            foreach(var s in ps.springs) {
                var from = s.from();
                var to = s.to();
                line(from.X, from.Y, to.X, to.Y);
            }

            p.advance();
        }
    }
}
