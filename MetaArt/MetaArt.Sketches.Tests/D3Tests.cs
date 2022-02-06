﻿using MetaArt.D3;
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

        static void AssertVector(Vector2 expected, Vector2 actual) {
            if(FloatEqual(expected.X, actual.X) && FloatEqual(expected.Y, actual.Y))
                return;
            Assert.Fail($"Expected {PrintVector(expected)} but was {PrintVector(actual)}");
        }
        static string PrintVector(Vector2 q) {
            return $"{q.X}f, {q.Y}f";
        }

        static void AssertVector(Vector3 expected, Vector3 actual) {
            if(FloatEqual(expected.X, actual.X) && FloatEqual(expected.Y, actual.Y) && FloatEqual(expected.Z, actual.Z))
                return;
            Assert.Fail($"Expected {PrintVector(expected)} but was {PrintVector(actual)}");
        }
        static string PrintVector(Vector3 q) {
            return $"{q.X}f, {q.Y}f, {q.Z}f";
        }


        [Test]
        public void RotateModel() {
            var m = new Model<object>(new Vector3[] { }, new (int, int, int, int, object)[] { });
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

        static void AssertQuaternion(Quaternion expected, Quaternion actual) {
            if(FloatEqual(expected.X, actual.X) && FloatEqual(expected.Y, actual.Y) && FloatEqual(expected.Z, actual.Z) && FloatEqual(expected.W, actual.W))
                return;
            Assert.Fail($"Expected {PrintQuaterion(expected)} but was {PrintQuaterion(actual)}");
        }

        static string PrintQuaterion(Quaternion q) {
            return $"{q.X}f, {q.Y}f, {q.Z}f, {q.W}f";
        }

        static bool FloatEqual(float x, float y) => Math.Abs(x - y) < delta;

        [Test]
        public void ShepereCameraControllerTest() {
            var controller = new YawPitchContoller();

            var c = controller.CreateCamera();
            AssertVector(new Vector3(0f, 0f, -600f), c.Location);
            AssertQuaternion(Quaternion.Identity, c.Rotation);
            AssertVector(new Vector3(0, 0, 1), controller.GetDirection());

            controller.Yaw(1);
            c = controller.CreateCamera();
            AssertVector(new Vector3(504.8826f, 0f, -324.18137f), c.Location);
            AssertQuaternion(new Quaternion(0f, 0.47942555f, 0f, 0.87758255f), c.Rotation);
            AssertVector(new Vector3(-0.841471f, 0f, 0.5403023f), controller.GetDirection());

            controller.Pitch(-0.5f);
            c = controller.CreateCamera();
            AssertVector(new Vector3(443.07614f, 287.65533f, -284.4959f), c.Location);
            AssertQuaternion(new Quaternion(-0.2171174f, 0.46452138f, -0.11861178f, 0.8503006f), c.Rotation);
            AssertVector(new Vector3(-0.73846024f, -0.47942555f, 0.47415984f), controller.GetDirection());

            controller.Pitch(-5f);
            c = controller.CreateCamera();
            AssertVector(new Vector3(357.0059f, 424.26407f, -229.23085f), c.Location);
            AssertQuaternion(new Quaternion(-0.33583632f, 0.44293144f, -0.18346822f, 0.8107805f), c.Rotation);
            AssertVector(new Vector3(-0.5950098f, -0.70710677f, 0.3820514f), controller.GetDirection());

            controller.Pitch(15f);
            c = controller.CreateCamera();
            AssertVector(new Vector3(357.0059f, -424.26407f, -229.23085f), c.Location);
            AssertQuaternion(new Quaternion(0.33583632f, 0.44293144f, 0.18346822f, 0.8107805f), c.Rotation);
            AssertVector(new Vector3(-0.5950098f, 0.70710677f, 0.3820514f), controller.GetDirection());
        }
    }
}