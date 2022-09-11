using MetaArt;
using SkiaSharp;
using System.Diagnostics;

class Curve {
    SKPoint[] coords = {
        new SKPoint(40, 40), 
        new SKPoint(80, 60), 
        new SKPoint(100, 100), 
        new SKPoint(60, 120), 
        new SKPoint(50, 150)
    };
    void setup() {
        size(200, 200);
    }

    void draw() {
        background(255);

        noFill();
        stroke(0);

		using var path1 = new SKPath { };
		path1.MoveTo(coords[0]);
		var d1 = coords[0] - coords[1];
		var d2 = coords[1] - coords[2];
		path1.CubicTo(coords[1], coords[1] + d2, coords[1]);
		//path1.QuadTo(coords[2], coords[3]);
		//path1.QuadTo(coords[3], coords[4]);
		//path1.QuadTo(coords[4], coords[4]);
		Canvas.DrawPath(path1, Graphics.StrokePaint);

		//using var path2 = new SKPath { };
		//path2.MoveTo(coords[0]);
		//path2.CubicTo(coords[0], coords[1], coords[2]);
		//path2.CubicTo(coords[1], coords[3], coords[2]);
		//path2.CubicTo(coords[2], coords[4], coords[3]);
		//path2.CubicTo(coords[3], coords[4], coords[4]);
		//path2.CubicTo(coords[3], coords[4]);
		//Canvas.DrawPath(path2, Graphics.StrokePaint);


		//beginShape();
		//curveVertex(40, 40); // the first control point
		//curveVertex(40, 40); // is also the start point of curve
		//curveVertex(80, 60);
		//curveVertex(100, 100);
		//curveVertex(60, 120);
		//curveVertex(50, 150); // the last point of curve
		//curveVertex(50, 150); // is also the last control point
		//endShape();

		//using var path = CreateSpline(coords);
		//Canvas.DrawPath(path, Graphics.StrokePaint);

		// Use the array to keep the code shorter;
		// you already know how to draw ellipses!
		fill(255, 0, 0);
        noStroke();
        foreach(var p in coords) {
            ellipse(p.X, p.Y, 3, 3);
        }
    }

	public static SKPath CreateSpline(params SKPoint[] Points) {
		int i, c = Points.Length;
		if(c == 0)
			throw new ArgumentException("No points provided.", nameof(Points));

		var AppendTo = new SKPath();
		AppendTo.MoveTo(Points[0]);

		if(c == 1)
			return AppendTo;

		if(c == 2) {
			AppendTo.LineTo(Points[1]);
			return AppendTo;
		}

		double[] V = new double[c];

		for(i = 0; i < c; i++)
			V[i] = Points[i].X;

		GetCubicBezierCoefficients(V, out double[] Ax, out double[] Bx);

		for(i = 0; i < c; i++)
			V[i] = Points[i].Y;

		GetCubicBezierCoefficients(V, out double[] Ay, out double[] By);

		for(i = 0; i < c - 1; i++) {
			AppendTo.CubicTo((float)Ax[i], (float)Ay[i], (float)Bx[i], (float)By[i],
				Points[i + 1].X, Points[i + 1].Y);
		}

		return AppendTo;
	}

	public static void GetCubicBezierCoefficients(double[] V, out double[] A, out double[] B) {
		// Calculate Spline between points P[0], ..., P[N].
		// Divide into segments, B[0], ...., B[N-1] of cubic Bezier curves:
		//
		// B[i](t) = (1-t)³P[i] + 3t(1-t)²A[i] + 3t²(1-t)B[i] + t³P[i+1]
		//
		// B'[i](t) = (-3+6t-3t²)P[i]+(3-12t+9t²)A[i]+(6t-9t²)B[i]+3t²P[i+1]
		// B"[i](t) = (6-6t)P[i]+(-12+18t)A[i]+(6-18t)B[i]+6tP[i+1]
		//
		// Choose control points A[i] and B[i] such that:
		//
		// B'[i](1) = B'[i+1](0) => A[i+1]+B[i]=2P[i+1], i<N		(eq 1)
		// B"[i](1) = B"[i+1](0) => A[i]-2B[i]+2A[i+1]-B[i+1]=0		(eq 2)
		//
		// Also add the boundary conditions:
		//
		// B"[0](0)=0 => 2A[0]-B[0]=P[0]			(eq 3)
		// B"[N-1](1)=0 => -A[N-1]+2B[N-1]=P[N]		(eq 4)
		//
		// Method solves this linear equation for one coordinate of A[i] and B[i] at a time.
		//
		// First, the linear equation, is reduced downwards. Only coefficients close to
		// the diagonal, and in the right-most column need to be processed. Furthermore,
		// we don't have to store values we know are zero or one. Since number of operations
		// depend linearly on number of vertices, algorithm is O(N).

		int N = V.Length - 1;
		int N2 = N << 1;
		int i = 0;
		int j = 0;
		double r11, r12, r15;               // r13 & r14 always 0.
		double r22, r23, r25;               // r21 & r24 always 0 for all except last equation, where r21 is -1.
		double /*r31,*/ r32, r33, r34, r35;
		double[,] Rows = new double[N2, 3];
		double a;

		A = new double[N];
		B = new double[N];

		r11 = 2;        // eq 3
		r12 = -1;
		r15 = V[j++];

		r22 = 1;        // eq 1
		r23 = 1;
		r25 = 2 * V[j++];

		// r31 = 1;        // eq 2
		r32 = -2;
		r33 = 2;
		r34 = -1;
		r35 = 0;

		while(true) {
			a = 1 / r11;
			// r11 = 1;
			r12 *= a;
			r15 *= a;

			// r21 is always 0. No need to eliminate column.
			// r22 is always 1. No need to scale row.

			// r31 is always 1 at this point.
			// r31 -= r11;
			r32 -= r12;
			r35 -= r15;

			if(r32 != 0) {
				r33 -= r32 * r23;
				r35 -= r32 * r25;
				// r32 = 0;
			}

			// r33 is always 0.

			// r11 always 1.
			Rows[i, 0] = r12;
			Rows[i, 1] = 0;
			Rows[i, 2] = r15;
			i++;

			// r21, r24 always 0.
			Rows[i, 0] = r22;
			Rows[i, 1] = r23;
			Rows[i, 2] = r25;
			i++;

			if(i >= N2 - 2)
				break;

			r11 = r33;
			r12 = r34;
			r15 = r35;

			r22 = 1;        // eq 1
			r23 = 1;
			r25 = 2 * V[j++];

			// r31 = 1;        // eq 2
			r32 = -2;
			r33 = 2;
			r34 = -1;
			r35 = 0;
		}

		r11 = r33;
		r12 = r34;
		r15 = r35;

		//r21 = -1;		// eq 4
		r22 = 2;
		r23 = 0;
		r25 = V[j++];

		a = 1 / r11;
		//r11 = 1;
		r12 *= a;
		r15 *= a;

		//r21 += r11;
		r22 += r12;
		r25 += r15;

		r25 /= r22;
		r22 = 1;

		// r11 always 1.
		Rows[i, 0] = r12;
		Rows[i, 1] = 0;
		Rows[i, 2] = r15;
		i++;

		// r21 and r24 always 0.
		Rows[i, 0] = r22;
		Rows[i, 1] = r23;
		Rows[i, 2] = r25;
		i++;

		// Then eliminate back up:

		j--;
		while(i > 0) {
			i--;
			if(i < N2 - 1) {
				a = Rows[i, 1];
				if(a != 0) {
					Rows[i, 1] = 0;
					Rows[i, 2] -= a * Rows[i + 1, 2];
				}
			}

			B[--j] = Rows[i, 2];

			i--;
			a = Rows[i, 0];
			if(a != 0) {
				Rows[i, 0] = 0;
				Rows[i, 2] -= a * Rows[i + 1, 2];
			}

			A[j] = Rows[i, 2];
		}
	}
}
