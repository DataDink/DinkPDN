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
using PaintDotNet.PropertySystem;
using System.Linq;

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

        protected abstract class ConfigurableAttribute : Attribute
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public abstract Control Create(PropertyInfo property);
            protected Control CreateHost<TType>(TType property) where TType : Property
            {
                var descriptor = PropertyControlDescriptor.All.First(d => d.PropertyType.IsAssignableFrom(typeof(TType)));
                return new PropertyControlHost(descriptor, property);
            }
        }

        protected class ConfigurableImageAttribute : ConfigurableAttribute
        {
            public override Control Create(PropertyInfo property) {
                return CreateHost(
                    new ImageProperty(Name)
                );
            }
        }

        protected class ConfigurableVectorAttribute : ConfigurableAttribute
        {
            public double X { get; set; }
            public double Y { get; set; }
            public object MaxX { get; set; }
            public object MaxY { get; set; }
            public object MinX { get; set; }
            public object MinY { get; set; }

            public override Control Create(PropertyInfo property)
            {
                return CreateHost(
                    new DoubleVectorProperty(
                        property.Name,
                        new Pair<double, double>(X, Y),
                        new Pair<double, double>(MinX.GetValue(double.MinValue), MinY.GetValue(double.MinValue)),
                        new Pair<double, double>(MaxX.GetValue(double.MaxValue), MaxY.GetValue(double.MaxValue)))
                );
            }
        }

        protected class ConfigurableVector3Attribute : ConfigurableVectorAttribute
        {
            public double Z { get; set; }
            public object MaxZ { get; set; }
            public object MinZ { get; set; }

            public override Control Create(PropertyInfo property)
            {
                return CreateHost(
                    new DoubleVector3Property(
                        property.Name,
                        new Tuple<double, double, double>(X, Y, Z),
                        new Tuple<double, double, double>(MinX.GetValue(double.MinValue), MinY.GetValue(double.MinValue), MinZ.GetValue(double.MinValue)),
                        new Tuple<double, double, double>(MaxX.GetValue(double.MaxValue), MaxY.GetValue(double.MaxValue), MaxZ.GetValue(double.MaxValue)))
                );
            }
        }

        protected abstract class DefaultConfigurableAttribute : ConfigurableAttribute
        {
            public object Default { get; set; }
        }

        protected class ConfigurableBoolAttribute : DefaultConfigurableAttribute
        {
            public override Control Create(PropertyInfo property)
            {
                return CreateHost(
                    new BooleanProperty(
                        property.Name,
                        Default.GetValue(false))
                );
            }
        }

        protected class ConfigurableStringAttribute : DefaultConfigurableAttribute
        {
            public object MaxLength { get; set; }
            public bool Multiline { get; set; }
            public override Control Create(PropertyInfo property)
            {
                return CreateHost(
                    new StringProperty(
                    property.Name,
                    Default.GetValue(""),
                    MaxLength.GetValue(int.MaxValue))
                );
            }
        }

        protected class ConfigurableSelectionAttribute : DefaultConfigurableAttribute
        {
            public string DelimitedValues { get; set; }
            public override Control Create(PropertyInfo property)
            {
                return CreateHost(
                    new StaticListChoiceProperty(
                        property.Name,
                        (DelimitedValues ?? "").ToString()
                        .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                        .Cast<object>().ToArray(),
                        Default.GetValue(0))
                );
            }
        }

        protected abstract class NumericConfigurableAttribute : DefaultConfigurableAttribute
        {
            public object Min { get; set; }
            public object Max { get; set; }
            public object SmallStep { get; set; }
            public object LargeStep { get; set; }
            public object MicroStep { get; set; }
        }

        protected class ConfigurableIntAttribute : NumericConfigurableAttribute
        {
            public override Control Create(PropertyInfo property)
            {
                return CreateHost(
                    new Int32Property(
                        property.Name,
                        Default.GetValue(0),
                        Min.GetValue(0),
                        Max.GetValue(255))
                );
            }
        }

        protected class ConfigurableDoubleAttribute : NumericConfigurableAttribute
        {
            public object Precision { get; set; }
            public override Control Create(PropertyInfo property)
            {
                return CreateHost(
                    new DoubleProperty(
                        property.Name,
                        Default.GetValue(0.5d),
                        Min.GetValue(0d),
                        Max.GetValue(1d))
                );
            }
        }
    }
}
