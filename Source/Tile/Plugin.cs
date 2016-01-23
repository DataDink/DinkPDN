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
using DinkPDN.Effects.UI;

namespace Tile
{
    [PluginSupportInfo(typeof(AssemblyPluginSupportInfo))]
    public class Plugin : ConfigurableEffect
    {
        public Plugin() : base("Fill") { }

        private Image Source { get; set; }

        private void SetSource(Image source)
        {
            if (Source != null) Source.Dispose();
            Source = source;
        }

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
            Name = "From Clipboard"
        )]
        public bool UseClipboard { get; set; }

        [ConfigurableFile(
            Name = "From File"
        )]
        public string FilePath { get; set; }

        protected override void Render(Rectangle[] rects, RenderArgs dst, RenderArgs src) { }

        protected override void OnPropertyChanged(PropertyInfo property, object oldValue, object newValue)
        {
            if (property.Name == "FilePath") LoadFile();
            if (property.Name == "UseClipboard") LoadClipboard();
            base.OnPropertyChanged(property, oldValue, newValue);
        }

        private void LoadFile() { try { SetSource(Image.FromFile(FilePath)); } catch { SetSource(null); } }

        private void LoadClipboard()
        {
            if (UseClipboard) {
                try { SetSource(Clipboard.GetImage()); } catch { SetSource(null); }
            } else { SetSource(null); }
            if (Source == null) LoadFile();
        }

        protected override void OnSetRenderInfo(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            base.OnSetRenderInfo(parameters, dstArgs, srcArgs);
            var bounds = EnvironmentParameters.GetSelection(srcArgs.Bounds).GetBoundsInt();

            if (Source == null) {
                Source = new Bitmap(bounds.Width, bounds.Height);
                using (var gfx = Graphics.FromImage(Source)) {
                    gfx.DrawImage(srcArgs.Bitmap, -bounds.X, -bounds.Y);
                }
            }

            var tileWidth = Math.Max(1, HorizontalScale > 0
                ? HorizontalScale * bounds.Width
                : VerticalScale == 0 ? Source.Width
                : bounds.Width * (VerticalScale * bounds.Height) / bounds.Height);
            var tileHeight = Math.Max(1, VerticalScale > 0
                ? VerticalScale * bounds.Height
                : HorizontalScale == 0 ? Source.Height
                : bounds.Height * (HorizontalScale * bounds.Width) / bounds.Width);
            var startX = HorizontalOffset == 0 ? 0 : tileWidth * HorizontalOffset - tileWidth;
            var startY = VerticalOffset == 0 ? 0 : tileHeight * VerticalOffset - tileHeight;

            using (var scaled = new Bitmap(Source, (int)tileWidth, (int)tileHeight))
            using (var result = new Bitmap(bounds.Width, bounds.Height))
            using (var gfx = Graphics.FromImage(result)) {
                dstArgs.Graphics.CompositingMode = gfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                dstArgs.Graphics.FillRectangle(Brushes.Transparent, bounds);
                for (var y = (int)startY; y < result.Height; y += (int)tileHeight)
                    for (var x = (int)startX; x < result.Width; x += (int)tileWidth) {
                        gfx.DrawImage(scaled, x, y);
                    }
                dstArgs.Graphics.DrawImage(result, bounds.Left, bounds.Top);
            }

        }
    }
}
