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
            p1.AsView().Add(surface, DisplayStyle.Background);
            p2.AsView().Add(surface, DisplayStyle.Visible);

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
            Intersect(Line(p1, p2), Line(p3, p4)).AsView().Add(surface, DisplayStyle.Background);
            Intersect(Line(p1, p2), Line(p5, p6)).AsView().Add(surface, DisplayStyle.Visible);

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
            i1.AsView().Add(surface, DisplayStyle.Background);
            i2.AsView().Add(surface, DisplayStyle.Visible);

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
            i1.AsView().Add(surface, DisplayStyle.Background);
            i2.AsView().Add(surface, DisplayStyle.Visible);

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
            i1.AsView().Add(surface, DisplayStyle.Background);
            i2.AsView().Add(surface, DisplayStyle.Visible);

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
            i1.AsView().Add(surface, DisplayStyle.Background);
            i2.AsView().Add(surface, DisplayStyle.Visible);

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
