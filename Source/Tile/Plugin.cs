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

namespace Tile
{
    [PluginSupportInfo(typeof(AssemblyPluginSupportInfo))]
    public class Plugin : Effect
    {
        public Plugin() : base("Test", new AssemblyPluginSupportInfo().Icon, "Test", EffectFlags.Configurable) {
            var prop = new DoubleProperty("Test", 0d, 0d, 10d);
            var info = PropertyControlInfo.CreateFor(prop);
            info.SetPropertyControlValue("Test", ControlInfoPropertyNames.DisplayName, "Name");
            info.SetPropertyControlValue("Test", ControlInfoPropertyNames.Description, "Desc");

            var win = new Form();
            var control = (Control)info.CreateConcreteControl(win);
            control.Dock = DockStyle.Fill;
            win.Controls.Add((Control)control);
            win.ShowDialog();
        }

        public override void Render(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs, Rectangle[] rois, int startIndex, int length)
        {

        }
    }
}
