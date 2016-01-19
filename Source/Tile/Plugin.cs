using DinkPDN.Effects.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaintDotNet;
using DinkPDN.UI.Controls;
using PaintDotNet.PropertySystem;
using PaintDotNet.IndirectUI;

namespace Tile
{
    public class Plugin : ConfigurablePixelEffect
    {
        public Plugin() : base("Test") { }

        protected override ColorBgra RenderPixel(int x, int y, ColorBgra initial, Surface source)
        {
            var stuff = PropertyControlDescriptor.All;
            var prop = new DoubleProperty("Test", 0d, 0d, 1d);
            var info = PropertyControlInfo.CreateFor(prop);
            var ctrl = (PropertyControl)Activator.CreateInstance(stuff.First().ControlType, info);
            ctrl.Property.
            return initial;
        }
    }
}
