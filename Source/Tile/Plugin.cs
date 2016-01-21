using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaintDotNet;
using PaintDotNet.PropertySystem;
using PaintDotNet.IndirectUI;
using PaintDotNet.Effects;
using DinkPDN.Common;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using DinkPDN.Effects.Simple;

namespace Tile
{
    [PluginSupportInfo(typeof(AssemblyPluginSupportInfo))]
    public class Plugin : ConfigurableEffect
    {
        [ConfigurableDouble(Name = "Test Test")]
        public double Value { get; set; }

        private readonly Random Rnd = new Random();

        protected override void Render(Rectangle[] rects, RenderArgs dst, RenderArgs src)
        {
            foreach (var rect in rects) 
                for (var y = rect.Top; y < rect.Bottom; y++) 
                    for (var x = rect.Left; x < rect.Right; x++) {
                        var color = ColorBgra.FromBgra(
                            (byte)(Rnd.Next(255)),
                            (byte)(Rnd.Next(255)),
                            (byte)(Rnd.Next(255)),
                            (byte)(Rnd.Next(255)));
                        src.Surface[x, y] = color;
                    }
        }
    }
}
