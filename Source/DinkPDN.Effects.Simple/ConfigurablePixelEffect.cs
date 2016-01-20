using DinkPDN.Common;
using PaintDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DinkPDN.Effects.Simple
{
    [PluginSupportInfo(typeof(AssemblyPluginSupportInfo))]
    public abstract class ConfigurablePixelEffect : ConfigurablePixelEffect<AssemblyPluginSupportInfo>
    {
        protected ConfigurablePixelEffect() : base() { }

        protected ConfigurablePixelEffect(string menu) : base(menu) { }

        protected ConfigurablePixelEffect(string name, string menu) : base(name, menu) { }

        protected ConfigurablePixelEffect(string name, Image image, string menu) : base(name, image, menu) { }
    }

    public abstract class ConfigurablePixelEffect<TPluginInfo> : ConfigurableEffect<TPluginInfo>
        where TPluginInfo : IPluginConfiguration, new()
    {
        protected ConfigurablePixelEffect() : base() { }

        protected ConfigurablePixelEffect(string menu) : base(menu) { }

        protected ConfigurablePixelEffect(string name, string menu) : base(name, menu) { }

        protected ConfigurablePixelEffect(string name, Image image, string menu) : base(name, image, menu) { }

        protected override void RenderRects(Rectangle[] rects, RenderArgs dst, RenderArgs src)
        {
            var source = src.Surface;
            foreach (var rect in rects) {
                for (var y = rect.Top; y < rect.Bottom; y++) {
                    for (var x = rect.Left; x < rect.Right; x++) {
                        dst.Surface[x, y] = RenderPixel(x, y, source[x, y], source);
                    }
                }
            }
        }

        protected abstract ColorBgra RenderPixel(int x, int y, ColorBgra initial, Surface source);
    }
}
