using System.Numerics;

namespace MetaArt.D3;
public class Model<T> {
    public readonly Vector3[] Vertices;
    public readonly (int, int, int, int, T)[] Quads;
    public Quaternion Rotation { get; set; }
    public Model(Vector3[] vertices, (int, int, int, int, T)[] quads) {
        Vertices = vertices;
        Quads = quads;
        Rotation = Quaternion.Identity;
    }
    public Vector3 GetVertex(int index) => Vector3.Transform(Vertices[index], Rotation);
}
