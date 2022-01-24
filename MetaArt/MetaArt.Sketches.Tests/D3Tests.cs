using MetaArt.D3;
using NUnit.Framework;
using System.Numerics;
using static MetaArt.D3.MathF;

namespace MetaArt.Sketches.Tests {
    [TestFixture]
    public class D3Tests {
        const float delta = 0.001f;
        [Test]
        public void ProjectPoint() {
            var c = new Camera(new Vector3(0, 0, -160), Quaternion.Identity, 100);
            AssertVector(new Vector2(-20, 40), c.ProjectPoint(new Vector3(-30, 60, -10)));

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

        void AssertVector(Vector2 expected, Vector2 actual) { 
            Assert.AreEqual(expected.X, actual.X, delta);
            Assert.AreEqual(expected.Y, actual.Y, delta);
        }
    }
}