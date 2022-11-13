using MetaArt.Core;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using static MetaConstruct.ConstructorHelper;

namespace MetaConstruct;

class Plot {
    Painter painter = null!;
    readonly Func<Constructor, Surface, PlotInfo> getPlot;

    PlotController controller = null!;

    public Plot(Func<Constructor, Surface, PlotInfo> getPlot) {
        this.getPlot = getPlot;
    }

    void setup() {
        if(deviceType() == DeviceType.Desktop)
            size(800, 800);
        //fullRedraw();

        void SetStyle(DisplayStyle style) {
            stroke(color(style == DisplayStyle.Background ? 70 : 255));
            strokeWeight(1);
        }

        painter = new Painter(
            DrawCircle: (circle, style) => {
                SetStyle(style);
                noFill();
                Sketch.circle(circle.center.X, circle.center.Y, circle.radius * 2);
            },
            DrawCircleSegment: (segment, style) => {
                SetStyle(style);
                noFill();
                arc(segment.circle.center.X, segment.circle.center.Y, segment.circle.radius * 2, segment.circle.radius * 2, segment.from, segment.to);
            },
            DrawLine: (l, style) => {
                var v = Vector2.Normalize(l.from - l.to);
                var p1 = l.to + v * (width + height);
                var p2 = l.from - v * (width + height);
                SetStyle(style);
                noFill();
                line(p1.X, p1.Y, p2.X, p2.Y);
            },
            DrawLineSegment: (l, style) => {
                SetStyle(style);
                noFill();
                line(l.from.X, l.from.Y, l.to.X, l.to.Y);
            },
            FillContour: (segments, style) => {
                noStroke();
                fill(White);
                beginShape();
                foreach(var segment in segments) {
                    arcVertex(segment.circle.center.X, segment.circle.center.Y, segment.circle.radius * 2, segment.circle.radius * 2, segment.from, segment.to);
                }
                endShape(CLOSE);
            },
            DrawPoint: (p, kind, style) => {
                SetStyle(style);
                if(kind == PointKind.Free) {
                    stroke(255, 255, 0);
                }
                strokeWeight(4);
                point(p.X, p.Y);
            }
        );

        controller = new PlotController((int)(width / displayDensity()), (int)(height / displayDensity()));

        var constructor = new Constructor();
        var surface = new Surface(constructor, 10);
        var info = getPlot(surface.Constructor, surface);
        surface.SetPoints(info.Points);
        controller.Load(surface);
        

        var toolCaption = caption("Tool");

        void SetTool(Tool tool) { 
            controller.SetTool(tool);
            toolCaption.Text = tool.ToString();
        }

        SetTool(Tool.Point);

        foreach(Tool tool in Enum.GetValues(typeof(Tool))) {
            command(
                exectute: () => {
                    SetTool(tool);
                },
                caption: tool.ToString(),
                shortCut: tool switch {
                    Tool.Hand => ('h', ModifierKeys.None),
                    Tool.Point => ('p', ModifierKeys.None),
                    Tool.Line => ('l', ModifierKeys.None),
                    Tool.Circle => ('c', ModifierKeys.None),
                    Tool.LineSegment => ('s', ModifierKeys.None),
                    Tool.CircleSegment => ('a', ModifierKeys.None),
                }
            );
        }
        command(
            exectute: () => {
                if(controller.undoManager.CanUndo)
                    controller.undoManager.Undo();
            },
            caption: "Undo",
            shortCut: ('z', ModifierKeys.Ctrl)
        );
        command(
            exectute: () => {
                if(controller.undoManager.CanRedo)
                    controller.undoManager.Redo();
            },
            caption: "Redo",
            shortCut: ('y', ModifierKeys.Ctrl)
        );
    }

    void draw() {
        background(Black);
        stroke(White);
        noFill();
        foreach(var item in controller.scene.VisibleElements) {
            switch(item) {
                case PlotElement p:
                    Plotter.Draw(controller.Surface, painter);
                    break;
            }
        }
    }

    void mousePressed() {
        controller.scene.Press(new System.Numerics.Vector2(mouseX / displayDensity(), mouseY / displayDensity()));
    }

    void mouseDragged() {
        controller.scene.Drag(new Vector2(mouseX / displayDensity(), mouseY / displayDensity()));
    }

    void mouseReleased() {
        controller.scene.Release(new System.Numerics.Vector2(mouseX / displayDensity(), mouseY / displayDensity()));
    }
}
record struct PlotInfo((FreePoint, Vector2)[] Points);
public static class CanvasExtensions {
    public static T Add<T>(this T entity, Surface surface, DisplayStyle? style = null) where T : Entity {
        style = style ?? entity switch { 
            Primitive => DisplayStyle.Background,
            Segment or PointView => DisplayStyle.Visible,
            _ => throw new InvalidOperationException()
        };
        surface.Add(entity, style.Value);
        return entity;
    }
}
