using MetaArt.Core;
using System.Diagnostics;
using System.Numerics;
using static MetaConstruct.Constructor;

namespace MetaConstruct;

public class Plot {

    void setup() {
        if(deviceType() == DeviceType.Desktop)
            size(400, 700);
        fullRedraw();


        var x1 =
            from p1 in Point()
            from p2 in Point()
            select Line(p1, p2)
        ;

        var x2 =
            from p1 in Point()
            from p2 in Point()
            select Circle(p1, p2)
        ;

        var x5 =
            from p1 in Point()
            from p2 in Point()
            from l in Line(p1, p2)
            select l
        ;

        var x6 =
            from p1 in Point()
            from p2 in Point()
            from c in Circle(p1, p2)
            select c
        ;

        var x7 =
            from p1 in Point()
            let l = new Line(p1, new Point())
            select l
        ;

        var x8 =
            from p1 in Constructor.Point()
            from p2 in Constructor.Point()
            let l = new Line(p1, p2)
            select l
        ;

        var circles =
            from center in Point()
            from top in Point()
            let centerCircle = new Circle(center, top)
            let c1 = new Circle(top, center)

            from i1 in Intersect(centerCircle, c1)
            let c2 = new Circle(i1.p1, center)

            from i2 in Intersect(c1, c2)
            let c3 = new Circle(i2.p1, center)

            from i3 in Intersect(c2, c3)
            let c4 = new Circle(i3.p1, center)

            from i4 in Intersect(c3, c4)
            let c5 = new Circle(i4.p1, center)

            select (c1, c2, c3, c4);

        //var center = new Point();
        //var point = new Point();
        //var centerCircle = new Circle(center, point);

        //var c1 = new Circle(point, center);
        //var intersection1 = new CirclesIntersection(centerCircle, c1);

        //Enumerable.SelectMany
    }


    void draw() {
        stroke(White);
        line(0, 0, 100, 100);
    }
}


record Point { 
}
record Line(Point From, Point To) { 
}
record Circle(Point From, Point To) {
}

class Plot<T> { 
}


static class Constructor {

    public static Plot<Point> Point() => throw new NotImplementedException();
    public static Plot<Line> Line(Point p1, Point p2) => throw new NotImplementedException();
    public static Plot<Circle> Circle(Point center, Point point) => throw new NotImplementedException();
    public static Plot<T> AsPlot<T>(T source) => throw new NotImplementedException();

    public static Plot<(Point p1, Point p2)> Intersect(Circle c1, Circle c2) => throw new NotImplementedException();
    public static Plot<Point> Intersect(Line l1, Line l2) => throw new NotImplementedException();
    public static Plot<(Point p1, Point p2)> Intersect(Circle c, Line l) => throw new NotImplementedException();

    public static Plot<TResult> Select<TSource, TResult>(this Plot<TSource> source, Func<TSource, TResult> selector) { 
        return source.SelectMany(x => AsPlot(selector(x)));
    }

    public static Plot<TResult> SelectMany<TSource, TResult>(this Plot<TSource> source, Func<TSource, Plot<TResult>> selector) { 
        return source.SelectMany(selector, (_, x) => x);
    }

    public static Plot<TResult> SelectMany<TSource, TSelect, TResult>(
        this Plot<TSource> source, Func<TSource, Plot<TSelect>> collectionSelector, 
        Func<TSource, TSelect, TResult> resultSelector
    ) { 
        throw new NotImplementedException();
    }
}
