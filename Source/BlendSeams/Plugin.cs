using DinkPDN.Effects.Simple;
using System.Linq;
using PaintDotNet;
using System;
using PaintDotNet.Effects;
using System.Drawing;

namespace BlendSeams
{
    public class Plugin : ConfigurablePixelEffect
    {
        public Plugin() : base(SubmenuNames.Render) { }

        private Rectangle Bounds { get; set; }
        private double CenterX { get; set; }
        private double CenterY { get; set; }

        [ConfigurableDouble(
            Name = "Border Radius",
            Description = "Adjusts the bordering around the original sample",
            Default = 0.75d)]
        public double BlendRadius { get; set; }
        [ConfigurableDouble(
            Name = "Border Softness",
            Description = "Adjusts the softness between the border and the original sample",
            Default = 1d)]
        public double RadiusBlending { get; set; }

        protected override void OnReady()
        {
            Bounds = EnvironmentParameters.GetSelection(SrcArgs.Bounds).GetBoundsInt();
            CenterX = Bounds.Left + Bounds.Width / 2;
            CenterY = Bounds.Top + Bounds.Height / 2;
        }
        
        protected override ColorBgra RenderPixel(int x, int y, ColorBgra initial, Surface source)
        {
            var targetA = source[Offset(Bounds.Left, x, Bounds.Right), y];
            initial = initial.Blend(targetA, GetBorderBlendValue(Bounds.Left, x, Bounds.Right));

            var targetC = source[Offset(Bounds.Left, x, Bounds.Right), Offset(Bounds.Top, y, Bounds.Bottom)];
            var targetB = source[x, Offset(Bounds.Top, y, Bounds.Bottom)];
            targetB = targetB.Blend(targetC, GetBorderBlendValue(Bounds.Left, x, Bounds.Right));
            initial = initial.Blend(targetB, GetBorderBlendValue(Bounds.Top, y, Bounds.Bottom));

            return initial;
        }

        private double GetBorderBlendValue(double min, double value, double max)
        {
            value -= min; max -= min;
            var radius = Math.Abs(value - max / 2) / max * 2;
            if (radius < BlendRadius) return 0d;
            var blend = (1d - BlendRadius) * RadiusBlending;
            return Math.Min(1d, (radius - BlendRadius) / blend);
        }

        private int Offset(int min, int value, int max) {
            value -= min; max -= min + 1;
            return ((value + max / 2) % max) + min;
        }
    }
}
