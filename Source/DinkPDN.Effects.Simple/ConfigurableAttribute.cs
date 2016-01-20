using PaintDotNet;
using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DinkPDN.Effects.Simple
{

    public abstract class ConfigurableAttribute : Attribute
    {
        protected static readonly Control UISystem = new Control();
        public string Name { get; set; }
        public string Description { get; set; }
        public abstract PropertyControl Create(PropertyInfo property);
        protected virtual PropertyControlInfo Configure(Property property)
        {
            var info = PropertyControlInfo.CreateFor(property);
            if (Name != null) info.SetPropertyControlValue(property.Name, ControlInfoPropertyNames.DisplayName, Name);
            if (Description != null) info.SetPropertyControlValue(property.Name, ControlInfoPropertyNames.Description, Description);
            return info;
        }
    }

    public abstract class DefaultConfigurableAttribute : ConfigurableAttribute
    {
        public object Default { get; set; }
    }

    public abstract class NumericConfigurableAttribute : DefaultConfigurableAttribute
    {
        public object Min { get; set; }
        public object Max { get; set; }
        public object SmallStep { get; set; }
        public object LargeStep { get; set; }
        public object MicroStep { get; set; }
    }
}
