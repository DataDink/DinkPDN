using DinkPDN.Common;
using PaintDotNet;
using PaintDotNet.Effects;
using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

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

    public abstract class BaseConfigurableEffect : PropertyBasedEffect
    {
        private readonly Type Type;
        private readonly Dictionary<PropertyInfo, ConfigurableAttribute> Properties;

        protected BaseConfigurableEffect(string name, Image image, string menu) : base(name, image, menu, EffectFlags.Configurable) {
            Properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new { prop = p, attr = p.GetCustomAttribute<ConfigurableAttribute>(true) })
                .Where(p => p.attr != null && p.prop.CanWrite)
                .ToDictionary(p => p.prop, p => p.attr);
        }

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            return new PropertyCollection(Properties.Select(kvp => kvp.Value.Create(kvp.Key)));
        }

        protected override void OnSetRenderInfo(PropertyBasedEffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            foreach (var kvp in Properties) {
                var prop = kvp.Key;
                var value = newToken.GetProperty(prop.Name).Value;
                prop.SetValue(this, value);
            }
            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        protected override ControlInfo OnCreateConfigUI(PropertyCollection props)
        {
            var control = CreateDefaultConfigUI(props);
            foreach (var kvp in Properties) {
                var prop = kvp.Key;
                var attr = kvp.Value;
                control.SetPropertyControlValue(prop.Name, ControlInfoPropertyNames.DisplayName, attr.Name ?? prop.Name);
                if (attr.Description != null) control.SetPropertyControlValue(prop.Name, ControlInfoPropertyNames.Description, attr.Description);

                var strprop = attr as ConfigurableStringAttribute;
                if (strprop != null) control.SetPropertyControlValue(prop.Name, ControlInfoPropertyNames.Multiline, strprop.Multiline);

                var intprop = attr as ConfigurableIntAttribute;
                if (intprop != null) {
                    control.SetPropertyControlValue(prop.Name, ControlInfoPropertyNames.UpDownIncrement, intprop.MicroStep.GetValue(1));
                    control.SetPropertyControlValue(prop.Name, ControlInfoPropertyNames.SliderSmallChange, intprop.SmallStep.GetValue(1));
                    control.SetPropertyControlValue(prop.Name, ControlInfoPropertyNames.SliderLargeChange, intprop.LargeStep.GetValue(10));
                }

                var dblprop = attr as ConfigurableDoubleAttribute;
                if (dblprop != null) {
                    var basevalue = dblprop.Default.GetValue(1d);
                    control.SetPropertyControlValue(prop.Name, ControlInfoPropertyNames.UpDownIncrement, dblprop.MicroStep.GetValue(basevalue / 1000d));
                    control.SetPropertyControlValue(prop.Name, ControlInfoPropertyNames.SliderSmallChange, dblprop.SmallStep.GetValue(basevalue / 100d));
                    control.SetPropertyControlValue(prop.Name, ControlInfoPropertyNames.SliderLargeChange, dblprop.LargeStep.GetValue(basevalue / 10d));
                    control.SetPropertyControlValue(prop.Name, ControlInfoPropertyNames.DecimalPlaces, dblprop.Precision.GetValue(3));
                }
            }

            return control;
        }

        protected abstract class ConfigurableAttribute : Attribute
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public abstract Property Create(PropertyInfo property);
        }

        protected class ConfigurableImageAttribute : ConfigurableAttribute
        {
            public override Property Create(PropertyInfo property) { return new ImageProperty(property.Name); }
        }

        protected class ConfigurableVectorAttribute : ConfigurableAttribute
        {
            public double X { get; set; }
            public double Y { get; set; }
            public object MaxX { get; set; }
            public object MaxY { get; set; }
            public object MinX { get; set; }
            public object MinY { get; set; }

            public override Property Create(PropertyInfo property)
            {
                return new DoubleVectorProperty(
                    property.Name,
                    new Pair<double, double>(X, Y),
                    new Pair<double, double>(MinX.GetValue(double.MinValue), MinY.GetValue(double.MinValue)),
                    new Pair<double, double>(MaxX.GetValue(double.MaxValue), MaxY.GetValue(double.MaxValue)));
            }
        }

        protected class ConfigurableVector3Attribute : ConfigurableVectorAttribute
        {
            public double Z { get; set; }
            public object MaxZ { get; set; }
            public object MinZ { get; set; }

            public override Property Create(PropertyInfo property)
            {
                return new DoubleVector3Property(
                    property.Name,
                    new Tuple<double, double, double>(X, Y, Z),
                    new Tuple<double, double, double>(MinX.GetValue(double.MinValue), MinY.GetValue(double.MinValue), MinZ.GetValue(double.MinValue)),
                    new Tuple<double, double, double>(MaxX.GetValue(double.MaxValue), MaxY.GetValue(double.MaxValue), MaxZ.GetValue(double.MaxValue)));
            }
        }

        protected abstract class DefaultConfigurableAttribute : ConfigurableAttribute
        {
            public object Default { get; set; }
        }

        protected class ConfigurableBoolAttribute : DefaultConfigurableAttribute
        {
            public override Property Create(PropertyInfo property)
            {
                return new BooleanProperty(
                    property.Name,
                    Default.GetValue(false));
            }
        }

        protected class ConfigurableStringAttribute : DefaultConfigurableAttribute
        {
            public object MaxLength { get; set; }
            public bool Multiline { get; set; }
            public override Property Create(PropertyInfo property)
            {
                return new StringProperty(
                    property.Name,
                    Default.GetValue(""),
                    MaxLength.GetValue(int.MaxValue));
            }
        }

        protected class ConfigurableSelectionAttribute : DefaultConfigurableAttribute
        {
            public string DelimitedValues { get; set; }
            public override Property Create(PropertyInfo property)
            {
                return new StaticListChoiceProperty(
                    property.Name,
                    (DelimitedValues ?? "").ToString()
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Cast<object>().ToArray(),
                    Default.GetValue(0));
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
            public override Property Create(PropertyInfo property)
            {
                return new Int32Property(
                    property.Name,
                    Default.GetValue(0),
                    Min.GetValue(0),
                    Max.GetValue(255));
            }
        }

        protected class ConfigurableDoubleAttribute : NumericConfigurableAttribute
        {
            public object Precision { get; set; }
            public override Property Create(PropertyInfo property)
            {
                return new DoubleProperty(
                    property.Name,
                    Default.GetValue(0.5d),
                    Min.GetValue(0d),
                    Max.GetValue(1d));
            }
        }
    }
}
