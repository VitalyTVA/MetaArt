﻿using MetaArt.D3;
using System.Numerics;

namespace D3;

static class CameraExtensions {
    public static void MoveOnShepeOnKeyPressed(YawPitchContoller controller) {
        float step = PI / 60;
        switch(key) {
            case 'a':
                controller.Yaw(-step);
                break;
            case 'd':
                controller.Yaw(step);
                break;
            case 'w':
                controller.Pitch(-step);
                break;
            case 's':
                controller.Pitch(step);
                break;
        };
    }
    public static void InitCoords() {
        translate(width / 2, height / 2);
        scale(1, -1);
    }

    //public static void line3(this Camera c, Vector3 p1, Vector3 p2) {
    //    var (x1, y1) = c.ToScreenCoords(p1);
    //    var (x2, y2) = c.ToScreenCoords(p2);
    //    line(x1, y1, x2, y2);
    //}
    public static void quad3(this Camera c, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4) {
        var (x1, y1) = c.ToScreenCoords(p1);
        var (x2, y2) = c.ToScreenCoords(p2);
        var (x3, y3) = c.ToScreenCoords(p3);
        var (x4, y4) = c.ToScreenCoords(p4);
        quad(x1, y1, x2, y2, x3, y3, x4, y4);
    }
}