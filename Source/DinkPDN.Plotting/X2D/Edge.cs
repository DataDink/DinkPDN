using System;
using System.Collections.Generic;
using System.Text;

namespace DinkPDN.Plotting.X2D
{
    public struct Edge
    {
        public readonly Vertice From;
        public readonly Vertice To;
        public readonly double Length;
        public readonly double ZAxis;

        public Edge(Vertice from, Vertice to)
        {
            From = from; To = to;
            var normal = to - from;
            Length = normal.Length;
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
        public static Vertice? Intersect(this Edge a, Edge b)
        {
            var linea = new Line(a);
            var lineb = new Line(b);
            var zdet = linea.Y * lineb.X - lineb.Y * linea.X;
            if (zdet == 0d) return null;
            var x = (lineb.X * linea.XY - linea.X * lineb.XY) / zdet;
            var y = (linea.Y * lineb.XY - lineb.Y * linea.XY) / zdet;
            return new Vertice(x, y);
        }

        private struct Line
        {
            public readonly double X;
            public readonly double Y;
            public readonly double XY;

            public Line(Edge edge)
            {
                X = edge.From.X - edge.To.X;
                Y = edge.To.Y - edge.From.Y;
                XY = Y * edge.From.X + X * edge.From.Y;
            }
        }
    }
}
