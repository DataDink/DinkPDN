using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DinkPDN.Effects.Simple
{
    public class ConfigurableBoolAttribute : DefaultConfigurableAttribute
    {
        public override PropertyControl Create(PropertyInfo property)
        {
            var info = Configure(new BooleanProperty(
                property.Name,
                Default.GetValue(false)));
            return (PropertyControl)info.CreateConcreteControl(UISystem);
        }
    }
}
