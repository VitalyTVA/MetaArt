using MetaArt.D3;
using System.Numerics;

namespace D3;

static class CameraExtensions {
    public static void MoveOnShpereOnMousePressed(YawPitchContoller controller) {
        const float scale = 200;
        controller.ChangePitch(-mouseYOffset / scale);
        controller.ChangeYaw(-mouseXOffset / scale);
    }
    public static void MoveOnShepeOnKeyPressed(YawPitchContoller controller) {
        float step = PI / 60;
        switch(key) {
            case 'a':
                controller.ChangeYaw(-step);
                break;
            case 'd':
                controller.ChangeYaw(step);
                break;
            case 'w':
                controller.ChangePitch(-step);
                break;
            case 's':
                controller.ChangePitch(step);
                break;
        };
    }
    public static void InitCoords(float scaleFactor) {
        translate(width / 2, height / 2);
        scale(scaleFactor, -scaleFactor);
    }

    //public static void line3(this Camera c, Vector3 p1, Vector3 p2) {
    //    var (x1, y1) = c.ToScreenCoords(p1);
    //    var (x2, y2) = c.ToScreenCoords(p2);
    //    line(x1, y1, x2, y2);
    //}
    public static void triangle(Vector2 p1, Vector2 p2, Vector2 p3) {
        var (x1, y1) = p1;
        var (x2, y2) = p2;
        var (x3, y3) = p3;
        Sketch.triangle(x1, y1, x2, y2, x3, y3);
    }
}
