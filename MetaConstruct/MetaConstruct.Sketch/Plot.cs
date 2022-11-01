﻿using MetaArt.Core;
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
            RegisterPlot(Bisection),
            RegisterPlot(DivideX16),
            RegisterPlot(Square),
            RegisterPlot(Pentagon),
            RegisterPlot(Pentaspiral),
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

    static PlotInfo Bisection(Constructor c, Surface s) {
        var p1 = c.Point();
        var p2 = c.Point();

        c.LineSegment(p1, p2).Add(s, DisplayStyle.Visible);
        var (bisection, _, (c1, c2)) = c.Bisection(p1, p2);
        bisection.Add(s);
        c1.Add(s);
        c2.Add(s);

        return new PlotInfo(
            new[] {
                (p1, new Vector2(300, 400)),
                (p2, new Vector2(500, 400)),
            }
        );
    }
    static PlotInfo DivideX16(Constructor c, Surface s) {
        var p1 = c.Point();
        var p2 = c.Point();

        c.LineSegment(p1, p2).Add(s);

        var (points, segments) = c.DivideNTimes(p1, p2, 4);

        foreach(var item in segments) {
            item.Add(s, DisplayStyle.Background);
        }

        return new PlotInfo(
            new[] {
                (p1, new Vector2(200, 400)),
                (p2, new Vector2(600, 400)),
            }
        );
    }

    static PlotInfo Square(Constructor c, Surface s) {
        var center = c.Point();
        var vertex0 = c.Point();

        var ((vertex1, vertex2, vertex3), (circle, diameter1, diameter2)) = c.Square(center, vertex0);

        circle.Add(s);
        c.LineSegment(diameter1, circle).Add(s, DisplayStyle.Background);
        c.LineSegment(diameter2, circle).Add(s, DisplayStyle.Background);

        c.LineSegment(vertex0, vertex1).Add(s);
        c.LineSegment(vertex1, vertex2).Add(s);
        c.LineSegment(vertex2, vertex3).Add(s);
        c.LineSegment(vertex3, vertex0).Add(s);

        return new PlotInfo(
            new[] {
                (center, new Vector2(400, 400)),
                (vertex0, new Vector2(300, 300)),
            }
        );
    }

    static PlotInfo Pentagon(Constructor c, Surface s) {
        var center = c.Point();
        var vertex0 = c.Point();

        var ((vertex1, vertex2, vertex3, vertex4), primitives) = c.Pentagon(center, vertex0);

        primitives.circle.Add(s);
        primitives.diameter.Add(s);
        primitives.bisection1.Add(s);

        c.LineSegment(primitives.bisection2, primitives.circle).Add(s, DisplayStyle.Background);
        c.CircleSegment(primitives.circle1, primitives.bisection1).Add(s, DisplayStyle.Background);
        c.CircleSegment(primitives.circle2, primitives.circle).Add(s, DisplayStyle.Background);
        c.CircleSegment(primitives.circle3, primitives.circle).Add(s, DisplayStyle.Background);

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

    static PlotInfo Pentaspiral(Constructor c, Surface s) {
        var center = c.Point();
        var vertex0 = c.Point();

        LineSegment[] MakePentagram(Point v0, Point v1, Point v2, Point v3, Point v4) => new[] {
            c.LineSegment(v0, v2).Add(s, DisplayStyle.Background),
            c.LineSegment(v2, v4).Add(s, DisplayStyle.Background),
            c.LineSegment(v4, v1).Add(s, DisplayStyle.Background),
            c.LineSegment(v1, v3).Add(s, DisplayStyle.Background),
            c.LineSegment(v3, v0).Add(s, DisplayStyle.Background),
        };

        var ((vertex1, vertex2, vertex3, vertex4), _) = c.Pentagon(center, vertex0);
        var segments = MakePentagram(vertex0, vertex1, vertex2, vertex3, vertex4);

        var vertex0_ = c.Intersect(c.Line(vertex0, vertex4), c.Line(vertex1, vertex2));
        var ((vertex1_, vertex2_, vertex3_, vertex4_), _) = c.Pentagon(center, vertex0_);
        var segments_ = MakePentagram(vertex0_, vertex1_, vertex2_, vertex3_, vertex4_);

        for(int i = 0; i < 5; i++) {
            var s1 = segments[(i + 1) % 5];
            var s2_ = segments_[(i + 2) % 5];

            var s4 = segments[(i + 4) % 5];
            var line = c.Line(s2_.To, center).AsLineSegment().Add(s, DisplayStyle.Background);

            var center_ = c.Intersect(s1.Line, s4.Line);
            var circle = c.Circle(center_, segments[i].From);
            var inersection = c.Intersect(line.Line, circle);
            CircleSegment(circle, circle.Point, inersection.Point2).Add(s);
        }
        for(int i = 0; i < 5; i ++) {
            var circle = c.Circle(segments[(i + 3) % 5].From, segments_[i].From);
            var line = c.Line(segments_[i].From, center);
            var inersection = c.Intersect(line, circle);
            CircleSegment(circle, circle.Point, inersection.Point2).Add(s);
        }

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
    public static (Line bisection, Point middle, (Circle c1, Circle c2) primitives) Bisection(this Constructor c, Point p1, Point p2) {
        var c1 = c.Circle(p1, p2);
        var c2 = c.Circle(p2, p1);
        var bisection = c.AsLine(c.Intersect(c1, c2));
        var middle = c.Intersect(bisection, c.Line(p1, p2));
        return (bisection, middle,(c1, c2));
    }

    public static (Point[] result, Segment[] segments) DivideNTimes(this Constructor c, Point p1, Point p2, int n) {
        var points = new List<Point> { p1, p2 };
        var segments = new List<Segment>();
        for(int i = 0; i < n; i++) {
            for(int j = 0; j < points.Count - 1; j++) {
                var (bisection, middle, (c1, c2)) = c.Bisection(points[j], points[j + 1]);
                points.Insert(j + 1, middle);
                j++;
                segments.Add(c.CircleSegment(c1, c2));
                segments.Add(c.CircleSegment(c2, c1));
                segments.Add(c.AsLine(c.Intersect(c2, c1)).AsLineSegment());
            }
        }
        return (points.ToArray(), segments.ToArray());
    }

    public static 
        (
            (Point vertex1, Point vertex2, Point vertex3, Point vertex4) result, 
            (Circle circle, Line diameter, Line bisection1, Line bisection2, Circle circle1, Circle circle2, Circle circle3) primitives
        )
        Pentagon(this Constructor c, Point center, Point vertex0) {
        var circle = c.Circle(center, vertex0);
        var diameter = c.Line(center, vertex0);

        var bisection1 = c.Bisection(vertex0, c.Intersect(diameter, circle).Point2).bisection;

        var bisection2 = c.Bisection(center, c.Intersect(bisection1, circle).Point2).bisection;
        c.LineSegment(bisection2, circle);
        var circle1 = c.Circle(c.Intersect(bisection1, bisection2), vertex0);
        c.CircleSegment(circle1, bisection1);

        var intersection = c.Intersect(bisection1, circle1);

        var circle2 = c.Circle(vertex0, intersection.Point1);
        c.CircleSegment(circle2, circle);
        var (vertex4, vertex1) = c.Intersect(circle, circle2);

        var circle3 = c.Circle(vertex0, intersection.Point2);
        c.CircleSegment(circle3, circle);
        var (vertex3, vertex2) = c.Intersect(circle, circle3);
        
        return ((vertex1, vertex2, vertex3, vertex4), (circle, diameter, bisection1, bisection2, circle1, circle2, circle3));
    }

    public static 
        (
            (Point vertex1, Point vertex2, Point vertex3) result, 
            (Circle circle, Line diameter1, Line diameter2) primitives
        )
        Square(this Constructor c, Point center, Point vertex0) {
        var circle = c.Circle(center, vertex0);
        var diameter1 = c.Line(center, vertex0);
        var diameter2 = c.Bisection(vertex0, c.Intersect(diameter1, circle).Point2).bisection;
        var vertex2 = c.Intersect(diameter1, circle).Point2;
        var (vertex1, vertex3) = c.Intersect(diameter2, circle);
        return ((vertex1, vertex2, vertex3), (circle, diameter1, diameter2));
    }
}