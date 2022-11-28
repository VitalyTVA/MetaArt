using MetaConstruct;
using MetaConstruct.Serialization;
using NUnit.Framework;
using System;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MetaContruct.Tests {
    [TestFixture]
    public class SerializationTests : ModelTestsBase {
        [Test]
        public void SavePoints() {
            var p1 = Point();
            var p2 = Point();
            var surface = CreateTestSurface();
            surface.SetPoints(new[] {
                (p1, new Vector2(1, 2)),
                (p2, new Vector2(3, 4)),
            });
            p1.Add(surface, DisplayStyle.Background);
            p2.Add(surface, DisplayStyle.Visible);

            AssertSerialization(surface);
        }

        [Test]
        public void SaveLine() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();
            var surface = CreateTestSurface();
            surface.SetPoints(new[] {
                (p1, new Vector2(1, 2)),
                (p2, new Vector2(3, 4)),
                (p3, new Vector2(10, 20)),
                (p4, new Vector2(30, 40)),
            });
            Line(p1, p2).Add(surface, DisplayStyle.Background);
            Line(p3, p4).Add(surface, DisplayStyle.Visible);

            AssertSerialization(surface);
        }

        [Test]
        public void SaveLineSegment() {
            var p1 = Point();
            var p2 = Point();
            var surface = CreateTestSurface();
            surface.SetPoints(new[] {
                (p1, new Vector2(1, 2)),
                (p2, new Vector2(3, 4)),
            });
            c.LineSegment(p1, p2).Add(surface, DisplayStyle.Background);

            AssertSerialization(surface);
        }

        [Test]
        public void SaveLineSegment_FromIntersectionPoints() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();
            var p5 = Point();
            var surface = CreateTestSurface();
            surface.SetPoints(new[] {
                (p1, new Vector2(0, 10)),
                (p2, new Vector2(10, -10)),
                (p3, new Vector2(-10, -10)),
                (p4, new Vector2(10, 0)),
                (p5, new Vector2(-10, 0)),
            });
            var line = Line(p4, p5);

            ConstructorHelper.LineSegment(line, Intersect(line, Line(p1, p2)), Intersect(line, Line(p1, p3)))
                .Add(surface, DisplayStyle.Visible);

            AssertSerialization(surface);
        }

        [Test]
        public void SaveCircle() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var surface = CreateTestSurface();
            surface.SetPoints(new[] {
                (p1, new Vector2(1, 2)),
                (p2, new Vector2(3, 4)),
                (p3, new Vector2(10, 20)),
            });
            Circle(p1, p2, p3).Add(surface, DisplayStyle.Background);

            AssertSerialization(surface);
        }


        [Test]
        public void SaveLine_CommonPoint() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var surface = CreateTestSurface();
            surface.SetPoints(new[] {
                (p1, new Vector2(1, 2)),
                (p2, new Vector2(3, 4)),
                (p3, new Vector2(10, 20)),
            });
            Line(p1, p2).Add(surface, DisplayStyle.Background);
            Line(p1, p3).Add(surface, DisplayStyle.Visible);

            AssertSerialization(surface);
        }

        [Test]
        public void SaveLineLineIntersectionPoint() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();
            var p5 = Point();
            var p6 = Point();
            var surface = CreateTestSurface();
            surface.SetPoints(new[] {
                (p1, new Vector2(1, 2)),
                (p2, new Vector2(3, 4)),
                (p3, new Vector2(-10, -20)),
                (p4, new Vector2(30, 40)),
                (p5, new Vector2(10, 20)),
                (p6, new Vector2(-30, -40)),
            });
            Intersect(Line(p1, p2), Line(p3, p4)).Add(surface, DisplayStyle.Background);
            Intersect(Line(p1, p2), Line(p5, p6)).Add(surface, DisplayStyle.Visible);

            AssertSerialization(surface);
        }

        [Test]
        public void SaveLineCircleIntersectionPoint() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();
            var surface = CreateTestSurface();
            surface.SetPoints(new[] {
                (p1, new Vector2(0, 0)),
                (p2, new Vector2(0, 10)),
                (p3, new Vector2(10, 0)),
                (p4, new Vector2(-10, 0)),
            });
            var (i1, i2) = Intersect(Line(p3, p4), Circle(p1, p2));
            i1.Add(surface, DisplayStyle.Background);
            i2.Add(surface, DisplayStyle.Visible);

            AssertSerialization(surface);
        }

        [Test]
        public void SaveLineCircleIntersectionPoint_CommonPoint() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var surface = CreateTestSurface();
            surface.SetPoints(new[] {
                (p1, new Vector2(0, 0)),
                (p2, new Vector2(0, 10)),
                (p3, new Vector2(10, 0)),
            });
            var (i1, i2) = Intersect(Line(p3, p2), Circle(p1, p2));
            i1.Add(surface, DisplayStyle.Background);
            i2.Add(surface, DisplayStyle.Visible);

            AssertSerialization(surface);
        }

        [Test]
        public void SaveCirclesIntersectionPoint() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();
            var surface = CreateTestSurface();
            surface.SetPoints(new[] {
                (p1, new Vector2(0, 0)),
                (p2, new Vector2(0, 10)),
                (p3, new Vector2(10, 0)),
                (p4, new Vector2(-10, 0)),
            });
            var (i1, i2) = Intersect(Circle(p3, p4), Circle(p1, p2));
            i1.Add(surface, DisplayStyle.Background);
            i2.Add(surface, DisplayStyle.Visible);

            AssertSerialization(surface);
        }

        [Test]
        public void SaveCirclesIntersectionPoint_CommonPoint() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var surface = CreateTestSurface();
            surface.SetPoints(new[] {
                (p1, new Vector2(0, 0)),
                (p2, new Vector2(0, 10)),
                (p3, new Vector2(10, 0)),
            });
            var (i1, i2) = Intersect(Circle(p1, p3), Circle(p2, p3));
            i1.Add(surface, DisplayStyle.Background);
            i2.Add(surface, DisplayStyle.Visible);

            AssertSerialization(surface);
        }

        [Test]
        public void SaveCircleSegment() {
            var p1 = Point();
            var p2 = Point();
            var surface = CreateTestSurface();
            surface.SetPoints(new[] {
                (p1, new Vector2(1, 2)),
                (p2, new Vector2(3, 4)),
            });
            var c1 = Circle(p1, p2);
            var c2 = Circle(p2, p1);
            var (i1, i2) = Intersect(c1, c2);
            CircleSegment(c1, i1, i2).Add(surface, DisplayStyle.Background);
            CircleSegment(c2, i1, i2).Add(surface, DisplayStyle.Visible);

            AssertSerialization(surface);
        }

        [Test]
        public void SaveContour_CircleSegments() {
            var center = Point();
            var top = Point();
            var centerCircle = Circle(center, top);
            var c1 = Circle(top, center);
            var c2 = Circle(Intersect(centerCircle, c1).Point1, center);
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
            var surface = CreateTestSurface();
            surface.SetPoints(new[] {
                (center, new Vector2(0, 0)),
                (top, new Vector2(0, 1)),
            });
            new Contour(new Segment[] {
                s1, s2, s3
            }).Add(surface, DisplayStyle.Background);

            AssertSerialization(surface);
        }

        [Test]
        public void SaveContour_LineSegments() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var surface = CreateTestSurface();
            new Contour(new Segment[] {
                LineSegment(p1, p2),
                LineSegment(p2, p3),
                LineSegment(p3, p1),

            }).Add(surface, DisplayStyle.Visible);
            surface.SetPoints(new[] {
                    (p1, new Vector2(0, 0)),
                    (p2, new Vector2(0, 1)),
                    (p3, new Vector2(1, 0)),
            });
            AssertSerialization(surface);
        }

        [Test]
        public void SaveSegmentAndContour1() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var surface = CreateTestSurface();
            var s = LineSegment(p1, p2);
            new Contour(new Segment[] {
                s,
                LineSegment(p2, p3),
                LineSegment(p3, p1),

            }).Add(surface, DisplayStyle.Visible);
            s.Add(surface);
            surface.SetPoints(new[] {
                    (p1, new Vector2(0, 0)),
                    (p2, new Vector2(0, 1)),
                    (p3, new Vector2(1, 0)),
            });
            AssertSerialization(surface);
        }

        [Test]
        public void SaveSegmentAndContour2() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var surface = CreateTestSurface();
            var s = LineSegment(p1, p2);
            s.Add(surface);
            new Contour(new Segment[] {
                s,
                LineSegment(p2, p3),
                LineSegment(p3, p1),

            }).Add(surface, DisplayStyle.Visible);
            surface.SetPoints(new[] {
                    (p1, new Vector2(0, 0)),
                    (p2, new Vector2(0, 1)),
                    (p3, new Vector2(1, 0)),
            });
            AssertSerialization(surface);
        }

        [Test, Explicit]
        public void TestExactPlot() {
            PlotCollection(PlotSource[0]);
        }

        [Test]
        [TestCaseSource(nameof(PlotSource))]
        public void PlotCollection((Func<Constructor, Surface, PlotInfo> action, string name) plot) {
            var (action, name) = plot;
            var surface = CreateTestSurface();
            var info = action(surface.Constructor, surface);
            surface.SetPoints(info.Points);
            AssertSerialization(surface);
        }
        static (Func<Constructor, Surface, PlotInfo> action, string name)[] PlotSource = PlotsCollection.Plots;

        static void AssertSerialization(Surface surface0) {
            var plot0 = surface0.PlotToString();

            var jsonString1 = SurfaceInfo.Serialize(surface0);
            var surface1 = new Surface(new Constructor(), surface0.PointHitTestDistance);
            SurfaceInfo.Deserialize(surface1, jsonString1);
            var plot1 = surface1.PlotToString();
            Assert.AreEqual(plot0, plot1);

            var jsonString2 = SurfaceInfo.Serialize(surface0);
            Assert.AreEqual(jsonString1, jsonString2);

            var surface2 = new Surface(new Constructor(), surface0.PointHitTestDistance);
            SurfaceInfo.Deserialize(surface2, jsonString1);
            var plot2 = surface1.PlotToString();
            Assert.AreEqual(plot0, plot2);
        }
    }
}
