using MetaArt.Core;
using System.Diagnostics;
using System.Numerics;

namespace MetaConstruct;

public class Plot {

    void setup() {
        if(deviceType() == DeviceType.Desktop)
            size(400, 700);
        fullRedraw();

        int[] numbers = { 5, 10, 8, 3, 6, 12 };

        IEnumerable<int> numQuery0 =
            from num in numbers
            let n2 = num * 3
            select n2 * 2;
        IEnumerable<int> numQuery1 =
            from num in numbers
            from num2 in Enumerable.Range(0, num)
            from num3 in Enumerable.Range(0, num2)
            where num3 % 2 == 0
            orderby num3
            select num3 * 2;

        var x1 =
            from p1 in Constructor.Point()
            from p2 in Constructor.Point()
            select new Line(p1, p2)
        ;

        var x2 =
            from p1 in Constructor.Point()
            from p2 in Constructor.Point()
            select new Circle(p1, p2)
        ;

        var x3 =
            from p1 in Constructor.Point()
            from c in new Circle(p1, Constructor.Point())
            select c
        ;

        var x4 =
            from p1 in Constructor.Point()
            from l in new Line(p1, Constructor.Point())
            select l
        ;

        var x5 =
            from p1 in Constructor.Point()
            from p2 in Constructor.Point()
            from l in new Line(p1, p2)
            select l
        ;

        var x6 =
            from p1 in Constructor.Point()
            from p2 in Constructor.Point()
            from c in new Circle(p1, p2)
            select c
        ;

        //var x7 =
        //    from p1 in Constructor.Point()
        //    let l = new Line(p1, Constructor.Point())
        //    select l
        //;

        //var x7 =
        //    from p1 in Constructor.Point()
        //    from p2 in Constructor.Point()
        //    let l = new Line(p1, p2)
        //    select l
        //;
    }


    void draw() {
        stroke(White);
        line(0, 0, 100, 100);
    }
}

abstract record Construct() { }

record Point : Construct { 
}
record Line(Point From, Point To) : Construct { 
}
record Circle(Point From, Point To) : Construct {
}
record CirclesIntersection(Circle c1, Circle c2) : Construct { 
}
record LinesIntersection(Line l1, Line l2) : Construct {
}

static class Constructor {

    public static TResult SelectMany<T, TResult>(this T source, Func<T, Point> selector, Func<T, Point, TResult> resultSelector) {
        var p2 = selector(source);
        return resultSelector(source, p2);
    }

    public static TResult SelectMany<T, TResult>(this T source, Func<T, Line> selector, Func<T, Line, TResult> resultSelector) 
        {
        var p2 = selector(source);
        return resultSelector(source, p2);
    }

    public static TResult SelectMany<T, TResult>(this T source, Func<T, Circle> selector, Func<T, Circle, TResult> resultSelector) {
        var p2 = selector(source);
        return resultSelector(source, p2);
    }

    //public static Line Select<T>(this T source, Func<T, Line> selector) {
    //    var p2 = selector(source);
    //    return p2;
    //}
    //public static Point Select<T>(this T source, Func<T, Point> selector) {
    //    var p2 = selector(source);
    //    return p2;
    //}

    public static Point Point() => new Point();
    public static CirclesIntersection Intersect(Circle c1, Circle c2) => new CirclesIntersection(c1, c2);
    public static LinesIntersection Intersect(Line l1, Line l2) => new LinesIntersection(l1, l2);
}
