using NUnit.Framework;
using SpringsPhysics;

namespace MetaArt.Sketches.Tests {
    [TestFixture]
    public class PhysicsTests {
        const float delta = 0.001f;
        [Test]
        public void CreateBox() {
            var boxSize = new Vector(9, 13);
            var boxBody = BoxBody.createBox(boxSize, 100, new Vector(200, 200), new Vector(80, -40), 2, 13);
            Assert.AreEqual(100, boxBody.mass);
            Assert.AreEqual(new Vector(200, 200), boxBody.position);
            Assert.AreEqual(new Vector(80, -40), boxBody.velocity);
            Assert.AreEqual(2083.333333, boxBody.momenOfInertia, delta);
            Assert.AreEqual(2, boxBody.angle);
            Assert.AreEqual(13, boxBody.angularVelocity);
            Assert.AreEqual(576041.625f, Extensions.energy(boxBody));
        }

        [Test]
        public void NoForceMotion() {
            var body1 = BoxBody.createBox(new Vector(10, 10), 100, new Vector(200, 100), new Vector(10, 20), 2, 3);
            var body2 = BoxBody.createBox(new Vector(20, 20), 50, new Vector(100, 50), new Vector(50, 100), -4, -5);
            var physics = new Physics(new List<RigidBody> { body1, body2 }, new List<ForceProvider>(), 0.2f);

            CollectionAssert.AreEqual(
                physics.positionsForTests(),
                new[] { new Vector(200, 100), new Vector(100, 50) }
            );
            CollectionAssert.AreEqual(
                physics.velocitiesForTests(),
                new[] { new Vector(10, 20), new Vector(50, 100) }
            );
            CollectionAssert.AreEqual(physics.anglesForTests(), new[] { 2, -4 });
            CollectionAssert.AreEqual(physics.angularVelocitiesForTests(), new[] { 3, -5 });

            physics.advance();
            CollectionAssert.AreEqual(
                physics.positionsForTests(),
                new[] { new Vector(202, 104), new Vector(110, 70) }
            );
            CollectionAssert.AreEqual(
                physics.velocitiesForTests(),
                new[] { new Vector(10, 20), new Vector(50, 100) }
            );
            CollectionAssert.AreEqual(physics.anglesForTests(), new float[] { 2.6f, -5 });
            CollectionAssert.AreEqual(physics.angularVelocitiesForTests(), new float[] { 3, -5 });
        }

        [Test]
        public void NormalizeanglesForTests() {
            var body1 = BoxBody.createBox(new Vector(10, 10), 100, new Vector(200, 100), new Vector(10, 20), 2, 3);
            var body2 = BoxBody.createBox(new Vector(20, 20), 50, new Vector(100, 50), new Vector(50, 100), -4, -5);
            var physics = new Physics(new List<RigidBody> { body1, body2 }, new List<ForceProvider>(), 2.2f);

            physics.advance();
            Assert.AreEqual(physics.anglesForTests(), new float[] { 2.3168149f, -2.43362904f });
            Assert.AreEqual(physics.angularVelocitiesForTests(), new float[] { 3, -5 });
        }
        [Test] public void ForceFieldMotion() {
            var physics = new Physics(
                new List<RigidBody> {
                    BoxBody.createBox(new Vector(10, 10), 100, new Vector(200, 100), new Vector(10, 20)),
                    BoxBody.createBox(new Vector(20, 20), 50, new Vector(100, 50), new Vector(50, 100)),
                },
                new List<ForceProvider> {
                    Physics.createForceField(new Vector(2, 3)),
                    new ForceProvider { getForce = (body => null) },
                    Physics.createForceField(new Vector(-4, 2)),
                },
                0.2f
            );

            Assert.AreEqual(
                physics.positionsForTests(),
                new[] { new Vector(200, 100), new Vector(100, 50) }
            );
            Assert.AreEqual(
                physics.velocitiesForTests(),
                new[] { new Vector(10, 20), new Vector(50, 100) }
            );

            physics.advance();
            Assert.AreEqual(
                physics.positionsForTests(),
                new[] { new Vector(201.92f, 104.2f), new Vector(109.92f, 70.2f) }
            );
            Assert.AreEqual(
                physics.velocitiesForTests(),
                new[] { new Vector(9.6f, 21), new Vector(49.6f, 101) }
            );
            Assert.AreEqual(
                physics.anglesForTests(),
                new float[] { 0, 0 }
            );
            Assert.AreEqual(
                physics.angularVelocitiesForTests(),
                new float[] { 0, 0 }
            );

            physics.catchUpPositions();
            Assert.AreEqual(
                physics.positionsForTests(),
                new[] { new Vector(201.92f, 104.2f), new Vector(109.92f, 70.2f) }
            );
            Assert.AreEqual(
                physics.velocitiesForTests(),
                new[] { new Vector(9.6f, 21), new Vector(49.6f, 101) }
            );
        }

        [Test]
        public void ForceFieldMotion_SmartAdvance() {
            var box = BoxBody.createBox(new Vector(10, 10), 100, new Vector(200, 100), new Vector(10, 20));
            var physics = new Physics(
                new List<RigidBody> { box },
                new List<ForceProvider> { Physics.createForceField(new Vector(-2, 5)) },
                0.2f,
                AdvanceMode.Smart
            );

            Assert.AreEqual(box.position, new Vector(200, 100));
            Assert.AreEqual(box.velocity, new Vector(9.8f, 20.5f));

            physics.advance();
            Assert.AreEqual(box.position, new Vector(201.96f, 104.1f));
            Assert.AreEqual(box.velocity, new Vector(9.400001f, 21.5f));

            physics.catchUpPositions();
            Assert.AreEqual(box.position, new Vector(202.90001f, 106.25f));
            Assert.AreEqual(box.velocity, new Vector(9.400001f, 21.5f));

            Assert.Throws<Exception>(() => physics.advance());
        }


        [Test]
        public void ForceFieldEnergy() {
            var field = Physics.createForceField(new Vector(0, 3));
            Assert.AreEqual(-60000, field.energy!(BoxBody.createBox(new Vector(10, 10), 100, new Vector(200, 200), new Vector(10, 20))));
            Assert.AreEqual(-10500, field.energy!(BoxBody.createBox(new Vector(20, 20), 50, new Vector(100, 70), new Vector(50, 100))));
        }

        [Test] 
        public void InvalidAdvanceTimeValue() {
            Assert.Throws<Exception>(() => new Physics(new List<RigidBody> { }, new List<ForceProvider> { }, 0));
            Assert.Throws<Exception>(() => new Physics(new List<RigidBody> { }, new List<ForceProvider> { }, -0.1f));
        }

        [Test] 
        public void DragAndTorqueMotion() {
            var body = BoxBody.createBox(new Vector(10, 10), 2, new Vector(200, 100), new Vector(0, 0));
            var physics = new Physics(
                new List<RigidBody> { body },
                new List<ForceProvider> {
                    new ForceProvider { getForce = body => new AppliedForce(new Vector(1, 2), body.position + new Vector(2, 3)) },
                    new ForceProvider { getForce = body => new AppliedForce(new Vector(-1, 1), body.position + new Vector(2, 2)) },
                },
                0.2f
            );

            physics.advance();
            Assert.AreEqual(body.position, new Vector(200, 100.06f));
            Assert.AreEqual(body.velocity, new Vector(0, 0.3f));
            Assert.AreEqual(body.angle, 0.006f, delta);
            Assert.AreEqual(body.angularVelocity, 0.03, delta);
        }
        [Test] 
        public void PhysicsTotalEnergy() {
            var body = BoxBody.createBox(new Vector(10, 10), 200, new Vector(200, 100), new Vector(0, 0));
            var physics = new Physics(
                new List<RigidBody> { body },
                new List<ForceProvider> {
                    Physics.createFixedSpring(100, new Vector(0, 0), body, new Vector(1, 2)),
                    Physics.createGravity(50),
                },
                0.01f
            );
            Assert.AreEqual(physics.totalEnergy(), 1540250, delta);
            physics.advance();
            Assert.AreEqual(physics.totalEnergy(), 1540135.5, delta);
        }


        [Test] 
        public void FixedPointSpring() {
            var body = BoxBody.createBox(new Vector(10, 10), 2, new Vector(21, 13), new Vector(10, 20));
            var spring = Physics.createFixedSpring(15, new Vector(9, 13), body, new Vector(2, 3));

            var someBody = BoxBody.createBox(new Vector(10, 10), 2, new Vector(200, 100), new Vector(0, 0));
            Assert.AreEqual(null, spring.getForce(someBody));
            Assert.AreEqual(0, spring.energy!(someBody));

            var appliedForce = spring.getForce(body)!.Value;
            Assert.AreEqual(appliedForce.force, new Vector(-210, -45));
            Assert.AreEqual(appliedForce.point, new Vector(23, 16));
            Assert.AreEqual(1537.5, spring.energy(body));

            body.angle = 2.2f;
            appliedForce = spring.getForce(body)!.Value;
            Assert.AreEqual(appliedForce.force, new Vector(-125.96263f, 2.227664f));
            Assert.AreEqual(appliedForce.point, new Vector(17.39750855f, 12.851489455f));
            Assert.AreEqual(529.0515397254967, spring.energy(body), delta);

            spring.setRate(30);
            appliedForce = spring.getForce(body)!.Value;
            Assert.AreEqual(appliedForce.force, new Vector(-251.92526f, 4.455328f));
            Assert.AreEqual(appliedForce.point, new Vector(17.39750855f, 12.851489455f));
            Assert.AreEqual(529.0515397254967 * 2, spring.energy(body), delta);

        }
        [Test] 
        public void DynamicSpring() {
            var body1 = BoxBody.createBox(new Vector(10, 10), 2, new Vector(21, 13), new Vector(10, 20));
            var body2 = BoxBody.createBox(new Vector(15, 25), 3, new Vector(57, 25), new Vector(30, 40));
            var spring = Physics.createDynamicSpring(15, body1, new Vector(9, 13), body2, new Vector(2, 3));

            var someBody = BoxBody.createBox(new Vector(10, 10), 2, new Vector(200, 100), new Vector(0, 0));
            Assert.AreEqual(spring.getForce(someBody), null);
            Assert.AreEqual(spring.energy!(someBody), 0);

            var appliedForceBody1 = spring.getForce(body1)!.Value;
            Assert.AreEqual(appliedForceBody1.force, new Vector(435, 30));
            Assert.AreEqual(appliedForceBody1.point, new Vector(30, 26));
            var appliedForceBody2 = spring.getForce(body2)!.Value;
            Assert.AreEqual(appliedForceBody2.force, new Vector(-435, -30));
            Assert.AreEqual(appliedForceBody2.point, new Vector(59, 28));
            Assert.AreEqual(spring.energy(body1) + spring.energy(body2), 6337.5);


            body1.angle = 2.2f;
            body2.angle = -3.5f;
            appliedForceBody1 = spring.getForce(body1)!.Value;
            Assert.AreEqual(appliedForceBody1.force, new Vector(733.22546f, 153.99367f));
            Assert.AreEqual(appliedForceBody1.point, new Vector(5.193037f, 12.625952f));
            appliedForceBody2 = spring.getForce(body2)!.Value;
            Assert.AreEqual(appliedForceBody2.force, new Vector(-733.22546f, -153.99367f));
            Assert.AreEqual(appliedForceBody2.point, new Vector(54.074736942349546f, 22.89219639350685f));
            Assert.AreEqual(spring.energy(body1) + spring.energy(body2), 18711.12109375f, delta);

            spring.setRate(30);
            appliedForceBody1 = spring.getForce(body1)!.Value;
            Assert.AreEqual(appliedForceBody1.force, new Vector(1466.4509f, 307.98734f));
        }
        [Test] 
        public void FreeFallAccuracy() {
            var body = BoxBody.createBox(new Vector(10, 10), 200, new Vector(0, 0), new Vector(0, 0));
            var gravity = Physics.createGravity(50);
            var physics = new Physics(
                new List<RigidBody> { body },
                new List<ForceProvider> { gravity },
                0.01f,
                AdvanceMode.Smart
            );
            var initialEnergy = physics.totalEnergy();
            var steps = 1000;
            for (var i = 0; i < steps; i += 1) {
                physics.advance();
            }
            physics.catchUpPositions();
            Assert.AreEqual(physics.totalEnergy() - initialEnergy, -12.25f, delta); //12.5 in js version
            var totalTime = steps * physics.step;
            Assert.AreEqual(body.position.Y - gravity.acceleration.Y * totalTime * totalTime / 2, 2.50125, delta);
        }
        
        [Test] 
        public void SpringPendulumAccuracy() {
            var body = BoxBody.createBox(new Vector(10, 10), 100, new Vector(0, 400), new Vector(0, 10));
            var gravity = Physics.createGravity(100);
            var spring = Physics.createFixedSpring(25, new Vector(0, 0), body, new Vector(0, 0));
            var physics = new Physics(
                new List<RigidBody> { body },
                new List<ForceProvider> { spring, gravity },
                0.01f,
                AdvanceMode.Smart
            );
            var initialEnergy = physics.totalEnergy();
            var steps = 1000;
            for (var i = 0; i < steps; i += 1) {
                physics.advance();
            }
            physics.catchUpPositions();
            Assert.AreEqual(physics.totalEnergy() - initialEnergy, 0.125d, delta); //0.0025572783779352903 in JS version
        }
    }
}