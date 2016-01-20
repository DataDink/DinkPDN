using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DinkPDN.Effects.Simple
{
    public class ConfigurableSelectionAttribute : DefaultConfigurableAttribute
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

}
