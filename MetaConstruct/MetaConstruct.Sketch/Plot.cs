using MetaArt.Core;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using static MetaConstruct.Constructor;

namespace MetaConstruct;

class Plot {
    Painter painter = null!;
    readonly Func<PlotInfo> getPlot;

    public Plot(Func<PlotInfo> getPlot) {
        this.getPlot = getPlot;
    }

    void setup() {
        if(deviceType() == DeviceType.Desktop)
            size(800, 800);
        //fullRedraw();

        painter = new Painter(
            DrawCircle: circle => {
                stroke(color(70));
                Sketch.circle(circle.center.X, circle.center.Y, circle.radius * 2);
            },
            DrawCircleSegment: segment => {
                stroke(White);
                Sketch.arc(segment.circle.center.X, segment.circle.center.Y, segment.circle.radius * 2, segment.circle.radius * 2, segment.from, segment.to);
            },
            DrawLine: l => {
                var v = Vector2.Normalize(l.from - l.to);
                var p1 = l.to + v * (width + height);
                var p2 = l.from - v * (width + height);
                stroke(color(70));
                line(p1.X, p1.Y, p2.X, p2.Y);
            },
            DrawLineSegment: l => {
                stroke(White);
                line(l.from.X, l.from.Y, l.to.X, l.to.Y);
            }
        );
    }

    void draw() {
        background(Black);
        stroke(White);
        noFill();

        var info = getPlot();
        Plotter.Draw(info.Points, painter, info.Entities);
    }
}
record struct PlotInfo((FreePoint, Vector2)[] Points, Entity[] Entities);

static class PlotsHelpers {
    public static readonly (Func<PlotInfo> action, string name)[] Plots = new[] {
            RegisterPlot(Test1),
            RegisterPlot(Test2),
            RegisterPlot(Test3),
            RegisterPlot(SixCircles),
        };
    static (Func<PlotInfo>, string) RegisterPlot(Func<PlotInfo> action, [CallerArgumentExpression("action")] string name = "") {
        return (action, name);
    }

    static PlotInfo Test1() {
        var center = Point();
        var top = Point();
        var centerCircle = Circle(center, top);
        var c1 = Circle(top, center);

        var c2 = Circle(Intersect(centerCircle, c1).Point1, center);
        var c3 = Circle(Intersect(centerCircle, c2).Point1, center);
        var c4 = Circle(Intersect(centerCircle, c3).Point1, center);
        var c5 = Circle(Intersect(centerCircle, c4).Point1, center);
        var c6 = Circle(Intersect(centerCircle, c5).Point1, center);

        var l1 = Line(c2.Center, c4.Center);
        var l2 = Line(c1.Center, c3.Center);

        var p1 = Intersect(l1, l2);
        var p2 = Intersect(Line(p1, c6.Center), c6);
        var l3 = Line(p1, p2.Point1);

        var s1 = CircleSegment(
            c1,
            Intersect(c1, centerCircle).Point2,
            Intersect(c1, c2).Point2
        );
        var s2 = CircleSegment(
            c2,
            Intersect(c2, c1).Point2,
            Intersect(c2, centerCircle).Point1
        );
        var s3 = CircleSegment(
            centerCircle,
            Intersect(centerCircle, c1).Point1,
            Intersect(centerCircle, c2).Point2
        );
        var contour = new Contour(new Segment[] { 

        });

        return new PlotInfo(
            new[] {
                (center, new Vector2(400, 400)),
                (top, new Vector2(400, 300)),
            },
            new Entity[] { 
                centerCircle, 
                c1, c2, c3, c4, c5, c6, l1, l2, l3, 
                LineSegment(l1), 
                LineSegment(l2), 
                LineSegment(l3),
                s1, s2, s3
            }
        );
    }

    static PlotInfo Test2() {
        var p1 = Point();
        var p2 = Point();
        var p3 = Point();

        var c1 = Circle(p1, p3);
        var c2 = Circle(p2, p3);

        return new PlotInfo(
            new[] {
                    (p1, new Vector2(300, 400)),
                    (p2, new Vector2(500, 400)),
                    (p3, new Vector2(400, 300)),
            },
            new Entity[] {
                c1, 
                c2,
                CircleSegment(c1, c2),
                CircleSegment(c2, c1).Invert(),
            }
        );
    }

    static PlotInfo Test3() {
        var p1 = Point();
        var p2 = Point();
        var p3 = Point();

        var l = Line(p3, p1);
        var c = Circle(p2, p3);

        return new PlotInfo(
            new[] {
                    (p1, new Vector2(300, 400)),
                    (p2, new Vector2(400, 400)),
                    (p3, new Vector2(400, 300)),
            },
            new Entity[] {
                l,
                c,
                CircleSegment(c, l),
                LineSegment(l, c),
            }
        );
    }

    static PlotInfo SixCircles() {
        var center = Point();
        var top = Point();
        var centerCircle = Circle(center, top);
        var c1 = Circle(top, center);

        var c2 = Circle(Intersect(centerCircle, c1).Point1, center);
        var c3 = Circle(Intersect(centerCircle, c2).Point1, center);
        var c4 = Circle(Intersect(centerCircle, c3).Point1, center);
        var c5 = Circle(Intersect(centerCircle, c4).Point1, center);
        var c6 = Circle(Intersect(centerCircle, c5).Point1, center);

        return new PlotInfo(
            new[] {
                (center, new Vector2(400, 400)),
                (top, new Vector2(400, 300)),
            },
            new Entity[] { 
                centerCircle, 
                c1, c2, c3, c4, c5, c6,
                CircleSegment(c1, centerCircle),
                CircleSegment(c2, centerCircle),
                CircleSegment(c3, centerCircle),
                CircleSegment(c4, centerCircle),
                CircleSegment(c5, centerCircle),
                CircleSegment(c6, centerCircle),
            }
        );
    }
}