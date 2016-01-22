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
        private Surface Source { get; set; }

        [ConfigurableDouble(
            Name = "Horizontal Scale",
            Description = "Set this to 0 to maintain aspect with Vertical Scale"
        )]
        public double HorizontalScale { get; set; }

        [ConfigurableDouble(
            Name = "Vertical Scale",
            Description = "Set this to 0 to maintain aspect with Horizontal Scale"
        )]
        public double VerticalScale { get; set; }

        [ConfigurableDouble(
            Name = "Horizontal Offset",
            Default = 0d
        )]
        public double HorizontalOffset { get; set; }

        [ConfigurableDouble(
            Name = "Vertical Offset",
            Default = 0d
        )]
        public double VerticalOffset { get; set; }

        [ConfigurableBool(
            Name = "Mirror Horizontal"
        )]
        public bool HorizontalMirror { get; set; }

        [ConfigurableBool(
            Name = "Mirror Vertical"
        )]
        public bool VerticalMirror { get; set; }

        [ConfigurableFile(
            Name = "Load From Disk"
        )]
        public string FilePath { get; set; }

        protected override void Render(Rectangle[] rects, RenderArgs dst, RenderArgs src)
        {
        }
    }
}
