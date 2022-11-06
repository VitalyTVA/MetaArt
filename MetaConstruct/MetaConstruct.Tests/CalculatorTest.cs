using MetaConstruct;
using NUnit.Framework;
using System;
using System.Linq;
using System.Numerics;

namespace MetaContruct.Tests {
    [TestFixture]
    public class CalculatorTest : ConstructorTestsBase {
        [Test]
        public void IntersectionCalcCount() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();
            var points = new[] {
                (p1, new Vector2(0, 0)),
                (p2, new Vector2(1, 1)),
                (p3, new Vector2(1, 0)),
                (p4, new Vector2(0, 1)),
            };
            var calculator = CreateCalculator(points);
            var l1 = Line(p1, p2);
            var l2 = Line(p3, p4);
            var c1 = Circle(p1, p2);
            var c2 = Circle(p3, p4);
            var i1 = Intersect(l1, l2);
            var i2 = Intersect(l1, c2);
            var i3 = Intersect(c1, c2);

            var c3 = Line(i1, i2.Point1);
            var l4 = Circle(i2.Point1, i3.Point1);
            var (x1, x2) = Intersect(c3, l4);

            Assert.AreEqual("(3.3635 3.3635) (-0.6315 -0.6315)", calculator.CalcLine(Line(x1, x2)).LineFToString());
            Assert.AreEqual("(3.3635 3.3635) 5.6498", calculator.CalcCircle(Circle(x1, x2)).CircleFToString());
            AssertCalcCounts(calculator, 1, 3, 1);
        }

        void AssertCalcCounts(Calculator calculator, int lines = 0, int lineCircle = 0, int circles = 0) {
            Assert.AreEqual(lines, calculator.LinesCalcCountForTests);
            Assert.AreEqual(lineCircle, calculator.LineCircleCalcCountForTests);
            Assert.AreEqual(circles, calculator.CirclesCalcCountForTests);
        }
    }
}
