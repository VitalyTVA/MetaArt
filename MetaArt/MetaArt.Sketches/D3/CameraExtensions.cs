using MetaArt.D3;
using System.Numerics;

namespace D3;

static class CameraExtensions {
    public static void MoveCameraOnShepeOnKeyPressed(SphereCameraContoller controller) {
        var direction = key switch {
            'a' => MoveDirection.Left,
            'd' => MoveDirection.Right,
            'w' => MoveDirection.Up,
            's' => MoveDirection.Down,
            _ => default(MoveDirection?)
        };
        if(direction is not null)
            controller.Move(direction.Value);
    }
    public static void InitCoords() {
        translate(width / 2, height / 2);
        scale(1, -1);
    }

    public static void line3(this Camera c, Vector3 p1, Vector3 p2) {
        var (x1, y1) = c.ProjectPoint(p1);
        var (x2, y2) = c.ProjectPoint(p2);
        line(x1, y1, x2, y2);
    }
    public static void quad3(this Camera c, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4) {
        var n = Extensions.GetNormal(p1, p2, p3);
        if(!c.IsVisible(p1, n)) return;

        var (x1, y1) = c.ProjectPoint(p1);
        var (x2, y2) = c.ProjectPoint(p2);
        var (x3, y3) = c.ProjectPoint(p3);
        var (x4, y4) = c.ProjectPoint(p4);
        quad(x1, y1, x2, y2, x3, y3, x4, y4);
    }
}
