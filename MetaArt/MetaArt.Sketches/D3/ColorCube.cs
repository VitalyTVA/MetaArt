﻿using System.Numerics;
using MetaArt.D3;
using Vector = MetaArt.Vector;

namespace D3;
class ColorCube {
    Model<Color> cube = null!;
    void setup() {
        size(600, 400);

        cube = Extensions.CreateCube(100, (
            front: Colors.Red,
            back: Colors.Blue,
            left: Colors.Pink,
            right: Colors.Green,
            top: Colors.Yellow,
            bottom: Colors.Orange
        ));
    }

    YawPitchContoller cameraController = new();

    void draw() {
        noSmooth();
        strokeWeight(1);
        noFill();
        background(0);
        CameraExtensions.InitCoords();

        var c = cameraController.CreateCamera();
        //vertices.RotateY(deltaTime / 1000f);

        var dx = mouseX - pmouseX;
        var dy = mouseY - pmouseY;

        if(isLeftMousePressed) {
            const float scale = 200;
            cameraController.Pitch(-dy / scale);
            cameraController.Yaw(-dx / scale);
        }
        if(isRightMousePressed) {
            cube.Rotate(c, dx, -dy);
        }


        var lines = new[] {
            (0, 1), (1, 2), (2, 3), (3, 0),
            (4, 5), (5, 6), (6, 7), (7, 4),
            (0, 4), (1, 5), (2, 6), (3, 7),
        };
        stroke(White);
        foreach(var (from, to) in lines) {
            c.line3(
                cube.GetVertex(from),
                cube.GetVertex(to)
            );
        }

        foreach(var (i1, i2, i3, i4, col) in cube.Quads) {
            fill(col);
            c.quad3(
                cube.GetVertex(i1),
                cube.GetVertex(i2),
                cube.GetVertex(i3),
                cube.GetVertex(i4)
            );
        }
    }

    void keyPressed() {
        CameraExtensions.MoveOnShepeOnKeyPressed(cameraController);
    }
}

//public class Lights {
//    public float Ambient;
//    public DirectionalLight Directional;
//}
//public record struct DirectionalLight(Vector3 deirection, float intensity);
