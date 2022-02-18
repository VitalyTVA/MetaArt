using MetaArt.D3;
using NUnit.Framework;
using System.Numerics;
using System.Reflection;
using static MetaArt.D3.MathFEx;
using static MetaArt.Sketches.Tests.TestExtensions;

namespace MetaArt.Sketches.Tests {
    [TestFixture]
    public class D3Tests {
        [Test]
        public void Scene_Visibility1() {
            var z = -10;
            var side = 60;
            var model = new Model<int>(new[] {
                new Vector3(side, side, z),
                new Vector3(side, -side, z),
                new Vector3(-side, -side, z),
                new Vector3(-side, side, z),
            },
            new Quad<int>[] {
                (3, 2, 1, 0, 100),
            });
            var scene = new Scene<int>(model);

            var c = new Camera(new Vector3(0, 0, -160), Quaternion.Identity, 100);
            var (i1, i2, i3, i4, val, vertices) = scene.GetQuads(c).Single();
            AssertVector(new Vector3(-40f, 40f, 150f), vertices[i1]);
            AssertVector(new Vector3(-40f, -40f, 150f), vertices[i2]);
            AssertVector(new Vector3(40f, -40f, 150f), vertices[i3]);
            AssertVector(new Vector3(40f, 40f, 150f), vertices[i4]);
            Assert.AreEqual(100, val);

            c = new Camera(new Vector3(0, 0, 160), Quaternion.CreateFromAxisAngle(Vector3.UnitY, PI), 100);
            Assert.False(scene.GetQuads(c).Any());

            model.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitY, PI));
            (i1, i2, i3, i4, val, vertices) = scene.GetQuads(c).Single();
            AssertVector(new Vector3(-40f, 40f, 150f), vertices[i1]);
            Assert.AreEqual(100, val);

            model.Scale = new Vector3(2, 3, .5f);
            (i1, i2, i3, i4, val, vertices) = scene.GetQuads(c).Single();
            AssertVector(new Vector3(-77.41933f, 116.129005f, 155f), vertices[i1]);
        }

        [Test]
        public void Scene_Visibility2() {
            var model = new Model<int>(new[] {
                new Vector3(20, 10, -60),
                new Vector3(20, -10, -60),
                new Vector3(30, -10, -40),
                new Vector3(30, 10, -40),
            },
            new Quad<int>[] {
                (3, 2, 1, 0, 100),
            });
            var scene = new Scene<int>(model);

            var c = new Camera(new Vector3(0, 0, -100), Quaternion.Identity, 10);
            Assert.False(scene.GetQuads(c).Any());

            model.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitY, -PI / 1000));
            scene.GetQuads(c).Single();

            model.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitY, PI * 2 / 1000));
            Assert.False(scene.GetQuads(c).Any());

            c = new Camera(new Vector3(0, 0, 100), Quaternion.CreateFromAxisAngle(Vector3.UnitY, PI), 10);
            model.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitY, PI));
            Assert.False(scene.GetQuads(c).Any());

            model.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitY, -PI * 2 / 1000));
            scene.GetQuads(c).Single();
        }

        [Test]
        public void Scene_TwoQuads() {
            var z = -10;
            var side = 60;
            var model = new Model<int>(new[] {
                new Vector3(side, side, z),
                new Vector3(side, -side, z),
                new Vector3(-side, -side, z),
                new Vector3(-side, side, z),

                new Vector3(side, side, z + 10),
                new Vector3(side, -side, z + 10),
                new Vector3(-side, -side, z + 10),
                new Vector3(-side, side, z + 10),
            },
            new Quad<int>[] {
                (7, 6, 5, 4, 200),
                (3, 2, 1, 0, 100),
            });
            var scene = new Scene<int>(model);

            var c = new Camera(new Vector3(0, 0, -160), Quaternion.Identity, 100);
            var vals = scene.GetQuads(c).Select(x => x.Item5);
            CollectionAssert.AreEqual(new[] { 200, 100 }, vals);

            c = new Camera(new Vector3(0, 0, 160), Quaternion.CreateFromAxisAngle(Vector3.UnitY, PI), 100);
            Assert.False(scene.GetQuads(c).Any());

            model.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitY, PI));
            vals = scene.GetQuads(c).Select(x => x.Item5);
            CollectionAssert.AreEqual(new[] { 200, 100 }, vals);
        }
        [Test]
        public void Scene_TwoModels() {
            var z = -10;
            var side = 60;
            var model1 = new Model<int>(new[] {
                new Vector3(side, side, z + 10),
                new Vector3(side, -side, z + 10),
                new Vector3(-side, -side, z + 10),
                new Vector3(-side, side, z + 10),
            },
            new Quad<int>[] {
                (3, 2, 1, 0, 200),
            });
            var model2 = new Model<int>(new[] {
                new Vector3(side, side, z),
                new Vector3(side, -side, z),
                new Vector3(-side, -side, z),
                new Vector3(-side, side, z),
            },
            new Quad<int>[] {
                (3, 2, 1, 0, 100),
            });
            var scene = new Scene<int>(model1, model2);

            var c = new Camera(new Vector3(0, 0, -160), Quaternion.Identity, 100);
            var vals = scene.GetQuads(c).Select(x => x.Item5);
            CollectionAssert.AreEqual(new[] { 200, 100 }, vals);
        }

        [Test]
        public void Scene_TwoModels_ReverseOrder() {
            var z = -10;
            var side = 60;
            var model1 = new Model<int>(new[] {
                new Vector3(side, side, z + 10),
                new Vector3(side, -side, z + 10),
                new Vector3(-side, -side, z + 10),
                new Vector3(-side, side, z + 10),
            },
            new Quad<int>[] {
                (3, 2, 1, 0, 200),
            });
            var model2 = new Model<int>(new[] {
                new Vector3(side, side, z),
                new Vector3(side, -side, z),
                new Vector3(-side, -side, z),
                new Vector3(-side, side, z),
            },
            new Quad<int>[] {
                (3, 2, 1, 0, 100),
            });
            var scene = new Scene<int>(model2, model1);

            var c = new Camera(new Vector3(0, 0, -160), Quaternion.Identity, 100);
            var vals = scene.GetQuads(c).Select(x => x.Item5);
            CollectionAssert.AreEqual(new[] { 200, 100 }, vals);
        }

        [Test]
        public void Scene_TrivialOrder() {
            AssertOrder(
                new[] { 1, 0 }, 
                CreateScene(
                    QuadXZ((-20, 100), (20, 100)),
                    QuadXZ((-20, 110), (20, 110))
                )
            );
            AssertOrder(
                new[] { 2, 0, 1 },
                CreateScene(
                    QuadXZ((-20, 110), (20, 110)),
                    QuadXZ((-20, 100), (20, 100)),
                    QuadXZ((-20, 120), (20, 120))
                )
            );
        }

        [Test]
        public void Scene_ZOverlap_NoXOverlap1() {
            AssertOrder(
                new[] { 0, 1 }, 
                CreateScene(
                    QuadXZ((-20, 120), (-10, 80)),
                    QuadXZ((10, 110), (20, 90))
                )
            );
        }
        [Test]
        public void Scene_ZOverlap2_NoXOverlap2() {
            AssertOrder(
                new[] { 0, 1 },
                CreateScene(
                    QuadXZ_N((-10, 80), (-20, 120)),
                    QuadXZ_N((20, 90), (10, 110))
                )
            );
        }
        [Test]
        public void Scene_ZOverlap_NoXOverlap3() {
            AssertOrder(
                new[] { 0, 1, 2 },
                CreateScene(
                    QuadXZ((-20, 120), (-10, 80)),
                    QuadXZ((10, 110), (20, 90)),
                    QuadXZ((40, 110), (50, 90))
                )
            );
        }
        [Test]
        public void Scene_ZOverlap_NoXOverlap4() {
            AssertOrder(
                new[] { 0, 1 },
                CreateScene(
                    QuadXZ((-20, 120), (0, 100)),
                    QuadXZ((0, 100), (20, 120))
                )
            );
        }

        [Test]
        public void Scene_ZOverlap_XOverlap_NoYOverlap1() {
            AssertOrder(
                new[] { 0, 1 },
                CreateScene(
                    QuadYZ_N((-10, 80), (-20, 120)),
                    QuadYZ_N((20, 90), (10, 110))
                )
            );
        }

        [Test]
        public void Scene_ZOverlap_XOverlap_NoYOverlap2() {
            AssertOrder(
                new[] { 0, 1 },
                CreateScene(
                    QuadYZ((-20, 120), (-10, 80)),
                    QuadYZ((10, 110), (20, 90))
                )
            );
        }

        [Test]
        public void Scene_ZOverlap2_XOverlap_ZOverlap_PBeyondQPlane() {
            AssertOrder(
                new[] { 1, 0 },
                CreateScene(
                    QuadXZ((-20, 120), (20, 80)),
                    QuadXZ((0, 110), (20, 130))
                )
            );
        }

        [Test]
        public void Scene_ZOverlap2_XOverlap_ZOverlap_PBeyondQPlane2() {
            var side = 100;
            var model = new Model<int>(new[] {
                new Vector3(-side, -side, side),
                new Vector3(-side, side, side),

                new Vector3(side, side, -side),
                new Vector3(side, -side, -side),
                new Vector3(-side, -side, -side),
                new Vector3(-side, side, -side),
            },
            new Quad<int>[] {
                (5, 4, 3, 2, 0),
                (5, 4, 0, 1, 1),
            });

            AssertOrder(
                new[] { 1, 0 },
                new Scene<int>(model),
                camera: new YawPitchContoller(0.08f, 0.025f).CreateCamera(600, 400)
            );
        }


        [Test]
        public void Scene_ZOverlap2_XOverlap_ZOverlap_PBeyondQPlane3() {
            AssertOrder(
                new[] { 0, 1 },
                CreateScene(
                    QuadXZ((-50, 100), (-50, 149)),
                    QuadXZ((-49, 100), (-49, 150))
                )
            );
        }

        [Test]
        public void Scene_ZOverlap2_XOverlap_ZOverlap_PBeyondQPlane4() {
            AssertOrder(
                new[] { 0, 1 },
                CreateScene(
                    QuadXZ((-50, 100), (-50, 140)),
                    QuadXZ((-49, 100), (-49, 150))
                )
            );
        }

        [Test]
        public void Scene_ZOverlap2_XOverlap_ZOverlap_QInFrontOfPPlane() {
            AssertOrder(
                new[] { 0, 1 },
                CreateScene(
                    QuadXZ((-20, 120), (20, 80)),
                    QuadXZ((-20, 70), (0, 90))
                )
            );
        }

        [Test]
        public void Scene_ZOverlap2_XOverlap_ZOverlap_QInFrontOfPPlane2() {
            var side = 100;
            var model = new Model<int>(new[] {
                new Vector3(side, side, side),
                new Vector3(side, -side, side),
                new Vector3(-side, -side, side),
                new Vector3(-side, side, side),

                new Vector3(side, side, -side),
                new Vector3(-side, side, -side),
            },
            new Quad<int>[] {
                (3, 2, 1, 0, 0),
                (3, 5, 4, 0, 1),
            });

            AssertOrder(
                new[] { 0, 1 },
                new Scene<int>(model),
                camera: new YawPitchContoller(0.125f, -0.515f).CreateCamera(600, 400)
            );
        }

        [Test]
        public void Scene_ZOverlap2_XOverlap_ZOverlap_QInFrontOfPPlane3() {
            var obj =
@"o X
v 4.000000 1.000000 -1.000000
v 4.000000 1.000000 1.000000
v 2.000000 1.000000 -1.000000
v 2.000000 -1.000000 -1.000000
v 2.000000 1.000000 1.000000
v 2.000000 -1.000000 1.000000
v 1.000000 4.000000 -1.000000
v 1.000000 2.000000 -1.000000
v 1.000000 4.000000 1.000000
v 1.000000 2.000000 1.000000
v -1.000000 2.000000 -1.000000
v -1.000000 2.000000 1.000000
f 2 5 3 1
f 4 3 5 6
f 11 8 10 12
f 8 7 9 10";
            AssertOrder(
                new[] { 2, 3, 0, 1 },
                CreateScene(obj, scale: 50),
                camera: new YawPitchContoller(yaw: 2.415f, pitch: 0.53f).CreateCamera(600, 400)
            );
        }

        [Test]
        public void Scene_ZOverlap2_XOverlap_ZOverlap_QInFrontOfPPlane4() {
            var obj =
@"o X
v 1 4 -1
v 1 2 -1
v 1 4 1
v 1 2 1
v -1 4 -1
v -1 2 -1
v -1 4 1
v -1 2 1
v 1 1 -4
v 1 -1 -4
v 1 1 -2
v 1 -1 -2
v -1 1 -4
v -1 -1 -4
v -1 1 -2
v -1 -1 -2
s off
f 6 2 4 8
f 2 1 5 6
f 11 15 13 9
f 12 11 15 16";
            AssertOrder(
                new[] { 1, 2, 0, 3 },
                CreateScene(obj, scale: 50),
                camera: new YawPitchContoller(yaw: -0.9099997f, pitch: 0.7853982f).CreateCamera(600, 400)
            );
        }

        [Test]
        public void Scene_ZOverlap2_XOverlap_ZOverlap_QInFrontOfPPlane5() {
            var obj =
@"o X
v 1 4 -1
v 1 2 -1
v 1 4 1
v 1 2 1
v -1 4 -1
v -1 2 -1
v -1 4 1
v -1 2 1
v 1 1 -4
v 1 -1 -4
v 1 1 -2
v 1 -1 -2
v -1 1 -4
v -1 -1 -4
v -1 1 -2
v -1 -1 -2
s off
f 2 1 5 6
f 12 11 15 16";
            AssertOrder(
                new[] { 0, 1 },
                CreateScene(obj, scale: 50),
                camera: new YawPitchContoller(yaw: -0.9099997f, pitch: 0.7853982f).CreateCamera(600, 400)
            );
        }

        [Test]
        public void Scene_ZOverlap2_XOverlap_ZOverlap_QInFrontOfPPlane6() {
            var obj =
@"o X_Cube
v 4.000000 1.000000 -1.000000
v 4.000000 -1.000000 -1.000000
v 4.000000 1.000000 1.000000
v 4.000000 -1.000000 1.000000
v 2.000000 1.000000 -1.000000
v 2.000000 -1.000000 -1.000000
v 2.000000 1.000000 1.000000
v 2.000000 -1.000000 1.000000
s off
f 4 3 7 8
f 6 2 4 8";
            Scene<int> scene = CreateScene(obj, scale: 50);
            foreach(var item in scene.GetModels()) {
                item.Rotation = new Quaternion(0.41467953f, 0.323241f, 0.5681285f, -0.6330752f);
            }
            AssertOrder(
                new int[] { },
                scene,
                camera: new YawPitchContoller(yaw: 2.04999971f, pitch: -0.759999931f).CreateCamera(600, 400)
            );
        }

        [Test]
        public void Scene_SwapPlanes1() {
            AssertOrder(
                new[] { 1, 0 },
                CreateScene(
                    QuadXZ((-20, 120), (10, 90)),
                    QuadXZ((0, 110), (10, 100))
                )
            );
        }

        [Test]
        public void Scene_SwapPlanes2() {
            AssertOrder(
                new[] { 1, 0, 2 },
                CreateScene(
                    QuadXZ((-20, 120), (10, 90)),
                    QuadXZ((0, 110), (10, 100)),
                    QuadXZ((-20, 80), (20, 80))
                )
            );
        }

        [Test]
        public void Scene_SwapPlanes3() {
            AssertOrder(
                new[] { 2, 1, 0 },
                CreateScene(
                    QuadXZ((-20, 120), (20, 80)),
                    QuadXZ((-18, 119), (22, 81)),
                    QuadXZ((-16, 118), (24, 82))
                )
            );
        }

        [Test]
        public void Scene_SwapPlanes4() {
            AssertOrder(
                new[] { 2, 1, 0, 3 },
                CreateScene(
                    QuadXZ((-20, 120), (20, 80)),
                    QuadXZ((-18, 119), (20, 80)),
                    QuadXZ((-16, 118), (20, 80)),
                    QuadXZ((-20, 70), (20, 70))
                )
            );
        }

        [Test]
        public void Scene_SwapPlanes5() {
            AssertOrder(
                new[] { 1, 0 },
                CreateScene(
                    (new Vector3(-20, -30, 110), new Vector3(-19, -30, 110), new Vector3(-19, 30, 90), new Vector3(-20, 30, 90)),
                    (new Vector3(-30, 20, 110), new Vector3(-30, 19, 110), new Vector3(30, 19, 90), new Vector3(30, 20, 90))
                )
            );
        }

        [Test]
        public void Scene_SwapPlanes6() {
            AssertOrder(
                new[] { 0, 1 },
                CreateScene(
                    (new Vector3(-30, 20, 110), new Vector3(-30, 19, 110), new Vector3(30, 19, 90), new Vector3(30, 20, 90)),
                    (new Vector3(-20, -30, 110), new Vector3(-19, -30, 110), new Vector3(-19, 30, 90), new Vector3(-20, 30, 90))
                )
            );
        }

        [Test]
        public void Scene_SwapCycle() {
            var scene = CreateScene(
                (new Vector3(-20, -30, 110), new Vector3(-19, -30, 110), new Vector3(-19, 30, 90), new Vector3(-20, 30, 90)),
                (new Vector3(-30, 20, 110), new Vector3(-30, 19, 110), new Vector3(30, 19, 90), new Vector3(30, 20, 90)),
                (new Vector3(20, 30, 110), new Vector3(21, 30, 110), new Vector3(-30, -31, 90), new Vector3(-30, -30, 90))
            );
            var e = Assert.Throws<InvalidOperationException>(() => scene.GetQuads(new Camera(new Vector3(0, 0, 0), Quaternion.Identity, 50)).ToArray());
            Assert.AreEqual("Cycle detected: (0, 1)", e!.Message);
        }

        [Test]
        public void Scene_VertexTouchAndIntersection() {
            var obj =
@"o torus
v 1 0.25 0
v 0.75 0 0
v 1 -0.25 0
v -0.625 0 -1.08253200000000005
v -0.5 0.25 -0.866026
v -0.375 0 -0.64951899999999992
v -0.5 -0.25 -0.866026
v -0.625 0 1.08253200000000005
v -0.5 0.25 0.866026
v -0.375 0 0.64951899999999992
v -0.5 -0.25 0.866026
s off
f 1 5 6 2
f 10 2 3 11";
            Scene<int> scene = CreateScene(obj, scale: 50);
            foreach(var item in scene.GetModels()) {
                item.Rotation = new Quaternion(0.44357088f, 0.8750796f, 0.121156156f, 0.15099645f);
            }
            AssertOrder(
                new int[] { 1, 0 },
                scene,
                camera: new YawPitchContoller(yaw: -4.435f, pitch: -0.014601809f).CreateCamera(600, 400)
            );
        }

        static (Vector3, Vector3, Vector3, Vector3) QuadYZ((float y, float z) v1, (float y, float z) v2) {
            return (
                new Vector3(10, v1.y, v1.z),
                new Vector3(10, v2.y, v2.z),
                new Vector3(-10, v2.y, v2.z),
                new Vector3(-10, v1.y, v1.z)
            );
        }

        static (Vector3, Vector3, Vector3, Vector3) QuadYZ_N((float y, float z) v1, (float y, float z) v2) {
            return (
                new Vector3(-10, v1.y, v1.z),
                new Vector3(-10, v2.y, v2.z),
                new Vector3(10, v2.y, v2.z),
                new Vector3(10, v1.y, v1.z)
            );
        }

        static (Vector3, Vector3, Vector3, Vector3) QuadXZ((float x, float z) v1, (float x, float z) v2) {
            return (
                new Vector3(v1.x, -10, v1.z),
                new Vector3(v2.x, -10, v2.z),
                new Vector3(v2.x, 10, v2.z),
                new Vector3(v1.x, 10, v1.z)
            );
        }
        static (Vector3, Vector3, Vector3, Vector3) QuadXZ_N((float x, float z) v1, (float x, float z) v2) {
            return (
                new Vector3(v1.x, 10, v1.z),
                new Vector3(v2.x, 10, v2.z),
                new Vector3(v2.x, -10, v2.z),
                new Vector3(v1.x, -10, v1.z)
            );
        }
        static void AssertOrder(int[] epxectedOrder, Scene<int> scene, Camera? camera= null) {
            camera = camera ?? new Camera(new Vector3(0, 0, 0), Quaternion.Identity, 50);
            var actual = scene.GetQuads(camera).Select(x => x.Item5).ToArray();
            Assert.AreEqual(epxectedOrder, actual);
        }
        static Scene<int> CreateScene(params (Vector3, Vector3, Vector3, Vector3)[] quads) {
            var vertices = quads
                .SelectMany(x => new[] { x.Item1, x.Item2, x.Item3, x.Item4 })
                .ToArray();
            var quads_ = Enumerable.Range(0, quads.Length)
                .Select(i => new Quad<int>(4 * i, 4 * i + 1, 4 * i + 2, 4 * i + 3, i))
                .ToArray();
            return new Scene<int>(new Model<int>(vertices, quads_));
        } 

        [Test]
        public void ProjectPoint() {
            var c = new Camera(new Vector3(0, 0, -160), Quaternion.Identity, 100);
            AssertVector(new Vector2(-20, 40), c.ProjectPoint(new Vector3(-30, 60, -10)));
            AssertVector(new Vector3(-30f, 60f, 150f), c.TranslatePoint(new Vector3(-30, 60, -10)));
            AssertVector(new Vector3(-30f, 60f, 170f), c.TranslatePoint(new Vector3(-30, 60, 10)));

            c = new Camera(new Vector3(10, 20, -160), Quaternion.Identity, 100);
            AssertVector(new Vector2(-20, 40), c.ProjectPoint(new Vector3(-30 + 10, 60 + 20, -10)));

            c = new Camera(new Vector3(-160, 0, 0), Quaternion.CreateFromAxisAngle(Vector3.UnitY, -PI/2), 100);
            AssertVector(new Vector2(20, 40), c.ProjectPoint(new Vector3(-10, 60, -30)));
            AssertVector(new Vector2(-40, -20), c.ProjectPoint(new Vector3(-10, -30, 60)));

            c = new Camera(new Vector3(0, 0, -160), Quaternion.CreateFromAxisAngle(Vector3.UnitZ, PI / 2), 100);
            AssertVector(new Vector2(-40, -20), c.ProjectPoint(new Vector3(-30, 60, -10)));

            c = new Camera(new Vector3(0, 0, -160), Quaternion.CreateFromAxisAngle(Vector3.UnitZ, -PI / 2) * 10, 100);
            AssertVector(new Vector2(40, 20), c.ProjectPoint(new Vector3(-30, 60, -10)));
        }

        [Test]
        public void GetNormal() {
            Assert.AreEqual(new Vector3(2400, 2000, 1600), Extensions.GetNormal(new Vector3(10, 20, 30), new Vector3(-20, 40, 50), new Vector3(60, 40, -70)));
        }

        [Test]
        public void IsVisible() {
            var c = new Camera(new Vector3(0, 0, -100), Quaternion.Identity, 40);
            //Assert.True(c.IsVisible(new Vector3(30, 0, -40), new Vector3(20, 0, -10)));
            Assert.False(c.IsVisible(new Vector3(30, 0, -40), new Vector3(20, 0, -10 + 1)));
            Assert.True(c.IsVisible(new Vector3(30, 0, -40), new Vector3(20, 0, -10 - 1)));
        }

        [Test]
        public void RotateModel() {
            var m = new Model<D3.VoidType>(new Vector3[] { }, new Quad<D3.VoidType>[] { });
            var c = new Camera(new Vector3(10, 20, 30), Quaternion.CreateFromYawPitchRoll(1, 2, 3), 40);

            Extensions.Rotate(m, c, 0, 0);
            AssertQuaternion(Quaternion.Identity, m.Rotation);

            Extensions.Rotate(m, c, 5, 0);
            AssertQuaternion(new Quaternion(0.0014680151f, -0.010298486f, 0.022730066f, 0.9996875f), m.Rotation);

            Extensions.Rotate(m, c, 0, 10);
            AssertQuaternion(new Quaternion(-0.02099153f, -0.051482804f, 0.0054865545f, 0.9984382f), m.Rotation);

            Extensions.Rotate(m, c, -2, -3);
            AssertQuaternion(new Quaternion(-0.015287506f, -0.034826715f, 0.0017010788f, 0.99927497f), m.Rotation);
        }

        [Test]
        public void ShepereCameraControllerTest() {
            var controller = new YawPitchContoller();

            var c = controller.CreateCamera(600, 400);
            AssertVector(new Vector3(0f, 0f, -600f), c.Location);
            AssertQuaternion(Quaternion.Identity, c.Rotation);
            AssertVector(new Vector3(0, 0, 1), controller.GetDirection());

            controller.Yaw(1);
            c = controller.CreateCamera(600, 400);
            AssertVector(new Vector3(504.8826f, 0f, -324.18137f), c.Location);
            AssertQuaternion(new Quaternion(0f, 0.47942555f, 0f, 0.87758255f), c.Rotation);
            AssertVector(new Vector3(-0.841471f, 0f, 0.5403023f), controller.GetDirection());

            controller.Pitch(-0.5f);
            c = controller.CreateCamera(600, 400);
            AssertVector(new Vector3(443.07614f, 287.65533f, -284.4959f), c.Location);
            AssertQuaternion(new Quaternion(-0.2171174f, 0.46452138f, -0.11861178f, 0.8503006f), c.Rotation);
            AssertVector(new Vector3(-0.73846024f, -0.47942555f, 0.47415984f), controller.GetDirection());

            controller.Pitch(-5f);
            c = controller.CreateCamera(600, 400);
            AssertVector(new Vector3(357.0059f, 424.26407f, -229.23085f), c.Location);
            AssertQuaternion(new Quaternion(-0.33583632f, 0.44293144f, -0.18346822f, 0.8107805f), c.Rotation);
            AssertVector(new Vector3(-0.5950098f, -0.70710677f, 0.3820514f), controller.GetDirection());

            controller.Pitch(15f);
            c = controller.CreateCamera(600, 400);
            AssertVector(new Vector3(357.0059f, -424.26407f, -229.23085f), c.Location);
            AssertQuaternion(new Quaternion(0.33583632f, 0.44293144f, 0.18346822f, 0.8107805f), c.Rotation);
            AssertVector(new Vector3(-0.5950098f, 0.70710677f, 0.3820514f), controller.GetDirection());
        }

        [Test]
        public void RangesOverlappring() {
            Assert.True(RangesAreApart((1, 2), (3, 4)));
            Assert.True(RangesAreApart((3, 4), (1, 2)));

            Assert.True(RangesAreApart((1, 3), (0, 1)));
            Assert.False(RangesAreApart((1, 3), (0, 1.1f)));

            Assert.True(RangesAreApart((0, 1), (1, 3)));
            Assert.False(RangesAreApart((0, 1.1f), (1, 3)));
        }

        [Test]
        public void PointsOnSameSideOfLine1() {
            Assert.True(Extensions.PointsOnSameSideOfLine(
                (new Vector2(1, 2), new Vector2(3, 3)),
                (new Vector2(1, 3), new Vector2(2, 3), new Vector2(3, 4))
            ));
            Assert.False(Extensions.PointsOnSameSideOfLine(
                (new Vector2(1, 2), new Vector2(3, 3)),
                (new Vector2(1, 3), new Vector2(2, 3), new Vector2(3, 2))
            ));
            Assert.False(Extensions.PointsOnSameSideOfLine(
                (new Vector2(1, 2), new Vector2(3, 3)),
                (new Vector2(1, 3), new Vector2(3, 2), new Vector2(2, 3))
            ));
            Assert.True(Extensions.PointsOnSameSideOfLine(
                (new Vector2(1, 2), new Vector2(3, 3)),
                (new Vector2(1, 1), new Vector2(2, 2), new Vector2(3, 2))
            ));
            Assert.False(Extensions.PointsOnSameSideOfLine(
                (new Vector2(1, 2), new Vector2(3, 3)),
                (new Vector2(1, 1), new Vector2(2, 2), new Vector2(3, 3))
            ));
        }
        [Test]
        public void PointsInside() {
            var quad = (new Vector2(1, 2), new Vector2(3, 3), new Vector2(2, 4), new Vector2(1, 4));
            Assert.True(Extensions.PointInside(quad, new Vector2(2, 3)));
            Assert.False(Extensions.PointInside(quad, new Vector2(1, 3)));

            Assert.False(Extensions.PointInside(quad, new Vector2(0, 3)));
            Assert.False(Extensions.PointInside(quad, new Vector2(1, 5)));
            Assert.False(Extensions.PointInside(quad, new Vector2(3, 4)));
            Assert.False(Extensions.PointInside(quad, new Vector2(2, 2)));

            quad = (new Vector2(1, 2), new Vector2(3, 3), new Vector2(2, 4), new Vector2(2, 4));

            Assert.True(Extensions.PointInside(quad, new Vector2(2, 3)));

            Assert.False(Extensions.PointInside(quad, new Vector2(1, 3)));
            Assert.False(Extensions.PointInside(quad, new Vector2(0, 3)));
            Assert.False(Extensions.PointInside(quad, new Vector2(1, 5)));
            Assert.False(Extensions.PointInside(quad, new Vector2(3, 4)));
            Assert.False(Extensions.PointInside(quad, new Vector2(2, 2)));
        }

        [Test]
        public void Intersects1() {
            var quad = (new Vector2(1, 2), new Vector2(3, 3), new Vector2(2, 4), new Vector2(1, 4));
            Assert.True(TestExtensions.Intersects(quad,
                (new Vector2(2, 3), new Vector2(3, 1), new Vector2(4, 3), new Vector2(4, 4))));
            Assert.True(TestExtensions.Intersects(quad,
                (new Vector2(2, 3), new Vector2(3, 1), new Vector2(4, 3), new Vector2(4, 3))));
            Assert.True(TestExtensions.Intersects(quad,
                (new Vector2(3, 1), new Vector2(4, 3), new Vector2(4, 4), new Vector2(2, 3))));
            Assert.True(TestExtensions.Intersects(quad,
                (new Vector2(4, 3), new Vector2(4, 4), new Vector2(2, 3), new Vector2(3, 1))));
            Assert.True(TestExtensions.Intersects(quad,
                (new Vector2(4, 4), new Vector2(2, 3), new Vector2(3, 1), new Vector2(4, 3))));

            Assert.False(TestExtensions.Intersects(quad,
                (new Vector2(3, 2), new Vector2(3, 1), new Vector2(4, 3), new Vector2(4, 4))));
        }

        [Test]
        public void Intersects2() {
            var quad = (new Vector2(2, 1), new Vector2(4, 1), new Vector2(4, 5), new Vector2(3, 5));
            Assert.True(TestExtensions.Intersects(quad,
                (new Vector2(1, 2), new Vector2(3, 3), new Vector2(2, 4), new Vector2(1, 4))));
            Assert.True(TestExtensions.Intersects(quad,
                (new Vector2(3, 3), new Vector2(2, 4), new Vector2(1, 4), new Vector2(1, 2))));
            Assert.True(TestExtensions.Intersects(quad,
                (new Vector2(2, 4), new Vector2(1, 4), new Vector2(1, 2), new Vector2(3, 3))));
            Assert.True(TestExtensions.Intersects(quad,
                (new Vector2(1, 4), new Vector2(1, 2), new Vector2(3, 3), new Vector2(2, 4))));

            Assert.False(TestExtensions.Intersects(quad,
                (new Vector2(1, 2), new Vector2(2, 3), new Vector2(2, 4), new Vector2(1, 4))));
        }

        [Test]
        public void Intersects3() {
            var quad = (new Vector2(2, 1), new Vector2(4, 1), new Vector2(4, 5), new Vector2(3, 5));
            Assert.True(TestExtensions.Intersects((new Vector2(1, 2), new Vector2(3, 3), new Vector2(2, 4), new Vector2(1, 4)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(3, 3), new Vector2(2, 4), new Vector2(1, 4), new Vector2(1, 2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(2, 4), new Vector2(1, 4), new Vector2(1, 2), new Vector2(3, 3)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(1, 4), new Vector2(1, 2), new Vector2(3, 3), new Vector2(2, 4)), quad));
            Assert.False(TestExtensions.Intersects((new Vector2(1, 2), new Vector2(2, 3), new Vector2(2, 4), new Vector2(1, 4)), quad));
        }
        [Test]
        public void Intersects4() {
            var quad = (new Vector2(0, 2), new Vector2(2, 2), new Vector2(2, 0), new Vector2(0, 0));
            Assert.False(TestExtensions.Intersects((new Vector2(2, 1), new Vector2(3, 0), new Vector2(4, 1), new Vector2(3, 2)), quad));
        }

        [Test]
        public void Intersects5() {
            var quad = (new Vector2(0, 0), new Vector2(4, 0), new Vector2(4, 1), new Vector2(0, 1));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2))));
            Assert.True(TestExtensions.Intersects((new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2)), quad));

            quad = (new Vector2(4, 0), new Vector2(4, 1), new Vector2(0, 1), new Vector2(0, 0));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2))));
            Assert.True(TestExtensions.Intersects((new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2)), quad));

            quad = (new Vector2(4, 1), new Vector2(0, 1), new Vector2(0, 0), new Vector2(4, 0));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2))));
            Assert.True(TestExtensions.Intersects((new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2)), quad));

            quad = (new Vector2(0, 1), new Vector2(0, 0), new Vector2(4, 0), new Vector2(4, 1));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2))));
            Assert.True(TestExtensions.Intersects((new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2)), quad));
        }

        [Test]
        public void Intersects6() {
            var quad = (new Vector2(0, 0), new Vector2(4, 0), new Vector2(4, 1), new Vector2(4, 1));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2), new Vector2(2, 2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, 2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2), new Vector2(1, -2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -2))));
            Assert.True(TestExtensions.Intersects((new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2), new Vector2(2, 2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, 2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2), new Vector2(1, -2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -2)), quad));

            quad = (new Vector2(4, 0), new Vector2(4, 1), new Vector2(0, 1), new Vector2(0, 1));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2), new Vector2(2, 2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, 2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2), new Vector2(1, -2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -2))));
            Assert.True(TestExtensions.Intersects((new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2), new Vector2(2, 2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, 2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2), new Vector2(1, -2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -2)), quad));

            quad = (new Vector2(4, 1), new Vector2(0, 1), new Vector2(0, 0), new Vector2(0, 0));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2), new Vector2(2, 2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, 2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2), new Vector2(1, -2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -2))));
            Assert.True(TestExtensions.Intersects((new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2), new Vector2(2, 2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, 2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2), new Vector2(1, -2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -2)), quad));

            quad = (new Vector2(0, 1), new Vector2(0, 0), new Vector2(4, 0), new Vector2(4, 0));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2), new Vector2(2, 2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, 2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2), new Vector2(1, -2))));
            Assert.True(TestExtensions.Intersects(quad, (new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -2))));
            Assert.True(TestExtensions.Intersects((new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, 2), new Vector2(2, 2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(2, -2), new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, 2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(2, 2), new Vector2(1, 2), new Vector2(1, -2), new Vector2(1, -2)), quad));
            Assert.True(TestExtensions.Intersects((new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -2)), quad));
        }

        [Test]
        public void Intersects7() {
            Assert.True(TestExtensions.Intersects(
                (new Vector2(0, 0), new Vector2(4, 0), new Vector2(4, 4), new Vector2(0, 4)),
                (new Vector2(1, 1), new Vector2(1, 2), new Vector2(2, 2), new Vector2(2, 1)))
            );

            Assert.AreEqual(new Vector2(1, 1), Extensions.GetQuadsIntersection(
                (new Vector2(0, 0), new Vector2(4, 0), new Vector2(4, 4), new Vector2(0, 4)),
                (new Vector2(1, 1), new Vector2(1, 2), new Vector2(2, 2), new Vector2(2, 1)))
            );
        }

        [Test]
        public void GetIntersection1() {
            var quad = (new Vector2(1, 2), new Vector2(3, 3), new Vector2(2, 4), new Vector2(1, 4));
            Assert.AreEqual(new Vector2(2, 3), Extensions.GetQuadsIntersection(quad,
                (new Vector2(2, 3), new Vector2(3, 1), new Vector2(4, 3), new Vector2(4, 4))));
        }

        [Test]
        public void GetIntersection_EdgeTouch() {
            var quad1 = (new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1));
            var quad2 = (new Vector2(0, 0), new Vector2(-1, 0), new Vector2(-1, -1), new Vector2(0, -1));
            Assert.False(TestExtensions.Intersects(quad1, quad2));
            Assert.False(TestExtensions.Intersects(quad2, quad1));
            Assert.Null(Extensions.GetQuadsIntersection(quad1, quad2));
            Assert.Null(Extensions.GetQuadsIntersection(quad2, quad1));
        }
        [Test]
        public void GetIntersection_SideTouch() {
            var quad1 = (new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1));
            var quad2 = (new Vector2(0, 0), new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, 1));
            Assert.False(TestExtensions.Intersects(quad1, quad2));
            Assert.False(TestExtensions.Intersects(quad2, quad1));
            Assert.Null(Extensions.GetQuadsIntersection(quad1, quad2));
            Assert.Null(Extensions.GetQuadsIntersection(quad2, quad1));
        }

        static Scene<int> CreateScene(string objFile, float scale) {
            using var stream = objFile.AsStream();
            var models = ObjLoader.Load(stream, info => info.Index).ToArray();
            foreach(var item in models) {
                item.Scale = new Vector3(scale, scale, scale);
            }
            return new Scene<int>(models);
        }
    }
}
