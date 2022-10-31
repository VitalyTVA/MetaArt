using MetaArt.Core;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using static MetaConstruct.ConstructorHelper;

namespace MetaConstruct;

class Plot {
    Painter painter = null!;
    readonly Func<Constructor, Surface, PlotInfo> getPlot;

    public Plot(Func<Constructor, Surface, PlotInfo> getPlot) {
        this.getPlot = getPlot;
    }

    void setup() {
        if(deviceType() == DeviceType.Desktop)
            size(800, 800);
        //fullRedraw();

        void SetStyle(DisplayStyle style) {
            stroke(color(style == DisplayStyle.Background ? 70 : 255));
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
            }
        );
    }

    void draw() {
        background(Black);
        stroke(White);
        noFill();

        var surface = new Surface();
        var info = getPlot(new Constructor(), surface);
        Plotter.Draw(info.Points, painter, surface.GetEntities());
    }
}
record struct PlotInfo((FreePoint, Vector2)[] Points);

static class PlotsHelpers {
    public static readonly (Func<Constructor, Surface, PlotInfo> action, string name)[] Plots = new[] {
            RegisterPlot(Test1),
            RegisterPlot(Test2),
            RegisterPlot(Test3),
            RegisterPlot(Pentagon),
            RegisterPlot(SixCircles),
        };
    static (Func<Constructor, Surface, PlotInfo>, string) RegisterPlot(Func<Constructor, Surface, PlotInfo> action, [CallerArgumentExpression("action")] string name = "") {
        return (action, name);
    }

    static PlotInfo Test1(Constructor c, Surface s) {
        var center = c.Point();
        var top = c.Point();
        var centerCircle = c.Circle(center, top).Add(s);
        var c1 = c.Circle(top, center).Add(s);

        var c2 = c.Circle(c.Intersect(centerCircle, c1).Point1, center).Add(s);
        var c3 = c.Circle(c.Intersect(centerCircle, c2).Point1, center).Add(s);
        var c4 = c.Circle(c.Intersect(centerCircle, c3).Point1, center).Add(s);
        var c5 = c.Circle(c.Intersect(centerCircle, c4).Point1, center).Add(s);
        var c6 = c.Circle(c.Intersect(centerCircle, c5).Point1, center).Add(s);

        var l1 = c.Line(c2.Center, c4.Center).Add(s);
        var l2 = c.Line(c1.Center, c3.Center).Add(s);

        var p1 = c.Intersect(l1, l2);
        var p2 = c.Intersect(c.Line(p1, c6.Center), c6);
        var l3 = c.Line(p1, p2.Point1).Add(s);

        var s1 = CircleSegment(
            c1,
            c.Intersect(c1, centerCircle).Point2,
            c.Intersect(c1, c2).Point2
        ).Add(s);
        var s2 = CircleSegment(
            c2,
            c.Intersect(c2, c1).Point2,
            c.Intersect(c2, centerCircle).Point1
        ).Add(s);
        var s3 = CircleSegment(
            centerCircle,
            c.Intersect(centerCircle, c1).Point1,
            c.Intersect(centerCircle, c2).Point2
        ).Add(s);
        new Contour(new Segment[] { 
            s1, s2, s3
        }).Add(s);
        l1.AsLineSegment().Add(s);
        l2.AsLineSegment().Add(s);
        l3.AsLineSegment().Add(s);

        return new PlotInfo(
            new[] {
                (center, new Vector2(400, 400)),
                (top, new Vector2(400, 300)),
            }
        );
    }

    static PlotInfo Test2(Constructor c, Surface s) {
        var p1 = c.Point();
        var p2 = c.Point();
        var p3 = c.Point();

        var c1 = c.Circle(p1, p3).Add(s);
        var c2 = c.Circle(p2, p3).Add(s);
        c.CircleSegment(c1, c2).Add(s);
        c.CircleSegment(c2, c1, invert: true).Add(s);

        return new PlotInfo(
            new[] {
                (p1, new Vector2(300, 400)),
                (p2, new Vector2(500, 400)),
                (p3, new Vector2(400, 300)),
            }
        );
    }

    static PlotInfo Test3(Constructor c, Surface s) {
        var p1 = c.Point();
        var p2 = c.Point();
        var p3 = c.Point();

        var l = c.Line(p3, p1).Add(s);
        var c_ = c.Circle(p2, p3).Add(s);

        c.CircleSegment(c_, l).Add(s);
        c.LineSegment(l, c_).Add(s);


        return new PlotInfo(
            new[] {
                (p1, new Vector2(300, 400)),
                (p2, new Vector2(400, 400)),
                (p3, new Vector2(400, 300)),
            }
        );
    }

    static PlotInfo Pentagon(Constructor c, Surface s) {
        var center = c.Point();
        var vertex0 = c.Point();
        var circle = c.Circle(center, vertex0).Add(s);
        var line0 = c.Line(center, vertex0).Add(s);

        var bisection1 = PlotPrimitives.Bisection(c, vertex0, c.Intersect(line0, circle).Point2).Add(s);

        var bisection2 = PlotPrimitives.Bisection(c, center, c.Intersect(bisection1, circle).Point2);
        c.LineSegment(bisection2, circle).Add(s, DisplayStyle.Background);
        var circle1 = c.Circle(c.Intersect(bisection1, bisection2), vertex0);
        c.CircleSegment(circle1, bisection1).Add(s, DisplayStyle.Background);

        var intersection = c.Intersect(bisection1, circle1);

        var circle2 = c.Circle(vertex0, intersection.Point1);
        c.CircleSegment(circle2, circle).Add(s, DisplayStyle.Background);
        var (vertex4, vertex1) = c.Intersect(circle, circle2);

        var circle3 = c.Circle(vertex0, intersection.Point2);
        c.CircleSegment(circle3, circle).Add(s, DisplayStyle.Background);
        var (vertex3, vertex2) = c.Intersect(circle, circle3);

        c.LineSegment(vertex0, vertex1).Add(s);
        c.LineSegment(vertex1, vertex2).Add(s);
        c.LineSegment(vertex2, vertex3).Add(s);
        c.LineSegment(vertex3, vertex4).Add(s);
        c.LineSegment(vertex4, vertex0).Add(s);

        return new PlotInfo(
            new[] {
                (center, new Vector2(400, 400)),
                (vertex0, new Vector2(400, 300)),
            }
        );
    }

    static PlotInfo SixCircles(Constructor c, Surface s) {
        var center = c.Point();
        var top = c.Point();
        var centerCircle = c.Circle(center, top).Add(s);
        var c1 = c.Circle(top, center).Add(s);

        var c2 = c.Circle(c.Intersect(centerCircle, c1).Point1, center).Add(s);
        var c3 = c.Circle(c.Intersect(centerCircle, c2).Point1, center).Add(s);
        var c4 = c.Circle(c.Intersect(centerCircle, c3).Point1, center).Add(s);
        var c5 = c.Circle(c.Intersect(centerCircle, c4).Point1, center).Add(s);
        var c6 = c.Circle(c.Intersect(centerCircle, c5).Point1, center).Add(s);

        c.CircleSegment(c1, centerCircle).Add(s);
        c.CircleSegment(c2, centerCircle).Add(s);
        c.CircleSegment(c3, centerCircle).Add(s);
        c.CircleSegment(c4, centerCircle).Add(s);
        c.CircleSegment(c5, centerCircle).Add(s);
        c.CircleSegment(c6, centerCircle).Add(s);

        return new PlotInfo(
            new[] {
                (center, new Vector2(400, 400)),
                (top, new Vector2(400, 300)),
            }
        );
    }
}

public class Surface {
    readonly List<(Entity, DisplayStyle)> entities = new List<(Entity, DisplayStyle)>();
    public void Add(Entity entity, DisplayStyle style) => entities.Add((entity, style));

    public IEnumerable<(Entity, DisplayStyle)> GetEntities() => entities;
}
public static class CanvasExtensions {
    public static T Add<T>(this T entity, Surface surface, DisplayStyle? style = null) where T : Entity {
        style = style ?? entity switch { 
            Primitive => DisplayStyle.Background,
            Segment => DisplayStyle.Visible,
            _ => throw new InvalidOperationException()
        };
        surface.Add(entity, style.Value);
        return entity;
    }
}

static class PlotPrimitives {
    public static Line Bisection(this Constructor c, Point p1, Point p2) => c.AsLine(c.Intersect(c.Circle(p1, p2), c.Circle(p2, p1)));
}