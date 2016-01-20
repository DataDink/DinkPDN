using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DinkPDN.Effects.Simple
{
    public class ConfigurableDoubleAttribute : NumericConfigurableAttribute
    {
        public object Precision { get; set; }
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
}
