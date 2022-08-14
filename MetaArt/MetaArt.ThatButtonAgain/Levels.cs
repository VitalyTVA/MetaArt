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
        foreach (var item in controller.scene.VisibleElements) {
            switch(item) {
                case Button b:
                    fill(b.IsPressed 
                        ? (b.IsEnabled ? Colors.ButtonBackPressed : Colors.ButtonBackPressedDisabled) 
                        : Colors.ButtonBackNormal);
                    stroke(Colors.ButtonBorder);
                    strokeWeight(Constants.ButtonBorderWeight);
                    rect(item.Rect.Left, item.Rect.Top, item.Rect.Width, item.Rect.Height, Constants.ButtonCornerRadius);
                    break;
                case Letter l:
                    noStroke();
                    if(MathFEx.VectorsEqual(l.Scale, Letter.NoScale)) {
                        fill(Colors.LetterDragBox);
                        rect(item.Rect.Left, item.Rect.Top, item.Rect.Width, item.Rect.Height);
                    }
                    pushMatrix();
                    translate(item.Rect.MidX, item.Rect.MidY);
                    scale(l.Scale.X, l.Scale.Y);
                    //fill(Colors.LetterDragBox);
                    //rect(0, 0, item.Rect.Width, item.Rect.Height);
                    fill(color(
                        lerp(Colors.UIElementColor.Red, Colors.LetterColor.Red, l.ActiveRatio),
                        lerp(Colors.UIElementColor.Green, Colors.LetterColor.Green, l.ActiveRatio),
                        lerp(Colors.UIElementColor.Blue, Colors.LetterColor.Blue, l.ActiveRatio),
                        l.Opacity * 255
                    ));
                    textAlign(TextAlign.CENTER, TextVerticalAlign.CENTER);
                    text(l.Value.ToString(), 0, -controller.letterVerticalOffset);
                    popMatrix();
                    break;
                case FadeOutElement f:
                    noStroke();
                    fill(0, f.Opacity);
                    rect(f.Rect.Left, f.Rect.Top, f.Rect.Width, f.Rect.Height);
                    break;
                case SvgElement s:
                    noStroke();
                    var svg = svgs[s.Kind];
                    float scaleX = s.Rect.Width / svg.Picture.CullRect.Width;
                    float scaleY = s.Rect.Height / svg.Picture.CullRect.Height;
                    var matrix = SKMatrix.CreateScale(scaleX, scaleY);
                    pushMatrix();
                    translate(s.Rect.Left, s.Rect.Top);
                    ((MetaArt.Skia.SkiaGraphics)MetaArt.Internal.Graphics.GraphicsInstance).Canvas.DrawPicture(svg.Picture, ref matrix);
                    popMatrix();
                    break;
                case PathElement p:
                    shapeCorners(Constants.ButtonCornerRadius);
                    fill(Colors.ButtonBackNormal);
                    stroke(Colors.ButtonBorder);
                    strokeWeight(Constants.ButtonBorderWeight);
                    beginShape();
                    foreach(var point in p.Points) {
                        vertex(point.X, point.Y);
                    }
                    endShape(EndShapeMode.CLOSE);
                    shapeCorners(0);
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
        controller.scene.Release(new System.Numerics.Vector2(mouseX / displayDensity(), mouseY / displayDensity()));
        //noLoop();
    }

    //static class Colors {
    //    public static Color LetterColor => new Color(0, 0, 0);
    //    public static Color UIElementColor => new Color(70, 70, 70);
    //    public static Color LetterDragBox => new Color(120, 0, 0, 10);
    //    public static Color Background => new Color(150, 150, 150);
    //    public static Color ButtonBackNormal => new Color(255, 255, 255);
    //    public static Color ButtonBackPressed => new Color(200, 200, 200);
    //    public static Color ButtonBackPressedDisabled => new Color(200, 0, 0);
    //}

    static class Colors {
        //static Palette Palette => Palettes.BlueGrayLight;
        static Palette Palette => Palettes.CoolGrayDark;

        public static Color LetterColor => Palette._900;
        public static Color UIElementColor => Palette._600;
        public static Color LetterDragBox => Color.Empty;//new Color(120, 0, 0, 10);
        public static Color Background => Palette._50;
        public static Color ButtonBackNormal => Palette._100;
        public static Color ButtonBackPressed => Palette._200;
        public static Color ButtonBorder => Palette._900;

        public static Color ButtonBackPressedDisabled => Palette.Error;
    }


    public static class CommonColors { 
        public static readonly Color Error_Light = Color.FromRGBValue(0xB00020);
        public static readonly Color Error_Dark = Color.FromRGBValue(0xCF6679);
    }

    static class Palettes {
        //https://material.io/design/color/the-color-system.html#color-usage-and-palettes
        public static readonly Palette BlueGrayLight = new Palette {
            _50 = Color.FromRGBValue(0xECEFF1),
            _100 = Color.FromRGBValue(0xCFD8DC),
            _200 = Color.FromRGBValue(0xB0BEC5),
            _300 = Color.FromRGBValue(0x90A4AE),
            _400 = Color.FromRGBValue(0x78909C),
            _500 = Color.FromRGBValue(0x607D8B),
            _600 = Color.FromRGBValue(0x546E7A),
            _700 = Color.FromRGBValue(0x455A64),
            _800 = Color.FromRGBValue(0x37474F),
            _900 = Color.FromRGBValue(0x263238),
            Error = CommonColors.Error_Light,
        };

        //https://compilezero.medium.com/dark-mode-ui-design-the-definitive-guide-part-1-color-53dcfaea5129
        //https://material.io/design/color/dark-theme.html#ui-application
        public static readonly Palette CoolGrayDark = new Palette {
            _50 = Color.FromRGBValue(0x1F2933),
            _100 = Color.FromRGBValue(0x323F4B),
            _200 = Color.FromRGBValue(0x3E4C59),
            _300 = Color.FromRGBValue(0x52606D),
            _400 = Color.FromRGBValue(0x616E7C),
            _500 = Color.FromRGBValue(0x7B8794),
            _600 = Color.FromRGBValue(0x9AA5B1),
            _700 = Color.FromRGBValue(0xCBD2D9),
            _800 = Color.FromRGBValue(0xE4E7EB),
            _900 = Color.FromRGBValue(0xF5F7FA),
            Error = CommonColors.Error_Dark,
        };
    }

    class Palette {
        public Color _50 { get; init; }
        public Color _100 { get; init; }
        public Color _200 { get; init; }
        public Color _300 { get; init; }
        public Color _400 { get; init; }
        public Color _500 { get; init; }
        public Color _600 { get; init; }
        public Color _700 { get; init; }
        public Color _800 { get; init; }
        public Color _900 { get; init; }
        public Color Error { get; init; }
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
class Level11 : LevelBase {
    protected override int LevelIndex => 11;
}
class Level12 : LevelBase {
    protected override int LevelIndex => 12;
}
class Level13 : LevelBase {
    protected override int LevelIndex => 13;
}