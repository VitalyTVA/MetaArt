namespace SpringsPhysics;

class Simulations {
    public static PhysicsSetup createBallisticTrajectory() {
        return new PhysicsSetup(
            boxes: new[] { BoxBody.createBox(new Vector(9, 13), 100, new Vector(0, 0), new Vector(80, -40), 1, 2) },
            forceFields: new[] { Physics.createGravity(100) },
            springs: new SpringForceProvider[] { }
        );
    }
    public static PhysicsSetup createUnbalancedSpringPendulum() {
        var boxSize = new Vector(100, 20);
        var box = BoxBody.createBox(boxSize, 100, new Vector(0, 400), new Vector(0, 0));
        return new PhysicsSetup(
            boxes: new[] { box },
            forceFields: new[] { Physics.createGravity(100) },
            springs: new[] { Physics.createFixedSpring(250, new Vector(0, 0), box, new Vector(boxSize.X / 2, 0)) }
        );
    }
    //static createUnbalancedSpringPendulum_noGravity(): PhysicsSetup {
    //    var boxSize = new Vector(100, 20);
    //    var box = createBox(boxSize, 100, new Vector(0, 300), new Vector(0, 0));
    //    return {
    //        boxes: [box],
    //        forceFields: [],
    //        springs: [Physics.createFixedSpring(350, new Vector(0, 0), box, new Vector(boxSize.x / 2, 0))]
    //    }
    //}
    public static PhysicsSetup createDoubleUnbalancedSpringPendulum() {
        var boxSize3 = new Vector(100, 20);
        var box3 = BoxBody.createBox(boxSize3, 100, new Vector(0, 200), new Vector(0, 0));
        var box3_ = BoxBody.createBox(boxSize3, 100, new Vector(0, 400), new Vector(0, 0));
        var spring3 = Physics.createFixedSpring(500, new Vector(0, 0), box3, new Vector(boxSize3.X / 2, 0));
        var spring3_ = Physics.createDynamicSpring(500, box3, new Vector(-boxSize3.X / 2, 0), box3_, new Vector(boxSize3.X / 2, 0));
        return new PhysicsSetup(
            boxes: new[] { box3, box3_ },
            forceFields: new[] { Physics.createGravity(100) },
            springs: new[] { spring3, spring3_ }
        );
    }
    public static PhysicsSetup createBalancedRotatingBoxes() {
        var boxSize = new Vector(100, 20);
        var box1 = BoxBody.createBox(boxSize, 100, new Vector(0, -200), new Vector(0, 0));
        var box2 = BoxBody.createBox(boxSize, 100, new Vector(0, 200), new Vector(0, 0));
        var spring =
            Physics.createDynamicSpring(500, box1, new Vector(-boxSize.X / 2, 0), box2, new Vector(boxSize.X / 2, 0));
        //Physics.createDynamicSpring(500, model1.value, new Vector(0, 0), model2.value, new Vector(0, 0))

        return new PhysicsSetup(
            boxes: new[] { box1, box2 },
            forceFields: new ForceField[] { },
            springs: new[] { spring }
        );
    }
    public static PhysicsSetup createPushPullSwing() {
        var boxSize = new Vector(100, 20);
        //var model = ModelFactory.createBoxModel(boxSize, 100, new Vector(300, 300), new Vector(0, 0));
        var box = BoxBody.createBox(boxSize, 100, new Vector(0, 0), new Vector(0, 0));

        var spring1 = Physics.createFixedSpring(500, new Vector(0, -200), box, new Vector(boxSize.X / 2, 0));
        var spring2 = Physics.createFixedSpring(500, new Vector(0, 200), box, new Vector(-boxSize.X / 2, 0));

        return new PhysicsSetup(
            boxes: new[] { box },
            forceFields: new ForceField[] { },
            springs: new[] { spring1, spring2 }
        );
    }
}