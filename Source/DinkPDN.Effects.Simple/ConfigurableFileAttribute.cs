using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;
using DinkPDN.Effects.Simple.Views;

namespace DinkPDN.Effects.Simple
{
    public class ConfigurableFileAttribute : ConfigurableAttribute
    {
        public override PropertyControl Create(PropertyInfo property)
        {
            var info = Configure(new StringProperty(property.Name));
            return new FilePropertyControl(info);
        }
    }
}
