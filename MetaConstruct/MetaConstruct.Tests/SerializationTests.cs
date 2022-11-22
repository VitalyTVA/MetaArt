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
    public class SerializationTests : ConstructorTestsBase {
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

            //var x = new SurfaceInfo(surface) { 
            //    //Date = DateTime.Today,
            //    //TemperatureCelsius = 117,
            //    //Summary = "test"
            //};

            //x.List.Add(new Foo { Name = "bar1" });

            //x.List.Add(new Bar { Name = "bar1", NameBar = "namebar1" });
            //x.List.Add(new Baz { Name = "foo", NameBaz = "namebaz1" });

            //var options = new JsonSerializerOptions { WriteIndented = true };

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
