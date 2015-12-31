using System;

namespace PaintDotNet
{
    public static class ColorBgraExtensions
    {
        public static ColorBgra Blend(this ColorBgra a, ColorBgra b, double strength = 0.5d)
        {
            var bstr = Math.Min(1d, Math.Max(0d, strength));
            var astr = 1 - bstr;
            return ColorBgra.FromBgra(
                (byte)(a.B * astr + b.B * bstr),
                (byte)(a.G * astr + b.G * bstr),
                (byte)(a.R * astr + b.R * bstr),
                (byte)(a.A * astr + b.A * bstr));
        }
    }
}
