using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using PaintDotNet.IndirectUI;

namespace DinkPDN.Effects.Simple
{
    public class ConfigurableFileAttribute : ConfigurableAttribute
    {
        public override PropertyControl Create(PropertyInfo property)
        {
            throw new NotImplementedException();
        }
    }
}
