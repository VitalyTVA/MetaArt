using MetaArt.D3;
using NUnit.Framework;
using System.Numerics;
using System.Reflection;
using static MetaArt.Sketches.Tests.TestExtensions;

namespace MetaArt.Sketches.Tests {
    [TestFixture]
    public class ObjLoaderTests {
        [Test]
        public void Cube() {
            var model = LoadModels<(int, int)>("cube", info => (info.Index, info.LineIndex)).Single();
            Assert.AreEqual(8, model.Vertices.Length);
            Assert.AreEqual(6, model.Quads.Length);
            AssertVector(new Vector3(1, 1, 1), model.Vertices[0]);
            AssertVector(new Vector3(1, -1, 1), model.Vertices[1]);
            AssertVector(new Vector3(1, 1, -1), model.Vertices[2]);
            AssertVector(new Vector3(1, -1, -1), model.Vertices[3]);
            AssertVector(new Vector3(-1, 1, 1), model.Vertices[4]);
            AssertVector(new Vector3(-1, -1, 1), model.Vertices[5]);
            AssertVector(new Vector3(-1, 1, -1), model.Vertices[6]);
            AssertVector(new Vector3(-1, -1, -1), model.Vertices[7]);

            AssertQuad(1, 5, 7, 3, model.Quads[0], (0, 13));
            AssertQuad(4, 3, 7, 8, model.Quads[1], (1, 14));
            AssertQuad(8, 7, 5, 6, model.Quads[2], (2, 15));
            AssertQuad(6, 2, 4, 8, model.Quads[3], (3, 16));
            AssertQuad(2, 1, 3, 4, model.Quads[4], (4, 17));
            AssertQuad(6, 5, 1, 2, model.Quads[5], (5, 18));
        }

        [Test]
        public void Triangle() {
            var model = LoadModels<VoidType>("triangle").Single();
            Assert.AreEqual(3, model.Vertices.Length);
            Assert.AreEqual(1, model.Quads.Length);
            AssertVector(new Vector3(1, 1, 1), model.Vertices[0]);
            AssertVector(new Vector3(1, -1, 1), model.Vertices[1]);
            AssertVector(new Vector3(1, 1, -1), model.Vertices[2]);

            AssertQuad(1, 2, 3, 3, model.Quads[0]);
        }

        [Test]
        public void Cubes() {
            var models = LoadModels<VoidType>("cubes").ToArray();
            Assert.AreEqual(4, models.Length);
            Assert.AreEqual(8, models[0].Vertices.Length);
            Assert.AreEqual(6, models[0].Quads.Length);
            Assert.AreEqual(8, models[1].Vertices.Length);
            Assert.AreEqual(6, models[1].Quads.Length);
            Assert.AreEqual(8, models[2].Vertices.Length);
            Assert.AreEqual(6, models[2].Quads.Length);
            Assert.AreEqual(8, models[3].Vertices.Length);
            Assert.AreEqual(6, models[3].Quads.Length);

            AssertVector(new Vector3(4, 1, 1), models[0].Vertices[0]);
            AssertVector(new Vector3(4, -1, 1), models[0].Vertices[1]);
            AssertVector(new Vector3(4, 1, -1), models[0].Vertices[2]);
            AssertVector(new Vector3(4, -1, -1), models[0].Vertices[3]);
            AssertVector(new Vector3(2, 1, 1), models[0].Vertices[4]);
            AssertVector(new Vector3(2, -1, 1), models[0].Vertices[5]);
            AssertVector(new Vector3(2, 1, -1), models[0].Vertices[6]);
            AssertVector(new Vector3(2, -1, -1), models[0].Vertices[7]);
            AssertQuad(1, 5, 7, 3, models[0].Quads[0]);
            AssertQuad(4, 3, 7, 8, models[0].Quads[1]);
            AssertQuad(8, 7, 5, 6, models[0].Quads[2]);
            AssertQuad(6, 2, 4, 8, models[0].Quads[3]);
            AssertQuad(2, 1, 3, 4, models[0].Quads[4]);
            AssertQuad(6, 5, 1, 2, models[0].Quads[5]);

            AssertVector(new Vector3(1, 4, 1), models[1].Vertices[0]);
            AssertVector(new Vector3(1, 2, 1), models[1].Vertices[1]);
            AssertVector(new Vector3(1, 4, -1), models[1].Vertices[2]);
            AssertVector(new Vector3(1, 2, -1), models[1].Vertices[3]);
            AssertVector(new Vector3(-1, 4, 1), models[1].Vertices[4]);
            AssertVector(new Vector3(-1, 2, 1), models[1].Vertices[5]);
            AssertVector(new Vector3(-1, 4, -1), models[1].Vertices[6]);
            AssertVector(new Vector3(-1, 2, -1), models[1].Vertices[7]);
            AssertQuad(1, 5, 7, 3, models[1].Quads[0]);
            AssertQuad(4, 3, 7, 8, models[1].Quads[1]);
            AssertQuad(8, 7, 5, 6, models[1].Quads[2]);
            AssertQuad(6, 2, 4, 8, models[1].Quads[3]);
            AssertQuad(2, 1, 3, 4, models[1].Quads[4]);
            AssertQuad(6, 5, 1, 2, models[1].Quads[5]);

            AssertVector(new Vector3(1, 1, 4), models[2].Vertices[0]);
            AssertVector(new Vector3(1, -1, 4), models[2].Vertices[1]);
            AssertVector(new Vector3(1, 1, 2), models[2].Vertices[2]);
            AssertVector(new Vector3(1, -1, 2), models[2].Vertices[3]);
            AssertVector(new Vector3(-1, 1, 4), models[2].Vertices[4]);
            AssertVector(new Vector3(-1, -1, 4), models[2].Vertices[5]);
            AssertVector(new Vector3(-1, 1, 2), models[2].Vertices[6]);
            AssertVector(new Vector3(-1, -1, 2), models[2].Vertices[7]);
            AssertQuad(1, 5, 7, 3, models[2].Quads[0]);
            AssertQuad(4, 3, 7, 8, models[2].Quads[1]);
            AssertQuad(8, 7, 5, 6, models[2].Quads[2]);
            AssertQuad(6, 2, 4, 8, models[2].Quads[3]);
            AssertQuad(2, 1, 3, 4, models[2].Quads[4]);
            AssertQuad(6, 5, 1, 2, models[2].Quads[5]);

            AssertVector(new Vector3(1, 1, 1), models[3].Vertices[0]);
            AssertVector(new Vector3(1, -1, 1), models[3].Vertices[1]);
            AssertVector(new Vector3(1, 1, -1), models[3].Vertices[2]);
            AssertVector(new Vector3(1, -1, -1), models[3].Vertices[3]);
            AssertVector(new Vector3(-1, 1, 1), models[3].Vertices[4]);
            AssertVector(new Vector3(-1, -1, 1), models[3].Vertices[5]);
            AssertVector(new Vector3(-1, 1, -1), models[3].Vertices[6]);
            AssertVector(new Vector3(-1, -1, -1), models[3].Vertices[7]);
            AssertQuad(1, 5, 7, 3, models[3].Quads[0]);
            AssertQuad(4, 3, 7, 8, models[3].Quads[1]);
            AssertQuad(8, 7, 5, 6, models[3].Quads[2]);
            AssertQuad(6, 2, 4, 8, models[3].Quads[3]);
            AssertQuad(2, 1, 3, 4, models[3].Quads[4]);
            AssertQuad(6, 5, 1, 2, models[3].Quads[5]);
        }

        [Test]
        public void Pentagon() {
            var obj =
    @"o X
v 0 0 0
v 1 0 0
v 2 1 0
v 3 3 0
v 4 6 0
f 1 2 3 4 5";
            var model = ObjLoader.Load(obj.AsStream(), x => x).Single();
            Assert.AreEqual(2, model.Quads.Length);
            AssertQuad(1, 2, 3, 4, model.Quads[0], (new QuadInfo(0, 7)));
            AssertQuad(1, 4, 5, 5, model.Quads[1], (new QuadInfo(1, 7)));
        }

        IEnumerable<Model<T>> LoadModels<T>(string fileName, Func<QuadInfo, T>? getValue = null) {
            var asm = Assembly.GetExecutingAssembly();
            return ObjLoader.Load<T>(asm.GetManifestResourceStream(asm.GetName().Name + $".Models.{fileName}.obj")!, getValue);
        }

        static void AssertQuad<T>(int i1, int i2, int i3, int i4, Quad<T> quad, T value = default!) {
            Assert.AreEqual(i1, quad.i1 + 1);
            Assert.AreEqual(i2, quad.i2 + 1);
            Assert.AreEqual(i3, quad.i3 + 1);
            Assert.AreEqual(i4, quad.i4 + 1);
            Assert.AreEqual(value, quad.value);
        }
    }
}
