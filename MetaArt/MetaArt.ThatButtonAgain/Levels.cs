﻿using MetaArt.Core;
using System.Diagnostics;
using System.Numerics;

namespace ThatButtonAgain;

public abstract class LevelBase {

    protected abstract int LevelIndex { get; }

    GameController controller = null!;

    void setup() {
        if(deviceType() == DeviceType.Desktop)
            size(400, 700);
        fullRedraw();
        textAlign(TextAlign.CENTER, TextVerticalAlign.CENTER);
        rectMode(CORNER);
        controller = new GameController(width / displayDensity(), height / displayDensity());


        textFont(createFont("SourceCodePro-Regular.ttf", controller.letterSize));


        controller.SetLevel(LevelIndex);
    }


    void draw()
    {
        controller.NextFrame(deltaTime);
        background(Colors.Background);
        scale(displayDensity(), displayDensity());
        noStroke();

        foreach (var item in controller.scene.Elements) {
            switch (item) {
                case Button b:
                    fill(b.IsPressed ? Colors.ButtonBackPressed : Colors.ButtonBackNormal);
                    rect(item.Rect.Left, item.Rect.Top, item.Rect.Width, item.Rect.Height);
                    break;
                case Letter l:
                    fill(Colors.LetterDragBox);
                    rect(item.Rect.Left, item.Rect.Top, item.Rect.Width, item.Rect.Height);
                    fill(Colors.LetterColor);
                    text(l.Value.ToString(), item.Rect.MidX, item.Rect.MidY - controller.letterVerticalOffset);
                    break;
                case FadeOutElement f:
                    fill(0, f.Opacity);
                    rect(0, 0, controller.scene.width, controller.scene.height);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        //if(isMousePressed || animations.HasAnimations)
        //    loop();
        //else
        //    noLoop();
    }

    void mousePressed()
    {
        controller.scene.Press(new System.Numerics.Vector2(mouseX / displayDensity(), mouseY / displayDensity()));
        //loop();
    }

    void mouseDragged()
    {
        controller.scene.Drag(new Vector2(mouseX / displayDensity(), mouseY / displayDensity()));
    }

    void mouseReleased()
    {
        //locked = false;
        controller.scene.Release();
        //noLoop();
    }

    static class Colors {
        public static Color LetterColor => new Color(0, 0, 0);
        public static Color LetterDragBox => new Color(120, 0, 0, 10);
        public static Color Background => new Color(100, 100, 100);
        public static Color ButtonBackNormal => new Color(255, 255, 255);
        public static Color ButtonBackPressed => new Color(200, 200, 200);
    }
}

class Level0 : LevelBase {
    protected override int LevelIndex => 0;
}

class Level1 : LevelBase {
    protected override int LevelIndex => 1;
}

