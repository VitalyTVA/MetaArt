using System.Numerics;

namespace MetaArt.D3;
public class Model<T> {
    public readonly Vector3[] Vertices;
    public readonly Triangle<T>[] Quads;
    public Quaternion Rotation { get; set; }
    public Vector3 Scale { get; set; }
    public Model(Vector3[] vertices, Triangle<T>[] quads) {
        Vertices = vertices;
        Quads = quads;
        Rotation = Quaternion.Identity;
        Scale = new Vector3(1, 1, 1);
    }
    public Vector3 GetVertex(int index) => Vector3.Transform(Vertices[index] * Scale, Rotation);
}
public record struct Triangle<T>(int i1, int i2, int i3, T value) {
    public static implicit operator Triangle<T>((int i1, int i2, int i3, T value) quad)
        => new Triangle<T>(quad.i1, quad.i2, quad.i3, quad.value);
}
public struct VoidType { }
