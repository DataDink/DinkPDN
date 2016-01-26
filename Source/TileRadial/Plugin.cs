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

namespace TileRadial
{
    public class Plugin : ConfigurablePixelEffect
    {
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
            Name = "Start Angle",
            Default = 0d,
            Min = 0d, Max = 360d
        )]
        public double StartAngle { get; set; }

        [ConfigurableDouble(
            Name = "Angle Step",
            Default = 45d,
            Min = .01d, Max = 360d
        )]
        public double AngleStep { get; set; }

        [ConfigurableDouble]
        public double Radius { get; set; }

        [ConfigurableBool(
            Name = "Ignore Tile Shape",
            Description = "The radius will ignore the tile height/width ratio"
        )]
        public bool IgnoreTileShape { get; set; }

        [ConfigurableBool(
            Name = "From Clipboard"
        )]
        public bool UseClipboard { get; set; }

        [ConfigurableFile(
            Name = "From File"
        )]
        public string FilePath { get; set; }

        public Plugin() : base("Fill") { }

        private readonly object SourceLock = new object();

        private Image _tileSource;
        private Image TileSource
        {
            get { return _tileSource; }
            set
            {
                lock(SourceLock) {
                    if (_tileSource != null) _tileSource.Dispose();
                    _tileSource = value;
                }
            }
        }

        private Surface _renderSource;
        private Surface RenderSource
        {
            get { return _renderSource; }
            set
            {
                lock (SourceLock) {
                    if (_renderSource != null) _renderSource.Dispose();
                    _renderSource = value;
                }
            }
        }

        private Rectangle Bounds { get; set; }

        protected override void OnPropertyChanged(PropertyInfo property, object oldValue, object newValue)
        {
            if (property.Name == "FilePath") LoadFile();
            if (property.Name == "UseClipboard") LoadClipboard();
            base.OnPropertyChanged(property, oldValue, newValue);
        }

        private void LoadFile() { try { TileSource = Image.FromFile(FilePath); } catch { TileSource = null; } }

        private void LoadClipboard()
        {
            if (UseClipboard) {
                try { TileSource = Clipboard.GetImage(); } catch { TileSource = null; }
            } else { TileSource = null; }
            if (TileSource == null) LoadFile();
        }

        protected override void OnSetRenderInfo(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            base.OnSetRenderInfo(parameters, dstArgs, srcArgs);

            Bounds = EnvironmentParameters.GetSelection(srcArgs.Bounds).GetBoundsInt();
            dstArgs.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

            if (TileSource == null) {
                TileSource = new Bitmap(Bounds.Width, Bounds.Height);
                using (var gfx = Graphics.FromImage(TileSource)) {
                    gfx.DrawImage(srcArgs.Bitmap, -Bounds.X, -Bounds.Y);
                }
            }

            var tileWidth = Math.Max(1, HorizontalScale > 0
                ? HorizontalScale * Bounds.Width
                : VerticalScale == 0 ? TileSource.Width
                : Bounds.Width * (VerticalScale * Bounds.Height) / Bounds.Height);
            var tileHeight = Math.Max(1, VerticalScale > 0
                ? VerticalScale * Bounds.Height
                : HorizontalScale == 0 ? TileSource.Height
                : Bounds.Height * (HorizontalScale * Bounds.Width) / Bounds.Width);
            var startX = HorizontalOffset == 0 ? 0 : tileWidth * HorizontalOffset - tileWidth;
            var startY = VerticalOffset == 0 ? 0 : tileHeight * VerticalOffset - tileHeight;

            using (var scaled = new Bitmap(TileSource, (int)tileWidth, (int)tileHeight))
            using (var result = new Bitmap(Bounds.Width, Bounds.Height))
            using (var gfx = Graphics.FromImage(result)) {
                for (var y = (int)startY; y < result.Height; y += (int)tileHeight)
                    for (var x = (int)startX; x < result.Width; x += (int)tileWidth) {
                        gfx.DrawImage(scaled, x, y);
                    }
                RenderSource = Surface.CopyFromBitmap(result);
            }

        }

        protected override ColorBgra Render(int x, int y, ColorBgra initial, Surface source)
        {
            return RenderSource[x - Bounds.Left, y - Bounds.Top];
        }
    }
}
