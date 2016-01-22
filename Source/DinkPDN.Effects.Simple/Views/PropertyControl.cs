using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DinkPDN.Effects.Simple.Views
{
    public abstract class PropertyControl : PaintDotNet.IndirectUI.PropertyControl
    {
        private static readonly Assembly HeaderSourceAssembly = Assembly.GetAssembly("PaintDot")
        private readonly 
        protected override bool OnFirstSelect() { return false; }

        protected PropertyControl(PaintDotNet.IndirectUI.PropertyControlInfo property) : base(property)
        {

        }
    }
}
