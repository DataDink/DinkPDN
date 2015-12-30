using System;
using System.Collections.Generic;
using System.Text;

namespace DinkPDN.Plotting.X3D
{
    public struct Edge
    {
        public readonly Vertice From;
        public readonly Vertice To;
        public readonly double Length;
        public readonly double XAxis;
        public readonly double YAxis;
        public readonly double ZAxis;

        public Edge(Vertice from, Vertice to)
        {
            From = from; To = to;
            var normal = to - from;
            Length = normal.Length;
            XAxis = normal.XAxis;
            YAxis = normal.YAxis;
            ZAxis = normal.ZAxis;
        }

        public static Edge operator +(Edge a, Edge b) { return new Edge(a.From + b.From, a.To + b.To); }
        public static Edge operator -(Edge a, Edge b) { return new Edge(a.From - b.From, a.To - b.To); }
        public static Edge operator *(Edge a, Edge b) { return new Edge(a.From * b.From, a.To * b.To); }
        public static Edge operator /(Edge a, Edge b) { return new Edge(a.From / b.From, a.To / b.To); }
        public static Edge operator %(Edge a, Edge b) { return new Edge(a.From % b.From, a.To % b.To); }
    }

    public static class EdgeExtensions
    {
        //public static Vertice? Intersect(this Edge a, Edge b)
        //{
        //    var linea = new Line(a);
        //    var lineb = new Line(b);
        //    var zdet = linea.Y * lineb.X - lineb.Y * linea.X;
        //    if (zdet == 0d) return null;
        //    var x = (lineb.X * linea.XY - linea.X * lineb.XY) / zdet;
        //    var y = (linea.Y * lineb.XY - lineb.Y * linea.XY) / zdet;
        //    var xdet = linea.Y * lineb.Z - linea.Y * lineb.Z;
        //    if (xdet == 0d) return null;
        //    var z = (lineb.Z * linea.ZY - linea.Z * lineb.ZY) / xdet;
        //    return new Vertice(x, y, z);
        //}
        
        private struct Line
        {
            public readonly double X;
            public readonly double Y;
            public readonly double Z;
            public readonly double XY;
            public readonly double ZY;

            public Line(Edge edge)
            {
                X = edge.From.X - edge.To.X;
                Y = edge.To.Y - edge.From.Y;
                Z = edge.From.Z - edge.To.Z;
                XY = Y * edge.From.X + X * edge.From.Y;
                ZY = Y * edge.From.Z + Z * edge.From.Y;
            }
        }
    }
}
