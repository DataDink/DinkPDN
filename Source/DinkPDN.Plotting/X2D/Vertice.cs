using System;
using System.Linq;

namespace DinkPDN.Plotting.X2D
{
    public struct Vertice
    {
        public readonly double X;
        public readonly double Y;
        public readonly double Length;
        public readonly double ZAxis;

        public Vertice(double x, double y)
        {
            X = x;
            Y = y;
            Length = Math.Sqrt(X * X + Y * Y);
            ZAxis = Angle(x, -y);
        }

        public static Vertice operator +(Vertice a, Vertice b) { return new Vertice(a.X + b.X, a.Y + b.Y); }
        public static Vertice operator -(Vertice a, Vertice b) { return new Vertice(a.X - b.X, a.Y - b.Y); }
        public static Vertice operator *(Vertice a, Vertice b) { return new Vertice(a.X * b.X, a.Y * b.Y); }
        public static Vertice operator /(Vertice a, Vertice b) { return new Vertice(a.X / b.X, a.Y / b.Y); }
        public static Vertice operator %(Vertice a, Vertice b) { return new Vertice(a.X % b.X, a.Y % b.Y); }

        public static Vertice operator +(Vertice a, double b) { return new Vertice(a.X + b, a.Y + b); }
        public static Vertice operator -(Vertice a, double b) { return new Vertice(a.X - b, a.Y - b); }
        public static Vertice operator *(Vertice a, double b) { return new Vertice(a.X * b, a.Y * b); }
        public static Vertice operator /(Vertice a, double b) { return new Vertice(a.X / b, a.Y / b); }
        public static Vertice operator %(Vertice a, double b) { return new Vertice(a.X % b, a.Y % b); }

        private static double Angle(double a, double b)
        {
            var deg = Math.Atan2(a, b) * 180 / Math.PI;
            while (deg < 0) deg += 360;
            return deg;
        }
    }

    public static class VerticeExtensions
    {
        public static Vertice Negate(this Vertice v) { return new Vertice(-v.X, -v.Y); }

        public static Vertice ScaleTo(this Vertice v, double l) { return v * (l / v.Length); }

        public static Vertice Project(this Vertice v, double axisZ, double length) { return v + Calculate(axisZ, length); }

        public static void Rotate(double d, double a1, double b1, out double a2, out double b2)
        {
            d *= Math.PI / 180d;
            a2 = Math.Round(Math.Cos(d) * a1 - Math.Sin(d) * b1, 15);
            b2 = Math.Round(Math.Sin(d) * a1 + Math.Cos(d) * b1, 15);
        }

        public static Vertice Calculate(double axisZ, double length)
        {
            axisZ = ToRadians(axisZ);
            var x = Math.Sin(axisZ);
            var y = Math.Cos(axisZ);
            return new Vertice(x * length, -y * length);
        }

        private static double ToRadians(double degrees) { return degrees * Math.PI / 180; }
    }
}
