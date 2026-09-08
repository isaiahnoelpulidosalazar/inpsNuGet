using System;
using System.Collections.Generic;

namespace inpsNuGet;

public class AreaCalculator
{
    /// <summary>Calculates the area of a square (S = a²).</summary>
    public static double SquareArea(double a) => Math.Pow(a, 2);
    
    /// <summary>Calculates the area of a rectangle (S = a * b).</summary>
    public static double RectangleArea(double a, double b) => a * b;
    
    /// <summary>Calculates the area of a parallelogram (S = a * h).</summary>
    public static double ParallelogramArea(double a, double h) => a * h;
    
    /// <summary>Calculates the area of a general triangle (S = 1/2 * a * h).</summary>
    public static double TriangleArea(double a, double h) => 0.5 * a * h;
    
    /// <summary>Calculates the area of a right triangle (S = 1/2 * a * b).</summary>
    public static double RightTriangleArea(double a, double b) => 0.5 * a * b;
    
    /// <summary>Calculates the area of an equilateral triangle (S = (√3 / 4) * a²).</summary>
    public static double EquilateralTriangleArea(double a) => (Math.Sqrt(3) / 4.0) * Math.Pow(a, 2);
    
    /// <summary>Calculates the area of a trapezoid (S = ((a + b) / 2) * h).</summary>
    public static double TrapezoidArea(double a, double b, double h) => ((a + b) / 2.0) * h;
    
    /// <summary>Calculates the area of a rhombus given diagonals (S = (d1 * d2) / 2).</summary>
    public static double RhombusArea(double d1, double d2) => (d1 * d2) / 2.0;
    
    /// <summary>Calculates square area using its diagonal (S = d² / 2).</summary>
    public static double SquareAsRhombusArea(double d) => Math.Pow(d, 2) / 2.0;
    
    /// <summary>Calculates the area of a circle (S = π * r²).</summary>
    public static double CircleArea(double r) => Math.PI * Math.Pow(r, 2);
    
    /// <summary>Calculates the area of an annulus / ring (S = π * (R² - r²)).</summary>
    public static double AnnulusArea(double R, double r) => Math.PI * (Math.Pow(R, 2) - Math.Pow(r, 2));
    
    /// <summary>Calculates the area of a circular sector (S = (α / 360) * π * r²).</summary>
    public static double SectorArea(double r, double alphaDegrees) => (alphaDegrees / 360.0) * Math.PI * Math.Pow(r, 2);
    
    /// <summary>Calculates the area of a circular segment.</summary>
    public static double SegmentArea(double r, double alphaDegrees)
    {
        double alphaRadians = alphaDegrees * (Math.PI / 180.0);
        double sectorPart = (alphaDegrees / 360.0) * Math.PI * Math.Pow(r, 2);
        double trianglePart = 0.5 * Math.Pow(r, 2) * Math.Sin(alphaRadians);
        return sectorPart - trianglePart;
    }
    
    /// <summary>Calculates the area of an ellipse (S = π * a * b).</summary>
    public static double EllipseArea(double a, double b) => Math.PI * a * b;
    
    /// <summary>Calculates the area of a polygon using the Shoelace formula.</summary>
    public static double PolygonShoelaceArea(IList<(double X, double Y)> vertices)
    {
        int n = vertices.Count;
        if (n < 3)
        {
            return 0.0;
        }
        double area = 0.0;
        for (int i = 0; i < n; i++)
        {
            int j = (i + 1) % n;
            area += vertices[i].X * vertices[j].Y;
            area -= vertices[j].X * vertices[i].Y;
        }
        return Math.Abs(area) / 2.0;
    }
    
    /// <summary>Calculates the surface area of a cube (S = 6 * a²).</summary>
    public static double CubeSurfaceArea(double a) => 6.0 * Math.Pow(a, 2);
    
    /// <summary>Calculates the surface area of a rectangular parallelepiped (S = 2(ab + ac + bc)).</summary>
    public static double RectangularParallelepipedSurfaceArea(double a, double b, double c) => 2.0 * (a * b + a * c + b * c);
    
    /// <summary>Calculates total surface area of a cylinder (S = 2πr(r + h)).</summary>
    public static double CylinderSurfaceArea(double r, double h) => 2.0 * Math.PI * r * (r + h);
    
    /// <summary>Calculates total surface area of a cone (S = πr(r + l)).</summary>
    public static double ConeSurfaceArea(double r, double l) => Math.PI * r * (r + l);

    /// <summary>Calculates total surface area of a frustum of a cone.</summary>
    public static double FrustumSurfaceArea(double R, double r, double l)
    {
        double lateralArea = Math.PI * (R + r) * l;
        double baseAreas = Math.PI * (Math.Pow(R, 2) + Math.Pow(r, 2));
        return lateralArea + baseAreas;
    }

    /// <summary>Calculates the surface area of a sphere (S = 4πr²).</summary>
    public static double SphereSurfaceArea(double r) => 4.0 * Math.PI * Math.Pow(r, 2);
}