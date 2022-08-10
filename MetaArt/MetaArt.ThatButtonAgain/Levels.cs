﻿using MetaArt.Core;
using System.Diagnostics;
using System.Numerics;

namespace ThatButtonAgain;

public abstract class LevelBase {

    protected abstract int LevelIndex { get; }

    GameController controller = null!;
    Dictionary<SoundKind, SoundFile> sounds = new();
    Dictionary<SvgKind, SkiaSharp.Extended.Svg.SKSvg> svgs = new();

    void setup() {
        if(deviceType() == DeviceType.Desktop)
            size(400, 700);
        fullRedraw();
        
        rectMode(CORNER);
        controller = new GameController(
            width / displayDensity(),
            height / displayDensity(),
            playSound: kind => sounds[kind].play()
        );


        textFont(createFont("SourceCodePro-Regular.ttf", controller.letterSize));

        sounds = Enum.GetValues(typeof(SoundKind))
            .Cast<SoundKind>()
            .ToDictionary(x => x, x => createSound(x + ".wav"));

        svgs = Enum.GetValues(typeof(SvgKind))
            .Cast<SvgKind>()
            .ToDictionary(x => x, x => {
                var svg = new SkiaSharp.Extended.Svg.SKSvg();
                svg.Load(GetStream(x + ".svg"));
                return svg;
            });

        controller.SetLevel(LevelIndex);
    }


    void draw()
    {
        controller.NextFrame(deltaTime);
        background(Colors.Background);
        scale(displayDensity(), displayDensity());
        noStroke();

        foreach (var item in controller.scene.VisibleElements) {
            switch (item) {
                case Button b:
                    fill(b.IsPressed 
                        ? (b.IsEnabled ? Colors.ButtonBackPressed : Colors.ButtonBackPressedDisabled) 
                        : Colors.ButtonBackNormal);
                    rect(item.Rect.Left, item.Rect.Top, item.Rect.Width, item.Rect.Height);
                    break;
                case DragableButton b:
                    fill(Colors.ButtonBackNormal);
                    rect(item.Rect.Left, item.Rect.Top, item.Rect.Width, item.Rect.Height);
                    break;
                case Letter l:
                    if(MathFEx.VectorsEqual(l.Scale, Letter.NoScale)) {
                        fill(Colors.LetterDragBox);
                        rect(item.Rect.Left, item.Rect.Top, item.Rect.Width, item.Rect.Height);
                    }
                    pushMatrix();
                    translate(item.Rect.MidX, item.Rect.MidY);
                    scale(l.Scale.X, l.Scale.Y);
                    //fill(Colors.LetterDragBox);
                    //rect(0, 0, item.Rect.Width, item.Rect.Height);
                    fill(
                        lerp(Colors.UIElementColor.Red, Colors.LetterColor.Red, l.ActiveRatio),
                        lerp(Colors.UIElementColor.Green, Colors.LetterColor.Green, l.ActiveRatio),
                        lerp(Colors.UIElementColor.Blue, Colors.LetterColor.Blue, l.ActiveRatio)
                    );
                    textAlign(TextAlign.CENTER, TextVerticalAlign.CENTER);
                    text(l.Value.ToString(), 0, -controller.letterVerticalOffset);
                    popMatrix();
                    break;
                case FadeOutElement f:
                    fill(0, f.Opacity);
                    rect(f.Rect.Left, f.Rect.Top, f.Rect.Width, f.Rect.Height);
                    break;
                case SvgElement s:
                    var svg = svgs[s.Kind];
                    float scaleX = s.Rect.Width / svg.Picture.CullRect.Width;
                    float scaleY = s.Rect.Height / svg.Picture.CullRect.Height;
                    var matrix = SKMatrix.CreateScale(scaleX, scaleY);
                    pushMatrix();
                    translate(s.Rect.Left, s.Rect.Top);
                    ((MetaArt.Skia.SkiaGraphics)MetaArt.Internal.Graphics.GraphicsInstance).Canvas.DrawPicture(svg.Picture, ref matrix);
                    popMatrix();
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
        public static Color UIElementColor => new Color(70, 70, 70);
        public static Color LetterDragBox => new Color(120, 0, 0, 10);
        public static Color Background => new Color(150, 150, 150);
        public static Color ButtonBackNormal => new Color(255, 255, 255);
        public static Color ButtonBackPressed => new Color(200, 200, 200);
        public static Color ButtonBackPressedDisabled => new Color(200, 0, 0);
    }
}

class Level0 : LevelBase {
    protected override int LevelIndex => 0;
}
class Level1 : LevelBase {
    protected override int LevelIndex => 1;
}
class Level2 : LevelBase {
    protected override int LevelIndex => 2;
}
class Level3 : LevelBase {
    protected override int LevelIndex => 3;
}
class Level4 : LevelBase {
    protected override int LevelIndex => 4;
}
class Level5 : LevelBase {
    protected override int LevelIndex => 5;
}
class Level6 : LevelBase {
    protected override int LevelIndex => 6;
}
class Level7 : LevelBase {
    protected override int LevelIndex => 7;
}
class Level8 : LevelBase {
    protected override int LevelIndex => 8;
}
class Level9 : LevelBase {
    protected override int LevelIndex => 9;
}
class Level10 : LevelBase {
    protected override int LevelIndex => 10;
}
