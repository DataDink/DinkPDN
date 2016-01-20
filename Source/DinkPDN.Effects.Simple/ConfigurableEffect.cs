using DinkPDN.Common;
using PaintDotNet;
using PaintDotNet.Effects;
using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DinkPDN.Effects.Simple
{
    [PluginSupportInfo(typeof(AssemblyPluginSupportInfo))]
    public abstract class ConfigurableEffect : ConfigurableEffect<AssemblyPluginSupportInfo>
    {
        protected ConfigurableEffect() : base() { }
        protected ConfigurableEffect(string menu) : base(menu) { }
        protected ConfigurableEffect(string name, string menu) : base(name, menu) { }
        protected ConfigurableEffect(string name, Image image, string menu) : base(name, image, menu) { }
    }

    public abstract class ConfigurableEffect<TPluginInfo> : BaseConfigurableEffect
        where TPluginInfo : IPluginConfiguration, new()
    {
        private static readonly TPluginInfo PluginInfo = new TPluginInfo();
        protected ConfigurableEffect() : this(null) { }
        protected ConfigurableEffect(string menu) : this(PluginInfo.DisplayName, menu) { }
        protected ConfigurableEffect(string name, string menu) : this(name, PluginInfo.Icon, menu) { }
        protected ConfigurableEffect(string name, Image image, string menu) : base(name, image, menu) { }
    }

    public abstract class BaseConfigurableEffect : Effect, INotifyPropertyChanged
    {
        private readonly Type Type;
        private readonly Dictionary<PropertyInfo, ConfigurableAttribute> Properties;
        protected RenderArgs Source { get; private set; }
        protected RenderArgs Destination { get; private set; }
        protected Rectangle[] Rects { get; private set; }

        protected BaseConfigurableEffect(string name, Image image, string menu) : base(name, image, menu, EffectFlags.Configurable) {
            Properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new { prop = p, attr = p.GetCustomAttribute<ConfigurableAttribute>(true) })
                .Where(p => p.attr != null && p.prop.CanWrite)
                .ToDictionary(p => p.prop, p => p.attr);
        }

        protected virtual void OnPropertyChanged(PropertyInfo property, object oldValue, object newValue)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(property.Name));
            RenderRects(Rects, Destination, Source);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override EffectConfigDialog CreateConfigDialog()
        {
            var dlg = new Dialog();
            foreach (var kvp in Properties) {
                var prop = kvp.Key;
                var attr = kvp.Value;
                var control = attr.Create(prop);
                control.Property.ValueChanged += (s, e) => {
                    var oldvalue = prop.GetValue(this, null);
                    prop.SetValue(this, control.Property.Value);
                    OnPropertyChanged(prop, oldvalue, control.Property.Value);
                };
                control.Dock = DockStyle.Top;
                dlg.Controls.Add(control);
                control.SendToBack();
            }
            return dlg;
        }

        protected virtual void OnReady(RenderArgs src, RenderArgs dst) { }

        public override void Render(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs, Rectangle[] rois, int startIndex, int length)
        {
            Source = srcArgs;
            Destination = dstArgs;
            Rects = rois.Skip(startIndex).Take(length).ToArray();
            OnReady(Source, Destination);

            RenderRects(Rects, Destination, Source);
        }

        protected abstract void RenderRects(Rectangle[] rects, RenderArgs dst, RenderArgs src);

        private class Dialog : EffectConfigDialog {
            public Dialog()
            {
                AutoSize = true;
                AutoSizeMode = AutoSizeMode.GrowOnly;
                MinimumSize = new Size(400, 200);
            }
        }
    }
}
