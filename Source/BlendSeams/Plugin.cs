using DinkPDN.Effects.Simple;
using System.Linq;
using PaintDotNet;
using DinkPDN.Plotting.X2D;
using System;
using PaintDotNet.Effects;

namespace BlendSeams
{
    public class Plugin : ConfigurablePixelEffect
    {
        public Plugin() : base(SubmenuNames.Render) { }

        private Vertice Center { get; set; }
        private Edge Top { get; set; }
        private Edge Left { get; set; }
        private Edge Bottom { get; set; }
        private Edge Right { get; set; }
        private double NW { get; set; }
        private double NE { get; set; }
        private double SE { get; set; }
        private double SW { get; set; }

        [ConfigurableDouble(
            Name = "Border Radius",
            Description = "Adjusts the bordering around the original sample",
            Default = 0.5d)]
        public double Radius { get; set; }
        [ConfigurableDouble(
            Name = "Border Softness",
            Description = "Adjusts the softness between the border and the original sample",
            Default = 0.5)]
        public double RadiusBlending { get; set; }
        [ConfigurableDouble(
            Name = "Corner Softness",
            Description = "Adjusts the softness between border edges",
            Default = .1)]
        public double BorderBlending { get; set; }

        protected override void OnReady()
        {
            Top = new Edge(new Vertice(SrcArgs.Surface.Bounds.Left, SrcArgs.Surface.Bounds.Top), new Vertice(SrcArgs.Surface.Bounds.Right, SrcArgs.Surface.Bounds.Top));
            Left = new Edge(new Vertice(SrcArgs.Surface.Bounds.Left, SrcArgs.Surface.Bounds.Top), new Vertice(SrcArgs.Surface.Bounds.Left, SrcArgs.Surface.Bounds.Bottom));
            Bottom = new Edge(new Vertice(SrcArgs.Surface.Bounds.Left, SrcArgs.Surface.Bounds.Bottom), new Vertice(SrcArgs.Surface.Bounds.Right, SrcArgs.Surface.Bounds.Bottom));
            Right = new Edge(new Vertice(SrcArgs.Surface.Bounds.Right, SrcArgs.Surface.Bounds.Top), new Vertice(SrcArgs.Surface.Bounds.Right, SrcArgs.Surface.Bounds.Bottom));
            Center = new Vertice(SrcArgs.Bounds.Width / 2, SrcArgs.Bounds.Height / 2);
            NW = (Top.From - Center).ZAxis;
            NE = (Top.To - Center).ZAxis;
            SE = (Bottom.To - Center).ZAxis;
            SW = (Bottom.From - Center).ZAxis;
        }

        protected override ColorBgra RenderPixel(int x, int y, ColorBgra initial, Surface source)
        {
            var pos = new Vertice(x, y);
            var inner = new Edge(new Vertice(source.Bounds.Width / 2, source.Bounds.Height / 2), pos);
            var quad = GetQuadrant(inner.ZAxis);
            var outer = new Edge(pos, GetValueByQuadrant(quad,
                Top.Intersect(inner),
                Right.Intersect(inner),
                Bottom.Intersect(inner),
                Left.Intersect(inner)).Value);
            var max = inner.Length + outer.Length;
            var dist = inner.Length / max;
            var radiusBlend = Math.Max(0, dist - Radius) / (1 - Radius);

            var xshift = (int)((x - source.Bounds.Left + source.Bounds.Width / 2d) % (source.Bounds.Width - 1)) + source.Bounds.Left;
            var yshift = (int)((y - source.Bounds.Top + source.Bounds.Height / 2d) % (source.Bounds.Height - 1)) + source.Bounds.Top;
            var borderColor = GetValueByQuadrant(quad,
                source[xshift, y],
                source[x, yshift]);
            var blendColor = GetValueByQuadrant(quad,
                source[x, yshift],
                source[xshift, y]);
            var borderBlend = GetValueByQuadrant(quad,
                GetBlendValue(NW, inner.ZAxis, NE),
                GetBlendValue(NE, inner.ZAxis, SE),
                GetBlendValue(SE, inner.ZAxis, SW),
                GetBlendValue(SW, inner.ZAxis, NW));
            borderColor = borderColor.Blend(blendColor, borderBlend * BorderBlending);

            return borderColor.Blend(initial, (1 - radiusBlend) * RadiusBlending);
        }

        private double GetBlendValue(double min, double actual, double max)
        {
            while (min > actual) min -= 360;
            while (max < actual) max += 360;
            var range = max - min;
            var value = (actual - min) / range * 2;
            return 1d - Math.Abs(1d - value);
        }

        private T GetValueByQuadrant<T>(Quadrant quad, T top, T right, T bottom, T left)
        {
            return quad == Quadrant.Top ? top
                : quad == Quadrant.Right ? right
                : quad == Quadrant.Bottom ? bottom
                : left;
        }

        private T GetValueByQuadrant<T>(Quadrant quad, T topbot, T rightleft)
        {
            return GetValueByQuadrant(quad, topbot, rightleft, topbot, rightleft);
        }

        private Quadrant GetQuadrant(double degrees)
        {
            if (degrees < NE) return Quadrant.Top;
            if (degrees < SE) return Quadrant.Right;
            if (degrees < SW) return Quadrant.Bottom;
            if (degrees < NW) return Quadrant.Left;
            return Quadrant.Top;
        }

        private enum Quadrant { Top, Right, Bottom, Left }
    }
}
