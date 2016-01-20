using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DinkPDN.Effects.Simple
{
    public class ConfigurableStringAttribute : DefaultConfigurableAttribute
    {
        public object MaxLength { get; set; }
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

}
