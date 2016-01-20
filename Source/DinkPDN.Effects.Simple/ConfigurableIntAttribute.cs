using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DinkPDN.Effects.Simple
{

    public class ConfigurableIntAttribute : NumericConfigurableAttribute
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
}
