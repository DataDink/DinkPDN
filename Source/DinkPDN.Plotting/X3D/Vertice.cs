using System;
using System.Linq;

namespace DinkPDN.Plotting.X3D
{
    public struct Vertice
    {
        public readonly double X;
        public readonly double Y;
        public readonly double Z;
        public readonly double Length;
        public readonly double XAxis;
        public readonly double YAxis;
        public readonly double ZAxis;

        public Vertice(double x, double y, double z) {
            X = x;
            Y = y;
            Z = z;
            Length = Math.Sqrt(X * X + Y * Y + Z * Z);
            XAxis = Angle(z, -y);
            YAxis = Angle(x, -z);
            ZAxis = Angle(x, -y);
        }

        public static Vertice operator +(Vertice a, Vertice b) { return new Vertice(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }
        public static Vertice operator -(Vertice a, Vertice b) { return new Vertice(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }
        public static Vertice operator *(Vertice a, Vertice b) { return new Vertice(a.X * b.X, a.Y * b.Y, a.Z * b.Z); }
        public static Vertice operator /(Vertice a, Vertice b) { return new Vertice(a.X / b.X, a.Y / b.Y, a.Z / b.Z); }
        public static Vertice operator %(Vertice a, Vertice b) { return new Vertice(a.X % b.X, a.Y % b.Y, a.Z % b.Z); }

        public static Vertice operator +(Vertice a, double b) { return new Vertice(a.X + b, a.Y + b, a.Z + b); }
        public static Vertice operator -(Vertice a, double b) { return new Vertice(a.X - b, a.Y - b, a.Z - b); }
        public static Vertice operator *(Vertice a, double b) { return new Vertice(a.X * b, a.Y * b, a.Z * b); }
        public static Vertice operator /(Vertice a, double b) { return new Vertice(a.X / b, a.Y / b, a.Z / b); }
        public static Vertice operator %(Vertice a, double b) { return new Vertice(a.X % b, a.Y % b, a.Z % b); }

        private static double Angle(double a, double b)
        {
            var deg = Math.Atan2(a, b) * 180 / Math.PI;
            while (deg < 0) deg += 360;
            return deg;
        }
    }

    public static class VerticeExtensions
    { 
        public static Vertice Negate(this Vertice v) { return new Vertice(-v.X, -v.Y, -v.Z); }

        public static Vertice ScaleTo(this Vertice v, double l) { return v * (l / v.Length); }

        //public static Vertice Project(this Vertice v, double axisX, double axisY, double axisZ, double length) { return v + Calculate(axisX, axisY, axisZ, length); }

        //public static void Rotate(double d, double a1, double b1, out double a2, out double b2)
        //{
        //    d *= Math.PI / 180d;
        //    a2 = Math.Round(Math.Cos(d) * a1 - Math.Sin(d) * b1, 15);
        //    b2 = Math.Round(Math.Sin(d) * a1 + Math.Cos(d) * b1, 15);
        //}

        //public static Vertice Calculate(double axisX, double axisY, double axisZ, double length)
        //{
        //    axisX = ToRadians(axisX); axisY = ToRadians(axisY); axisZ = ToRadians(axisZ);
        //    var normz = NormalizeRadians(axisZ, axisX); var normx = NormalizeRadians(axisX, axisZ);
        //    var x = Math.Round(Math.Sin(axisZ) * Math.Sin(normz), 15);
        //    var y = Math.Round(Math.Cos(axisZ) * Math.Sin(normz), 15);
        //    var z = Math.Round(Math.Sin(axisX) * Math.Sin(normx), 15);
            
        //    return new Vertice(x * length, y * length, z * length);
        //}

        //private static double ToRadians(double degrees) { return degrees * Math.PI / 180; }

        //private static double NormalizeRadians(double axis, double by)
        //{
        //    var norms = new[] { axis, by }
        //        .Select(v => Math.PI / 4 - Math.Abs(Math.PI / 4 - (v % Math.PI / 2)))
        //        .ToArray();
        //    return norms[0] / (norms.Sum()) * Math.PI / 2;
        //}
    }
}
