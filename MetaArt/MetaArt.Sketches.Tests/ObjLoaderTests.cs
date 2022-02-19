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
            Assert.AreEqual(12, model.Quads.Length);
            AssertVector(new Vector3(1, 1, 1), model.Vertices[0]);
            AssertVector(new Vector3(1, -1, 1), model.Vertices[1]);
            AssertVector(new Vector3(1, 1, -1), model.Vertices[2]);
            AssertVector(new Vector3(1, -1, -1), model.Vertices[3]);
            AssertVector(new Vector3(-1, 1, 1), model.Vertices[4]);
            AssertVector(new Vector3(-1, -1, 1), model.Vertices[5]);
            AssertVector(new Vector3(-1, 1, -1), model.Vertices[6]);
            AssertVector(new Vector3(-1, -1, -1), model.Vertices[7]);

            AssertQuad(1, 5, 7, model.Quads[0], (0, 13));
            AssertQuad(1, 7, 3, model.Quads[1], (1, 13));
            AssertQuad(4, 3, 7, model.Quads[2], (2, 14));
            AssertQuad(4, 7, 8, model.Quads[3], (3, 14));
            AssertQuad(8, 7, 5, model.Quads[4], (4, 15));
            AssertQuad(8, 5, 6, model.Quads[5], (5, 15));
            AssertQuad(6, 2, 4, model.Quads[6], (6, 16));
            AssertQuad(6, 4, 8, model.Quads[7], (7, 16));
            AssertQuad(2, 1, 3, model.Quads[8], (8, 17));
            AssertQuad(2, 3, 4, model.Quads[9], (9, 17));
            AssertQuad(6, 5, 1, model.Quads[10], (10, 18));
            AssertQuad(6, 1, 2, model.Quads[11], (11, 18));
        }

        [Test]
        public void Triangle() {
            var model = LoadModels<VoidType>("triangle").Single();
            Assert.AreEqual(3, model.Vertices.Length);
            Assert.AreEqual(1, model.Quads.Length);
            AssertVector(new Vector3(1, 1, 1), model.Vertices[0]);
            AssertVector(new Vector3(1, -1, 1), model.Vertices[1]);
            AssertVector(new Vector3(1, 1, -1), model.Vertices[2]);

            AssertQuad(1, 2, 3, model.Quads[0]);
        }

        [Test]
        public void Cubes() {
            var models = LoadModels<VoidType>("cubes").ToArray();
            Assert.AreEqual(4, models.Length);
            Assert.AreEqual(8, models[0].Vertices.Length);
            Assert.AreEqual(12, models[0].Quads.Length);
            Assert.AreEqual(8, models[1].Vertices.Length);
            Assert.AreEqual(12, models[1].Quads.Length);
            Assert.AreEqual(8, models[2].Vertices.Length);
            Assert.AreEqual(12, models[2].Quads.Length);
            Assert.AreEqual(8, models[3].Vertices.Length);
            Assert.AreEqual(12, models[3].Quads.Length);

            AssertVector(new Vector3(4, 1, 1), models[0].Vertices[0]);
            AssertVector(new Vector3(4, -1, 1), models[0].Vertices[1]);
            AssertVector(new Vector3(4, 1, -1), models[0].Vertices[2]);
            AssertVector(new Vector3(4, -1, -1), models[0].Vertices[3]);
            AssertVector(new Vector3(2, 1, 1), models[0].Vertices[4]);
            AssertVector(new Vector3(2, -1, 1), models[0].Vertices[5]);
            AssertVector(new Vector3(2, 1, -1), models[0].Vertices[6]);
            AssertVector(new Vector3(2, -1, -1), models[0].Vertices[7]);
            AssertQuad(1, 5, 7, models[0].Quads[0]);
            AssertQuad(1, 7, 3, models[0].Quads[1]);
            AssertQuad(4, 3, 7, models[0].Quads[2]);
            AssertQuad(4, 7, 8, models[0].Quads[3]);
            AssertQuad(8, 7, 5, models[0].Quads[4]);
            AssertQuad(8, 5, 6, models[0].Quads[5]);
            AssertQuad(6, 2, 4, models[0].Quads[6]);
            AssertQuad(6, 4, 8, models[0].Quads[7]);
            AssertQuad(2, 1, 3, models[0].Quads[8]);
            AssertQuad(2, 3, 4, models[0].Quads[9]);
            AssertQuad(6, 5, 1, models[0].Quads[10]);
            AssertQuad(6, 1, 2, models[0].Quads[11]);


            AssertVector(new Vector3(1, 4, 1), models[1].Vertices[0]);
            AssertVector(new Vector3(1, 2, 1), models[1].Vertices[1]);
            AssertVector(new Vector3(1, 4, -1), models[1].Vertices[2]);
            AssertVector(new Vector3(1, 2, -1), models[1].Vertices[3]);
            AssertVector(new Vector3(-1, 4, 1), models[1].Vertices[4]);
            AssertVector(new Vector3(-1, 2, 1), models[1].Vertices[5]);
            AssertVector(new Vector3(-1, 4, -1), models[1].Vertices[6]);
            AssertVector(new Vector3(-1, 2, -1), models[1].Vertices[7]);
            AssertQuad(1, 5, 7, models[1].Quads[0]);
            AssertQuad(1, 7, 3, models[1].Quads[1]);
            AssertQuad(4, 3, 7, models[1].Quads[2]);
            AssertQuad(4, 7, 8, models[1].Quads[3]);
            AssertQuad(8, 7, 5, models[1].Quads[4]);
            AssertQuad(8, 5, 6, models[1].Quads[5]);
            AssertQuad(6, 2, 4, models[1].Quads[6]);
            AssertQuad(6, 4, 8, models[1].Quads[7]);
            AssertQuad(2, 1, 3, models[1].Quads[8]);
            AssertQuad(2, 3, 4, models[1].Quads[9]);
            AssertQuad(6, 5, 1, models[1].Quads[10]);
            AssertQuad(6, 1, 2, models[1].Quads[11]);

            AssertVector(new Vector3(1, 1, 4), models[2].Vertices[0]);
            AssertVector(new Vector3(1, -1, 4), models[2].Vertices[1]);
            AssertVector(new Vector3(1, 1, 2), models[2].Vertices[2]);
            AssertVector(new Vector3(1, -1, 2), models[2].Vertices[3]);
            AssertVector(new Vector3(-1, 1, 4), models[2].Vertices[4]);
            AssertVector(new Vector3(-1, -1, 4), models[2].Vertices[5]);
            AssertVector(new Vector3(-1, 1, 2), models[2].Vertices[6]);
            AssertVector(new Vector3(-1, -1, 2), models[2].Vertices[7]);
            AssertQuad(1, 5, 7, models[2].Quads[0]);
            AssertQuad(1, 7, 3, models[2].Quads[1]);
            AssertQuad(4, 3, 7, models[2].Quads[2]);
            AssertQuad(4, 7, 8, models[2].Quads[3]);
            AssertQuad(8, 7, 5, models[2].Quads[4]);
            AssertQuad(8, 5, 6, models[2].Quads[5]);
            AssertQuad(6, 2, 4, models[2].Quads[6]);
            AssertQuad(6, 4, 8, models[2].Quads[7]);
            AssertQuad(2, 1, 3, models[2].Quads[8]);
            AssertQuad(2, 3, 4, models[2].Quads[9]);
            AssertQuad(6, 5, 1, models[2].Quads[10]);
            AssertQuad(6, 1, 2, models[2].Quads[11]);

            AssertVector(new Vector3(1, 1, 1), models[3].Vertices[0]);
            AssertVector(new Vector3(1, -1, 1), models[3].Vertices[1]);
            AssertVector(new Vector3(1, 1, -1), models[3].Vertices[2]);
            AssertVector(new Vector3(1, -1, -1), models[3].Vertices[3]);
            AssertVector(new Vector3(-1, 1, 1), models[3].Vertices[4]);
            AssertVector(new Vector3(-1, -1, 1), models[3].Vertices[5]);
            AssertVector(new Vector3(-1, 1, -1), models[3].Vertices[6]);
            AssertVector(new Vector3(-1, -1, -1), models[3].Vertices[7]);
            AssertQuad(1, 5, 7, models[2].Quads[0]);
            AssertQuad(1, 7, 3, models[2].Quads[1]);
            AssertQuad(4, 3, 7, models[2].Quads[2]);
            AssertQuad(4, 7, 8, models[2].Quads[3]);
            AssertQuad(8, 7, 5, models[2].Quads[4]);
            AssertQuad(8, 5, 6, models[2].Quads[5]);
            AssertQuad(6, 2, 4, models[2].Quads[6]);
            AssertQuad(6, 4, 8, models[2].Quads[7]);
            AssertQuad(2, 1, 3, models[2].Quads[8]);
            AssertQuad(2, 3, 4, models[2].Quads[9]);
            AssertQuad(6, 5, 1, models[2].Quads[10]);
            AssertQuad(6, 1, 2, models[2].Quads[11]);
        }

        [Test]
        public void Invert() {
            var obj =
    @"o X
v 0 0 0
v 1 0 0
v 2 1 0
v 3 3 0
f 1 2 3 4
f 1 2 3";
            var model = GetModel(obj, invert: true);
            Assert.AreEqual(3, model.Quads.Length);
            AssertQuad(4, 3, 2, model.Quads[0], (new QuadInfo(0, 6)));
            AssertQuad(4, 2, 1, model.Quads[1], (new QuadInfo(1, 6)));
            AssertQuad(3, 2, 1, model.Quads[2], (new QuadInfo(2, 7)));
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
            var model = GetModel(obj);
            Assert.AreEqual(3, model.Quads.Length);
            AssertQuad(1, 2, 3, model.Quads[0], (new QuadInfo(0, 7)));
            AssertQuad(1, 3, 4, model.Quads[1], (new QuadInfo(1, 7)));
            AssertQuad(1, 4, 5, model.Quads[2], (new QuadInfo(2, 7)));
        }

        [Test]
        public void Hexagon() {
            var obj =
    @"o X
v 0 0 0
v 1 0 0
v 2 1 0
v 3 3 0
v 4 6 0
v 5 10 0
f 1 2 3 4 5 6";
            var model = GetModel(obj);
            Assert.AreEqual(4, model.Quads.Length);
            AssertQuad(1, 2, 3, model.Quads[0], (new QuadInfo(0, 8)));
            AssertQuad(1, 3, 4, model.Quads[1], (new QuadInfo(1, 8)));
            AssertQuad(1, 4, 5, model.Quads[2], (new QuadInfo(2, 8)));
            AssertQuad(1, 5, 6, model.Quads[3], (new QuadInfo(3, 8)));
        }

        [Test]
        public void Septagon() {
            var obj =
    @"o X
v 0 0 0
v 1 0 0
v 2 1 0
v 3 3 0
v 4 6 0
v 5 10 0
v 6 15 0
f 1 2 3 4 5 6 7";
            var model = GetModel(obj);
            Assert.AreEqual(5, model.Quads.Length);
            AssertQuad(1, 2, 3, model.Quads[0], (new QuadInfo(0, 9)));
            AssertQuad(1, 3, 4, model.Quads[1], (new QuadInfo(1, 9)));
            AssertQuad(1, 4, 5, model.Quads[2], (new QuadInfo(2, 9)));
            AssertQuad(1, 5, 6, model.Quads[3], (new QuadInfo(3, 9)));
            AssertQuad(1, 6, 7, model.Quads[4], (new QuadInfo(4, 9)));
        }

        static Model<QuadInfo> GetModel(string obj, bool invert = false) {
            return ObjLoader.Load(obj.AsStream(), new LoadOptions<QuadInfo>(x => x, invert: invert)).Single();
        }

        IEnumerable<Model<T>> LoadModels<T>(string fileName, Func<QuadInfo, T>? getValue = null) {
            var asm = Assembly.GetExecutingAssembly();
            return ObjLoader.Load<T>(asm.GetManifestResourceStream(asm.GetName().Name + $".Models.{fileName}.obj")!, new LoadOptions<T>(getValue));
        }

        static void AssertQuad<T>(int i1, int i2, int i3, Triangle<T> quad, T value = default!) {
            Assert.AreEqual(i1, quad.i1 + 1);
            Assert.AreEqual(i2, quad.i2 + 1);
            Assert.AreEqual(i3, quad.i3 + 1);
            Assert.AreEqual(value, quad.value);
        }
    }
}
