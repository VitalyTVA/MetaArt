namespace SpringsPhysics;
class Springs {
    void setup() {
        size(600, 600);

        ps = Simulations.createUnbalancedSpringPendulum();
        p = Physics.create(ps);
    }

    Physics p = null!;
    PhysicsSetup ps = null!;

    void draw() {
        background(0);
        fill(255);
        stroke(0, 255, 0);

        translate(100, 100);

        foreach(BoxBody b in p.bodies) {
            circle(b.position.X, b.position.Y, 20);
        }


        foreach(var s in ps.springs) {
            var from = s.from();
            var to = s.to();
            line(from.X, from.Y, to.X, to.Y);
        }

        p.advance();
    }
}
