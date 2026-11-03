namespace inpsNuGet;

/// <summary>Area and surface-area formulas.</summary>
public static class AreaCalculator
{
    const double SqrtThreeOverFour = 0.43301270189221932; // sqrt(3) / 4

    /// <summary>S = a^2</summary>
    public static double SquareArea(double a) => a * a;
    /// <summary>S = a * b</summary>
    public static double RectangleArea(double a, double b) => a * b;
    /// <summary>S = a * h</summary>
    public static double ParallelogramArea(double a, double h) => a * h;
    /// <summary>S = 1/2 * a * h</summary>
    public static double TriangleArea(double a, double h) => 0.5 * a * h;
    /// <summary>S = 1/2 * a * b (legs)</summary>
    public static double RightTriangleArea(double a, double b) => 0.5 * a * b;
    /// <summary>S = (sqrt(3) / 4) * a^2</summary>
    public static double EquilateralTriangleArea(double a) => SqrtThreeOverFour * a * a;
    /// <summary>S = ((a + b) / 2) * h</summary>
    public static double TrapezoidArea(double a, double b, double h) => (a + b) / 2.0 * h;
    /// <summary>S = (d1 * d2) / 2</summary>
    public static double RhombusArea(double d1, double d2) => d1 * d2 / 2.0;
    /// <summary>Square from its diagonal: S = d^2 / 2</summary>
    public static double SquareAsRhombusArea(double d) => d * d / 2.0;
    /// <summary>S = pi * r^2</summary>
    public static double CircleArea(double r) => Math.PI * r * r;
    /// <summary>S = pi * (R^2 - r^2), computed as (R - r)(R + r) for numeric stability.</summary>
    public static double AnnulusArea(double R, double r) => Math.PI * (R - r) * (R + r);
    /// <summary>S = (alpha / 360) * pi * r^2</summary>
    public static double SectorArea(double r, double alphaDegrees) => alphaDegrees / 360.0 * Math.PI * r * r;

    /// <summary>Circular segment: sector minus triangle = r^2/2 * (alpha - sin(alpha)).</summary>
    public static double SegmentArea(double r, double alphaDegrees)
    {
        double rad = alphaDegrees * (Math.PI / 180.0);
        return 0.5 * r * r * (rad - Math.Sin(rad));
    }

    /// <summary>S = pi * a * b</summary>
    public static double EllipseArea(double a, double b) => Math.PI * a * b;

    /// <summary>Polygon area via the Shoelace formula. Fewer than 3 vertices returns 0.</summary>
    public static double PolygonShoelaceArea(IReadOnlyList<(double X, double Y)> vertices)
    {
        ArgumentNullException.ThrowIfNull(vertices);
        int n = vertices.Count;
        if (n < 3) return 0.0;

        double sum = 0.0;
        var prev = vertices[n - 1];
        for (int i = 0; i < n; i++)
        {
            var cur = vertices[i];
            sum += prev.X * cur.Y - cur.X * prev.Y;
            prev = cur;
        }
        return Math.Abs(sum) / 2.0;
    }

    /// <summary>S = 6 * a^2</summary>
    public static double CubeSurfaceArea(double a) => 6.0 * a * a;
    /// <summary>S = 2(ab + ac + bc)</summary>
    public static double RectangularParallelepipedSurfaceArea(double a, double b, double c) => 2.0 * (a * b + a * c + b * c);
    /// <summary>S = 2*pi*r*(r + h)</summary>
    public static double CylinderSurfaceArea(double r, double h) => 2.0 * Math.PI * r * (r + h);
    /// <summary>S = pi*r*(r + l), l = slant height</summary>
    public static double ConeSurfaceArea(double r, double l) => Math.PI * r * (r + l);
    /// <summary>Total surface of a conical frustum: pi*((R + r)*l + R^2 + r^2)</summary>
    public static double FrustumSurfaceArea(double R, double r, double l) => Math.PI * ((R + r) * l + R * R + r * r);
    /// <summary>S = 4*pi*r^2</summary>
    public static double SphereSurfaceArea(double r) => 4.0 * Math.PI * r * r;
}
