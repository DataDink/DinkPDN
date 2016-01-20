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

        protected BaseConfigurableEffect(string name, Image image, string menu) : base(name, image, menu, EffectFlags.Configurable) {
            Properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new { prop = p, attr = p.GetCustomAttribute<ConfigurableAttribute>(true) })
                .Where(p => p.attr != null && p.prop.CanWrite)
                .ToDictionary(p => p.prop, p => p.attr);
        }

        protected override void OnSetRenderInfo(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            foreach (var kvp in Properties) {
                var prop = kvp.Key;
                var oldValue = prop.GetValue(this);
                var newValue = newToken.GetProperty(prop.Name).Value;
                prop.SetValue(this, newValue);
                if (oldValue != newValue) {
                    prop.SetValue(this, newValue);
                    OnPropertyChanged(prop, oldValue, newValue);
                }
            }
            base.OnSetRenderInfo(parameters, dstArgs, srcArgs);
        }

        protected virtual void OnPropertyChanged(PropertyInfo property, object oldValue, object newValue)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(property.Name));
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

        private readonly object ReadyLock = new object();
        private bool _ready = false;
        protected virtual void OnReady() { }

        protected override void OnRender(Rectangle[] renderRects, int startIndex, int length)
        {
            lock (ReadyLock) {
                if (_ready) return;
                _ready = true;
                OnReady();
            }
        }

        protected abstract class ConfigurableAttribute : Attribute
        {
            protected static readonly Control UISystem = new Control();
            public string Name { get; set; }
            public string Description { get; set; }
            public abstract PropertyControl Create(PropertyInfo property);
            protected virtual PropertyControlInfo Configure(Property property)
            {
                var info = PropertyControlInfo.CreateFor(property);
                info.SetPropertyControlValue(property.Name, ControlInfoPropertyNames.DisplayName, Name);
                info.SetPropertyControlValue(property.Name, ControlInfoPropertyNames.Description, Description);
                return info;
            }
        }

        protected class ConfigurableImageAttribute : ConfigurableAttribute
        {
            public override PropertyControl Create(PropertyInfo property) {
                var info = Configure(new ImageProperty(property.Name));
                return (PropertyControl)info.CreateConcreteControl(UISystem);
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

            public override PropertyControl Create(PropertyInfo property)
            {
                var info = Configure(new DoubleVectorProperty(
                    property.Name,
                    new Pair<double, double>(X, Y),
                    new Pair<double, double>(MinX.GetValue(double.MinValue), MinY.GetValue(double.MinValue)),
                    new Pair<double, double>(MaxX.GetValue(double.MaxValue), MaxY.GetValue(double.MaxValue))));
                return (PropertyControl)info.CreateConcreteControl(UISystem);
            }
        }

        protected class ConfigurableVector3Attribute : ConfigurableVectorAttribute
        {
            public double Z { get; set; }
            public object MaxZ { get; set; }
            public object MinZ { get; set; }

            public override PropertyControl Create(PropertyInfo property)
            {
                var info = Configure(new DoubleVector3Property(
                    property.Name,
                    new Tuple<double, double, double>(X, Y, Z),
                    new Tuple<double, double, double>(MinX.GetValue(double.MinValue), MinY.GetValue(double.MinValue), MinZ.GetValue(double.MinValue)),
                    new Tuple<double, double, double>(MaxX.GetValue(double.MaxValue), MaxY.GetValue(double.MaxValue), MaxZ.GetValue(double.MaxValue))));
                return (PropertyControl)info.CreateConcreteControl(UISystem);
            }
        }

        protected abstract class DefaultConfigurableAttribute : ConfigurableAttribute
        {
            public object Default { get; set; }
        }

        protected class ConfigurableBoolAttribute : DefaultConfigurableAttribute
        {
            public override PropertyControl Create(PropertyInfo property)
            {
                var info = Configure(new BooleanProperty(
                    property.Name,
                    Default.GetValue(false)));
                return (PropertyControl)info.CreateConcreteControl(UISystem);
            }
        }

        protected class ConfigurableStringAttribute : DefaultConfigurableAttribute
        {
            public IComparable MaxLength { get; set; }
            public bool Multiline { get; set; }
            public override PropertyControl Create(PropertyInfo property)
            {
                var info = Configure(new StringProperty(
                    property.Name,
                    Default.GetValue(""),
                    MaxLength.GetValue(int.MaxValue)));
                info.SetPropertyControlValue(property.Name, ControlInfoPropertyNames.Multiline, Multiline);
                return (PropertyControl)info.CreateConcreteControl(UISystem);
            }
        }

        protected class ConfigurableSelectionAttribute : DefaultConfigurableAttribute
        {
            public string DelimitedValues { get; set; }
            public override PropertyControl Create(PropertyInfo property)
            {
                var info = Configure(new StaticListChoiceProperty(
                    property.Name,
                    (DelimitedValues ?? "").ToString()
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Cast<object>().ToArray(),
                    Default.GetValue(0)));
                return (PropertyControl)info.CreateConcreteControl(UISystem);
            }
        }

        protected abstract class NumericConfigurableAttribute : DefaultConfigurableAttribute
        {
            public IComparable Min { get; set; }
            public IComparable Max { get; set; }
            public IComparable SmallStep { get; set; }
            public IComparable LargeStep { get; set; }
            public IComparable MicroStep { get; set; }
        }

        protected class ConfigurableIntAttribute : NumericConfigurableAttribute
        {
            public override PropertyControl Create(PropertyInfo property)
            {
                var info = Configure(new Int32Property(
                    property.Name,
                    Default.GetValue(0),
                    Min.GetValue(0),
                    Max.GetValue(255)));
                info.SetPropertyControlValue(property.Name, ControlInfoPropertyNames.UpDownIncrement, MicroStep.GetValue(1));
                info.SetPropertyControlValue(property.Name, ControlInfoPropertyNames.SliderSmallChange, SmallStep.GetValue(1));
                info.SetPropertyControlValue(property.Name, ControlInfoPropertyNames.SliderLargeChange, LargeStep.GetValue(10));
                return (PropertyControl)info.CreateConcreteControl(UISystem);
            }
        }

        protected class ConfigurableDoubleAttribute : NumericConfigurableAttribute
        {
            public IComparable Precision { get; set; }
            public override PropertyControl Create(PropertyInfo property)
            {
                var info = Configure(new DoubleProperty(
                    property.Name,
                    Default.GetValue(0.5d),
                    Min.GetValue(0d),
                    Max.GetValue(1d)));
                var basevalue = Default.GetValue(1d);
                info.SetPropertyControlValue(property.Name, ControlInfoPropertyNames.UpDownIncrement, MicroStep.GetValue(basevalue / 1000d));
                info.SetPropertyControlValue(property.Name, ControlInfoPropertyNames.SliderSmallChange, SmallStep.GetValue(basevalue / 100d));
                info.SetPropertyControlValue(property.Name, ControlInfoPropertyNames.SliderLargeChange, LargeStep.GetValue(basevalue / 10d));
                info.SetPropertyControlValue(property.Name, ControlInfoPropertyNames.DecimalPlaces, Precision.GetValue(3));
                return (PropertyControl)info.CreateConcreteControl(UISystem);
            }
        }

        private class Dialog : EffectConfigDialog { }
    }
}
