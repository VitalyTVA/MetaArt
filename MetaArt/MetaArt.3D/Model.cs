using System.Numerics;

namespace MetaArt.D3;
public class Model<T> {
    public readonly Vector3[] Vertices;
    public readonly Tri<T>[] Tris;
    public Quaternion Rotation { get; set; }
    public Vector3 Scale { get; set; }
    public Model(Vector3[] vertices, Tri<T>[] tris) {
        Vertices = vertices;
        Tris = tris;
        Rotation = Quaternion.Identity;
        Scale = new Vector3(1, 1, 1);
    }
    public Vector3 GetVertex(int index) => Vector3.Transform(Vertices[index] * Scale, Rotation);
}
public record struct Tri<T>(int i1, int i2, int i3, T value) {
    public static implicit operator Tri<T>((int i1, int i2, int i3, T value) tri)
        => new Tri<T>(tri.i1, tri.i2, tri.i3, tri.value);
}
public struct VoidType { }
