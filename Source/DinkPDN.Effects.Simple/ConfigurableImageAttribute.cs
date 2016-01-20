using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DinkPDN.Effects.Simple
{

    public class ConfigurableImageAttribute : ConfigurableAttribute
    {
        public override PropertyControl Create(PropertyInfo property)
        {
            var info = Configure(new ImageProperty(property.Name));
            return (PropertyControl)info.CreateConcreteControl(UISystem);
        }
    }
}
