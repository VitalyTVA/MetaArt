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
            Assert.AreEqual(12, model.Tris.Length);
            AssertVector(new Vector3(1, 1, 1), model.Vertices[0]);
            AssertVector(new Vector3(1, -1, 1), model.Vertices[1]);
            AssertVector(new Vector3(1, 1, -1), model.Vertices[2]);
            AssertVector(new Vector3(1, -1, -1), model.Vertices[3]);
            AssertVector(new Vector3(-1, 1, 1), model.Vertices[4]);
            AssertVector(new Vector3(-1, -1, 1), model.Vertices[5]);
            AssertVector(new Vector3(-1, 1, -1), model.Vertices[6]);
            AssertVector(new Vector3(-1, -1, -1), model.Vertices[7]);

            AssertTri(1, 5, 7, model.Tris[0], (0, 13));
            AssertTri(1, 7, 3, model.Tris[1], (1, 13));
            AssertTri(4, 3, 7, model.Tris[2], (2, 14));
            AssertTri(4, 7, 8, model.Tris[3], (3, 14));
            AssertTri(8, 7, 5, model.Tris[4], (4, 15));
            AssertTri(8, 5, 6, model.Tris[5], (5, 15));
            AssertTri(6, 2, 4, model.Tris[6], (6, 16));
            AssertTri(6, 4, 8, model.Tris[7], (7, 16));
            AssertTri(2, 1, 3, model.Tris[8], (8, 17));
            AssertTri(2, 3, 4, model.Tris[9], (9, 17));
            AssertTri(6, 5, 1, model.Tris[10], (10, 18));
            AssertTri(6, 1, 2, model.Tris[11], (11, 18));
        }

        [Test]
        public void Triangle() {
            var model = LoadModels<VoidType>("triangle").Single();
            Assert.AreEqual(3, model.Vertices.Length);
            Assert.AreEqual(1, model.Tris.Length);
            AssertVector(new Vector3(1, 1, 1), model.Vertices[0]);
            AssertVector(new Vector3(1, -1, 1), model.Vertices[1]);
            AssertVector(new Vector3(1, 1, -1), model.Vertices[2]);

            AssertTri(1, 2, 3, model.Tris[0]);
        }

        [Test]
        public void Cubes() {
            var models = LoadModels<VoidType>("cubes").ToArray();
            Assert.AreEqual(4, models.Length);
            Assert.AreEqual(8, models[0].Vertices.Length);
            Assert.AreEqual(12, models[0].Tris.Length);
            Assert.AreEqual(8, models[1].Vertices.Length);
            Assert.AreEqual(12, models[1].Tris.Length);
            Assert.AreEqual(8, models[2].Vertices.Length);
            Assert.AreEqual(12, models[2].Tris.Length);
            Assert.AreEqual(8, models[3].Vertices.Length);
            Assert.AreEqual(12, models[3].Tris.Length);

            AssertVector(new Vector3(4, 1, 1), models[0].Vertices[0]);
            AssertVector(new Vector3(4, -1, 1), models[0].Vertices[1]);
            AssertVector(new Vector3(4, 1, -1), models[0].Vertices[2]);
            AssertVector(new Vector3(4, -1, -1), models[0].Vertices[3]);
            AssertVector(new Vector3(2, 1, 1), models[0].Vertices[4]);
            AssertVector(new Vector3(2, -1, 1), models[0].Vertices[5]);
            AssertVector(new Vector3(2, 1, -1), models[0].Vertices[6]);
            AssertVector(new Vector3(2, -1, -1), models[0].Vertices[7]);
            AssertTri(1, 5, 7, models[0].Tris[0]);
            AssertTri(1, 7, 3, models[0].Tris[1]);
            AssertTri(4, 3, 7, models[0].Tris[2]);
            AssertTri(4, 7, 8, models[0].Tris[3]);
            AssertTri(8, 7, 5, models[0].Tris[4]);
            AssertTri(8, 5, 6, models[0].Tris[5]);
            AssertTri(6, 2, 4, models[0].Tris[6]);
            AssertTri(6, 4, 8, models[0].Tris[7]);
            AssertTri(2, 1, 3, models[0].Tris[8]);
            AssertTri(2, 3, 4, models[0].Tris[9]);
            AssertTri(6, 5, 1, models[0].Tris[10]);
            AssertTri(6, 1, 2, models[0].Tris[11]);


            AssertVector(new Vector3(1, 4, 1), models[1].Vertices[0]);
            AssertVector(new Vector3(1, 2, 1), models[1].Vertices[1]);
            AssertVector(new Vector3(1, 4, -1), models[1].Vertices[2]);
            AssertVector(new Vector3(1, 2, -1), models[1].Vertices[3]);
            AssertVector(new Vector3(-1, 4, 1), models[1].Vertices[4]);
            AssertVector(new Vector3(-1, 2, 1), models[1].Vertices[5]);
            AssertVector(new Vector3(-1, 4, -1), models[1].Vertices[6]);
            AssertVector(new Vector3(-1, 2, -1), models[1].Vertices[7]);
            AssertTri(1, 5, 7, models[1].Tris[0]);
            AssertTri(1, 7, 3, models[1].Tris[1]);
            AssertTri(4, 3, 7, models[1].Tris[2]);
            AssertTri(4, 7, 8, models[1].Tris[3]);
            AssertTri(8, 7, 5, models[1].Tris[4]);
            AssertTri(8, 5, 6, models[1].Tris[5]);
            AssertTri(6, 2, 4, models[1].Tris[6]);
            AssertTri(6, 4, 8, models[1].Tris[7]);
            AssertTri(2, 1, 3, models[1].Tris[8]);
            AssertTri(2, 3, 4, models[1].Tris[9]);
            AssertTri(6, 5, 1, models[1].Tris[10]);
            AssertTri(6, 1, 2, models[1].Tris[11]);

            AssertVector(new Vector3(1, 1, 4), models[2].Vertices[0]);
            AssertVector(new Vector3(1, -1, 4), models[2].Vertices[1]);
            AssertVector(new Vector3(1, 1, 2), models[2].Vertices[2]);
            AssertVector(new Vector3(1, -1, 2), models[2].Vertices[3]);
            AssertVector(new Vector3(-1, 1, 4), models[2].Vertices[4]);
            AssertVector(new Vector3(-1, -1, 4), models[2].Vertices[5]);
            AssertVector(new Vector3(-1, 1, 2), models[2].Vertices[6]);
            AssertVector(new Vector3(-1, -1, 2), models[2].Vertices[7]);
            AssertTri(1, 5, 7, models[2].Tris[0]);
            AssertTri(1, 7, 3, models[2].Tris[1]);
            AssertTri(4, 3, 7, models[2].Tris[2]);
            AssertTri(4, 7, 8, models[2].Tris[3]);
            AssertTri(8, 7, 5, models[2].Tris[4]);
            AssertTri(8, 5, 6, models[2].Tris[5]);
            AssertTri(6, 2, 4, models[2].Tris[6]);
            AssertTri(6, 4, 8, models[2].Tris[7]);
            AssertTri(2, 1, 3, models[2].Tris[8]);
            AssertTri(2, 3, 4, models[2].Tris[9]);
            AssertTri(6, 5, 1, models[2].Tris[10]);
            AssertTri(6, 1, 2, models[2].Tris[11]);

            AssertVector(new Vector3(1, 1, 1), models[3].Vertices[0]);
            AssertVector(new Vector3(1, -1, 1), models[3].Vertices[1]);
            AssertVector(new Vector3(1, 1, -1), models[3].Vertices[2]);
            AssertVector(new Vector3(1, -1, -1), models[3].Vertices[3]);
            AssertVector(new Vector3(-1, 1, 1), models[3].Vertices[4]);
            AssertVector(new Vector3(-1, -1, 1), models[3].Vertices[5]);
            AssertVector(new Vector3(-1, 1, -1), models[3].Vertices[6]);
            AssertVector(new Vector3(-1, -1, -1), models[3].Vertices[7]);
            AssertTri(1, 5, 7, models[2].Tris[0]);
            AssertTri(1, 7, 3, models[2].Tris[1]);
            AssertTri(4, 3, 7, models[2].Tris[2]);
            AssertTri(4, 7, 8, models[2].Tris[3]);
            AssertTri(8, 7, 5, models[2].Tris[4]);
            AssertTri(8, 5, 6, models[2].Tris[5]);
            AssertTri(6, 2, 4, models[2].Tris[6]);
            AssertTri(6, 4, 8, models[2].Tris[7]);
            AssertTri(2, 1, 3, models[2].Tris[8]);
            AssertTri(2, 3, 4, models[2].Tris[9]);
            AssertTri(6, 5, 1, models[2].Tris[10]);
            AssertTri(6, 1, 2, models[2].Tris[11]);
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
            Assert.AreEqual(3, model.Tris.Length);
            AssertTri(3, 2, 1, model.Tris[0], (new TriInfo(0, 6)));
            AssertTri(4, 3, 1, model.Tris[1], (new TriInfo(1, 6)));
            AssertTri(3, 2, 1, model.Tris[2], (new TriInfo(2, 7)));
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
            Assert.AreEqual(3, model.Tris.Length);
            AssertTri(1, 2, 3, model.Tris[0], (new TriInfo(0, 7)));
            AssertTri(1, 3, 4, model.Tris[1], (new TriInfo(1, 7)));
            AssertTri(1, 4, 5, model.Tris[2], (new TriInfo(2, 7)));
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
            Assert.AreEqual(4, model.Tris.Length);
            AssertTri(1, 2, 3, model.Tris[0], (new TriInfo(0, 8)));
            AssertTri(1, 3, 4, model.Tris[1], (new TriInfo(1, 8)));
            AssertTri(1, 4, 5, model.Tris[2], (new TriInfo(2, 8)));
            AssertTri(1, 5, 6, model.Tris[3], (new TriInfo(3, 8)));
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
            Assert.AreEqual(5, model.Tris.Length);
            AssertTri(1, 2, 3, model.Tris[0], (new TriInfo(0, 9)));
            AssertTri(1, 3, 4, model.Tris[1], (new TriInfo(1, 9)));
            AssertTri(1, 4, 5, model.Tris[2], (new TriInfo(2, 9)));
            AssertTri(1, 5, 6, model.Tris[3], (new TriInfo(3, 9)));
            AssertTri(1, 6, 7, model.Tris[4], (new TriInfo(4, 9)));
        }

        static Model<TriInfo> GetModel(string obj, bool invert = false) {
            return ObjLoader.Load(obj.AsStream(), new LoadOptions<TriInfo>(x => x, invert: invert)).Single();
        }

        IEnumerable<Model<T>> LoadModels<T>(string fileName, Func<TriInfo, T>? getValue = null) {
            var asm = Assembly.GetExecutingAssembly();
            return ObjLoader.Load<T>(asm.GetManifestResourceStream(asm.GetName().Name + $".Models.{fileName}.obj")!, new LoadOptions<T>(getValue));
        }

        static void AssertTri<T>(int i1, int i2, int i3, Tri<T> tri, T value = default!) {
            Assert.AreEqual(i1, tri.i1 + 1);
            Assert.AreEqual(i2, tri.i2 + 1);
            Assert.AreEqual(i3, tri.i3 + 1);
            Assert.AreEqual(value, tri.value);
        }

        [Test]
        public void ValidateModels() {
            var asm = Assembly.GetExecutingAssembly();
            var names = asm
                .GetManifestResourceNames()
                .Where(x => x.EndsWith(".obj"))
                //.Where(x => !x.EndsWith("monkey.obj"))
                .ToArray();
            CollectionAssert.IsNotEmpty(names.Where(x => x.EndsWith("cubes.obj")));
            foreach(var name in names) {
                ValidateModels(asm.GetManifestResourceStream(name)!);
            }
        }

        [Test]
        public void DuplicateVerticesDetection() {
            var obj =
    @"o X
v 0 0 0
v 0 0 0.000001
v 0 0 1
v 0 1 0
f 1 3 4";
            Assert.Throws<DuplicateVerticesException>(() => ValidateModels(obj.AsStream()));

            obj =
@"o X
v 0 0 0
v 0 0 1
v 0 1 0
f 1 2 3
o y
v 0 0.00001 0
v 0 0 1
v 0 1 0
f 4 5 6";
            Assert.Throws<DuplicateVerticesException>(() => ValidateModels(obj.AsStream()));
        }

        static void ValidateModels(Stream s) {
            var models = ObjLoader.Load(s, new LoadOptions<TriInfo>(getValue: x => x)).ToArray();
            var vertices = models.SelectMany(x => x.Vertices).ToArray();
            for(int i = 0; i < vertices.Length; i++) {
                for(int j = i + 1; j < vertices.Length; j++) {
                    if(Vector3.DistanceSquared(vertices[i], vertices[j]) < 0.0001)
                        throw new DuplicateVerticesException();
                }
            }
        }

        class DuplicateVerticesException : Exception {}
    }
}
