using MetaArt.Core;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using static MetaConstruct.ConstructorHelper;

namespace MetaConstruct;

class Plot {
    Painter painter = null!;
    readonly Func<Constructor, PlotInfo> getPlot;

    public Plot(Func<Constructor, PlotInfo> getPlot) {
        this.getPlot = getPlot;
    }

    void setup() {
        if(deviceType() == DeviceType.Desktop)
            size(800, 800);
        //fullRedraw();

        painter = new Painter(
            DrawCircle: circle => {
                stroke(color(70));
                noFill();
                Sketch.circle(circle.center.X, circle.center.Y, circle.radius * 2);
            },
            DrawCircleSegment: segment => {
                stroke(White);
                noFill();
                arc(segment.circle.center.X, segment.circle.center.Y, segment.circle.radius * 2, segment.circle.radius * 2, segment.from, segment.to);
            },
            DrawLine: l => {
                var v = Vector2.Normalize(l.from - l.to);
                var p1 = l.to + v * (width + height);
                var p2 = l.from - v * (width + height);
                stroke(color(70));
                noFill();
                line(p1.X, p1.Y, p2.X, p2.Y);
            },
            DrawLineSegment: l => {
                stroke(White);
                noFill();
                line(l.from.X, l.from.Y, l.to.X, l.to.Y);
            },
            FillContour: segments => {
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

        var info = getPlot(new Constructor());
        Plotter.Draw(info.Points, painter, info.Entities);
    }
}
record struct PlotInfo((FreePoint, Vector2)[] Points, Entity[] Entities);

static class PlotsHelpers {
    public static readonly (Func<Constructor, PlotInfo> action, string name)[] Plots = new[] {
            RegisterPlot(Test1),
            RegisterPlot(Test2),
            RegisterPlot(Test3),
            RegisterPlot(Pentagon),
            RegisterPlot(SixCircles),
        };
    static (Func<Constructor, PlotInfo>, string) RegisterPlot(Func<Constructor, PlotInfo> action, [CallerArgumentExpression("action")] string name = "") {
        return (action, name);
    }

    static PlotInfo Test1(Constructor c) {
        var center = c.Point();
        var top = c.Point();
        var centerCircle = c.Circle(center, top);
        var c1 = c.Circle(top, center);

        var c2 = c.Circle(c.Intersect(centerCircle, c1).Point1, center);
        var c3 = c.Circle(c.Intersect(centerCircle, c2).Point1, center);
        var c4 = c.Circle(c.Intersect(centerCircle, c3).Point1, center);
        var c5 = c.Circle(c.Intersect(centerCircle, c4).Point1, center);
        var c6 = c.Circle(c.Intersect(centerCircle, c5).Point1, center);

        var l1 = c.Line(c2.Center, c4.Center);
        var l2 = c.Line(c1.Center, c3.Center);

        var p1 = c.Intersect(l1, l2);
        var p2 = c.Intersect(c.Line(p1, c6.Center), c6);
        var l3 = c.Line(p1, p2.Point1);

        var s1 = CircleSegment(
            c1,
            c.Intersect(c1, centerCircle).Point2,
            c.Intersect(c1, c2).Point2
        );
        var s2 = CircleSegment(
            c2,
            c.Intersect(c2, c1).Point2,
            c.Intersect(c2, centerCircle).Point1
        );
        var s3 = CircleSegment(
            centerCircle,
            c.Intersect(centerCircle, c1).Point1,
            c.Intersect(centerCircle, c2).Point2
        );
        var contour = new Contour(new Segment[] { 
            s1, s2, s3
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
                s1, s2, s3,
                contour
            }
        );
    }

    static PlotInfo Test2(Constructor c) {
        var p1 = c.Point();
        var p2 = c.Point();
        var p3 = c.Point();

        var c1 = c.Circle(p1, p3);
        var c2 = c.Circle(p2, p3);

        return new PlotInfo(
            new[] {
                    (p1, new Vector2(300, 400)),
                    (p2, new Vector2(500, 400)),
                    (p3, new Vector2(400, 300)),
            },
            new Entity[] {
                c1, 
                c2,
                c.CircleSegment(c1, c2),
                c.CircleSegment(c2, c1, invert: true),
            }
        );
    }

    static PlotInfo Test3(Constructor c) {
        var p1 = c.Point();
        var p2 = c.Point();
        var p3 = c.Point();

        var l = c.Line(p3, p1);
        var c_ = c.Circle(p2, p3);

        return new PlotInfo(
            new[] {
                    (p1, new Vector2(300, 400)),
                    (p2, new Vector2(400, 400)),
                    (p3, new Vector2(400, 300)),
            },
            new Entity[] {
                l,
                c_,
                c.CircleSegment(c_, l),
                c.LineSegment(l, c_),
            }
        );
    }

    static PlotInfo Pentagon(Constructor c) {
        var center = c.Point();
        var vertex0 = c.Point();
        var circle = c.Circle(center, vertex0);
        var line0 = c.Line(center, vertex0);

        var line1 = PlotPrimitives.Bisection(c, vertex0, c.Intersect(line0, circle).Point2);

        var point0 = c.Intersect(line1, circle).Point2;
        var line3 = PlotPrimitives.Bisection(c, center, point0);
        var circle5 = c.Circle(c.Intersect(line1, line3), vertex0);

        var intersection = c.Intersect(line1, circle5);
        var circle6 = c.Circle(vertex0, intersection.Point2);
        var circle7 = c.Circle(vertex0, intersection.Point1);

        var (vertex4, vertex1) = c.Intersect(circle, circle7);
        var (vertex3, vertex2) = c.Intersect(circle, circle6);

        return new PlotInfo(
            new[] {
                    (center, new Vector2(400, 400)),
                    (vertex0, new Vector2(400, 300)),
            },
            new Entity[] {
                circle,
                line1,
                line0,
                line3,
                circle5,
                circle6,
                circle7,
                c.LineSegment(vertex0, vertex1),
                c.LineSegment(vertex1, vertex2),
                c.LineSegment(vertex2, vertex3),
                c.LineSegment(vertex3, vertex4),
                c.LineSegment(vertex4, vertex0),
            }
        );
    }

    static PlotInfo SixCircles(Constructor c) {
        var center = c.Point();
        var top = c.Point();
        var centerCircle = c.Circle(center, top);
        var c1 = c.Circle(top, center);

        var c2 = c.Circle(c.Intersect(centerCircle, c1).Point1, center);
        var c3 = c.Circle(c.Intersect(centerCircle, c2).Point1, center);
        var c4 = c.Circle(c.Intersect(centerCircle, c3).Point1, center);
        var c5 = c.Circle(c.Intersect(centerCircle, c4).Point1, center);
        var c6 = c.Circle(c.Intersect(centerCircle, c5).Point1, center);

        return new PlotInfo(
            new[] {
                (center, new Vector2(400, 400)),
                (top, new Vector2(400, 300)),
            },
            new Entity[] { 
                centerCircle, 
                c1, c2, c3, c4, c5, c6,
                c.CircleSegment(c1, centerCircle),
                c.CircleSegment(c2, centerCircle),
                c.CircleSegment(c3, centerCircle),
                c.CircleSegment(c4, centerCircle),
                c.CircleSegment(c5, centerCircle),
                c.CircleSegment(c6, centerCircle),
            }
        );
    }
}

static class PlotPrimitives {
    public static Line Bisection(this Constructor c, Point p1, Point p2) => c.AsLine(c.Intersect(c.Circle(p1, p2), c.Circle(p2, p1)));
}