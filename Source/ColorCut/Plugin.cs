using DinkPDN.Effects.Simple;
using PaintDotNet;
using System;

namespace ColorCut
{
    public class Plugin : ConfigurablePixelEffect
    {
        public Plugin() : base("Color") { }
        private ColorBgra BgColor { get; set; }
        private double BgShade { get; set; } 
        private ColorBgra BgNormal { get; set; }

        protected override void OnReady()
        {
            BgColor = EnvironmentParameters.SecondaryColor;
            BgShade = BgColor.ToColor().GetBrightness();
            BgNormal = Normalize(BgColor);
        }

        [ConfigurableInt(
            Name = "Color Threshold", 
            Description = "The max deviation in color from the background a pixel can have to be affected.",
            Default = 40, Min = 0, Max = 256, 
            MicroStep = 1, SmallStep = 8, LargeStep = 16)]
        public int ColorThreshold { get; set; }

        [ConfigurableDouble(
            Name = "Shade Threshold",
            Description = "The max deviation in shade from the background a pixel can have to be affected.",
            Default = 1d)]
        public double ShadeThreshold { get; set; }

        [ConfigurableDouble(
            Name = "Alpha Falloff",
            Description = "The ratio at which the alpha channel will be tapered off.",
            Default = 0.5d)]
        public double AlphaFalloff { get; set; }

        protected override ColorBgra RenderPixel(int x, int y, ColorBgra initial, Surface source)
        {
            var shade = initial.ToColor().GetBrightness();
            if (Math.Abs(shade - BgShade) > ShadeThreshold) return initial;

            var norm = Normalize(initial);
            if (Math.Abs(BgNormal.R - norm.R) > ColorThreshold
                || Math.Abs(BgNormal.G - norm.G) > ColorThreshold
                || Math.Abs(BgNormal.B - norm.B) > ColorThreshold) {
                return initial;
            }

            var alpha = (byte)(Math.Min(255, Math.Max(0,
                Math.Max(Math.Max(
                    Math.Abs(BgColor.R - initial.R),
                    Math.Abs(BgColor.G - initial.G)),
                    Math.Abs(BgColor.B - initial.B)))
                * (1d - AlphaFalloff)));
            return ColorBgra.FromBgra(initial.B, initial.G, initial.R, alpha);
        }

        private static ColorBgra Normalize(ColorBgra color)
        {
            var nbase = Math.Min(Math.Min(color.R, color.G), color.B);
            return ColorBgra.FromBgr((byte)(color.B - nbase), (byte)(color.G - nbase), (byte)(color.R - nbase));
        }
    }
}
