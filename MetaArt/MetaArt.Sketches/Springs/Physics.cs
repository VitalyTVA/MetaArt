namespace SpringsPhysics;

public record struct AppliedForce(Vector force, Vector point);

public class RigidBody {
    public float mass { get; init; }
    public float momenOfInertia { get; init; }
    public float angle { get; set; }
    public float angularVelocity { get; set; }

    public Vector position { get; set; }
    public Vector velocity { get; set; }
}

public class BoxBody : RigidBody {
    public static BoxBody createBox(Vector size, float mass, Vector position, Vector velocity, float angle = 0, float angularVelocity = 0) {
    var momentOfInertia = mass * (size.X * size.X + size.Y * size.Y) / 12;
    return new BoxBody {
        mass = mass,
        position = position,
        velocity = velocity,
        momenOfInertia = momentOfInertia,
        angle = angle,
        angularVelocity = angularVelocity,
        size = size,
    };
}

    public Vector size { get; init; }
}

public static class Extensions {
    public static Vector toWorldPoint(this RigidBody body, Vector bodyPoint) {
        var rotatedBodyPoint = bodyPoint.Rotate(body.angle);
        return body.position + rotatedBodyPoint;
    }
    public static float energy(this RigidBody body) {
        return (body.velocity.LengthSquared * body.mass + body.angularVelocity * body.angularVelocity * body.momenOfInertia) / 2;
    }
}

public class ForceProvider {
    public Func<RigidBody, AppliedForce?> getForce { get; init; } = null!;
    public Func<RigidBody, float>? energy{ get; init; }
}

public class ForceField : ForceProvider {
    public Vector acceleration { get; init; }
}
public class SpringForceProvider : ForceProvider {
    //TODO test methods
    public Func<Vector> from { get; init; } = null!;
    public Func<Vector> to { get; init; } = null!;
    public Action<float> setRate { get; init; } = null!;
}

public record PhysicsSetup(BoxBody[] boxes, ForceProvider[] forceFields, SpringForceProvider[] springs, ForceProvider[]? additionalForces = null);

public enum AdvanceMode {
    Default,
    Smart,
}

class Physics {
    const float defaultStep = 0.01f;
    static ForceField createForceField(Vector acceleration) {
        return new ForceField {
            getForce = body => {
                var mass = body.mass;
                return new AppliedForce(
                    new Vector(acceleration.X * mass, acceleration.Y * mass), body.position);
            },
            energy = body => {
                if(acceleration.X != 0)
                    throw new Exception("todo");
                return -acceleration.Y * body.mass * body.position.Y;
            },
            acceleration = acceleration,
        };
    }
    public static ForceField createGravity(float g) {
        return createForceField(new Vector(0, g));
    }
    public static SpringForceProvider createFixedSpring(float rate, Vector fixedPoint, RigidBody body, Vector bodyPoint) {
        var spring = createSpring(
            rate,
            () => fixedPoint,
            () => body.toWorldPoint(bodyPoint)
        );
        return new SpringForceProvider {
            getForce = x => {
                if(body != x)
                    return null;
                return spring.getToForce();
            },
            energy = x => {
                if(body != x)
                    return 0;
                return spring.energy();
            },
            from = spring.from,
            to = spring.to,
            setRate = spring.setRate,
        };
    }
    static SpringForceProvider createDynamicSpring(float rate, RigidBody fromBody, Vector fromBodyPoint, RigidBody toBody, Vector toBodyPoint) {
        var spring = Physics.createSpring(
            rate,
            () => Extensions.toWorldPoint(fromBody, fromBodyPoint),
            () => Extensions.toWorldPoint(toBody, toBodyPoint));
        return new SpringForceProvider {
            getForce = x => {
                if(fromBody != x && toBody != x)
                    return null;
                if(toBody == x) {
                    return spring.getToForce();
                }
                return spring.getFromForce();
            },
            energy = x => {
                //TODO test
                if(fromBody != x)
                    return 0;
                return spring.energy();
            },
            from = spring.from,
            to = spring.to,
            setRate = spring.setRate,
        };
    }

    record struct Spring(Func<AppliedForce> getFromForce, Func<AppliedForce> getToForce, Func<float> energy, Func<Vector> from, Func<Vector> to, Action<float> setRate);

    static Spring createSpring(float rate, Func<Vector> fromWorldPoint, Func<Vector> toWorldPoint) {
        //TODO test
        AppliedForce getForce(Vector from, Vector to) {
            var vectorBetween = to - from;
            var force = vectorBetween.Mult(rate);
            return new AppliedForce(force, from);
        };
        return new Spring(
            getFromForce: () => getForce(fromWorldPoint(), toWorldPoint()),
            getToForce: () => getForce(toWorldPoint(), fromWorldPoint()),
            energy: () => (fromWorldPoint() - toWorldPoint()).LengthSquared * rate / 2,
            from: fromWorldPoint,
            to: toWorldPoint,
            setRate: newRate => rate = newRate
        );
    }


    public readonly List<RigidBody> bodies;
    readonly List<ForceProvider> forces;
    readonly float step;
    readonly AdvanceMode advanceMode;
    bool stopped;
    public Physics(List<RigidBody> bodies, List<ForceProvider> forces, float step = defaultStep, AdvanceMode advanceMode = AdvanceMode.Default) {
        if(step <= 0)
            throw new Exception("timeDelta should be positive");

        this.bodies = bodies;
        this.forces = forces;
        this.step = step;
        this.advanceMode = advanceMode;

        if(this.advanceMode == AdvanceMode.Smart)
            this.advanceVelocities(step / 2);
    }
    public static Physics create(PhysicsSetup setup) {
        //TODO test
        var forces = setup.forceFields.Concat(setup.springs);
        if(setup.additionalForces != null)
            forces = forces.Concat(setup.additionalForces);
        return new Physics(setup.boxes.ToList<RigidBody>(), forces.ToList(), defaultStep, AdvanceMode.Smart);
    }

    Vector[] positions() => this.bodies.Select(x => x.position).ToArray();
    Vector[] velocities() => this.bodies.Select(x => x.velocity).ToArray();
    float[] angles() => this.bodies.Select((x, _) => x.angle).ToArray();
    float[] angularVelocities() => this.bodies.Select((x, _) => x.angularVelocity).ToArray();

    public void advance() {
        if(this.stopped) {
            throw new Exception("Simulation stopped");
        }
        if(this.advanceMode == AdvanceMode.Default) {
            this.advanceVelocities(this.step);
            this.advancePositions(this.step);
        } else {
            this.advancePositions(this.step);
            this.advanceVelocities(this.step);
        }
    }
    public void catchUpPositions() {
        if(this.advanceMode == AdvanceMode.Smart) {
            this.advancePositions(this.step / 2);
            this.stopped = true;
        }
    }
    private void advanceVelocities(float dt) {
        foreach(var body in this.bodies) {
            var totalForceX = 0f;
            var totalForceY = 0f;
            var totalTorque = 0f;
            foreach(var force in this.forces) {
                var forceValue = force.getForce(body);
                if(forceValue == null)
                    continue;

                var r = body.position.VectorTo(forceValue.Value.point);
                var torque = r.X * forceValue.Value.force.Y - r.Y * forceValue.Value.force.X;

                totalForceX += forceValue.Value.force.X;
                totalForceY += forceValue.Value.force.Y;
                totalTorque += torque;
            }
            var v = body.velocity;
            body.velocity = new Vector(v.X + dt * totalForceX / body.mass, v.Y + dt * totalForceY / body.mass);
            body.angularVelocity = body.angularVelocity + dt * totalTorque / body.momenOfInertia;
        }
    }
    void advancePositions(float dt) {
        foreach(var body in this.bodies) {
            var p = body.position;
            var v = body.velocity;
            body.position = new Vector(p.X + v.X * dt, p.Y + v.Y * dt);
            body.angle = body.angle + body.angularVelocity * dt;
            if(Sketch.abs(body.angle) > Sketch.TWO_PI)
                body.angle = body.angle % Sketch.TWO_PI;
        }
    }
    float totalEnergy() {
        var res = 0f;
        foreach(var x in bodies) {
            res += Extensions.energy(x);
            res += this.forces.Sum(f => f.energy!(x));

        }
        return res;
    }
}
