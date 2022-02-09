using System.Numerics;

namespace MetaArt.D3;
public class Model<T> {
    public readonly Vector3[] Vertices;
    public readonly Quad<T>[] Quads;
    public Quaternion Rotation { get; set; }
    public Vector3 Scale { get; set; }
    public Model(Vector3[] vertices, Quad<T>[] quads) {
        Vertices = vertices;
        Quads = quads;
        Rotation = Quaternion.Identity;
        Scale = new Vector3(1, 1, 1);
    }
    public Vector3 GetVertex(int index) => Vector3.Transform(Vertices[index] * Scale, Rotation);
}
public record struct Quad<T>(int i1, int i2, int i3, int i4, T value) {
    public static implicit operator Quad<T>((int i1, int i2, int i3, int i4, T value) quad) 
        => new Quad<T>(quad.i1, quad.i2, quad.i3, quad.i4, quad.value);
}
public struct VoidType { }
