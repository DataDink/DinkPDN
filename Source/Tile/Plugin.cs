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
    public class Plugin : ConfigurablePixelEffect
    {
        [ConfigurableDouble(Name = "Test Test")]
        public double Value { get; set; }

        protected override ColorBgra RenderPixel(int x, int y, ColorBgra initial, Surface source)
        {
            return ColorBgra.Black;
        }
    }
}
