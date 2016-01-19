using DinkPDN.Common;
using PaintDotNet;
using PaintDotNet.Effects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DinkPDN.UI.Controls;

namespace DinkPDN.Effects.Extended
{
    public class ConfigurableEffect<TPluginInfo> : Effect
        where TPluginInfo : IPluginConfiguration, new()
    {
        private static readonly TPluginInfo PluginInfo = new TPluginInfo();
        protected ConfigurableEffect() : this(null) { }
        protected ConfigurableEffect(string menu) : this(PluginInfo.DisplayName, menu) { }
        protected ConfigurableEffect(string name, string menu) : this(name, PluginInfo.Icon, menu) { }
        protected ConfigurableEffect(string name, Image image, string menu) : base(name, image, menu) { }

        public override void Render(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs, Rectangle[] rois, int startIndex, int length)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class BaseConfigurableEffect : Effect
    {
        private readonly Type Type;
        private readonly Dictionary<PropertyInfo, ConfigurableAttribute> Properties;

        protected BaseConfigurableEffect(string name, Image image, string menu) : base(name, image, menu, EffectFlags.Configurable)
        {
            Properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new { prop = p, attr = p.GetCustomAttribute<ConfigurableAttribute>(true) })
                .Where(p => p.attr != null && p.prop.CanWrite)
                .ToDictionary(p => p.prop, p => p.attr);
        }

        protected class ConfigurableAttribute : Attribute
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public PropertyControlDescriptor.ExperienceTypes Experience { get; set; }
            public object Default { get; set; }
            public IComparable Min { get; set; }
            public IComparable Max { get; set; }
            public IComparable MicroStep { get; set; }
            public IComparable SmallStep { get; set; }
            public IComparable LargeStep { get; set; }
            public object Precision { get; set; }
            public bool Multiline { get; set; }
            public string DelimitedValues { get; set; }
            public virtual Control Create(PropertyInfo property)
            {
                var control = 
            }
        }
    }
}
