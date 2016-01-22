using PaintDotNet.IndirectUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace DinkPDN.Effects.Simple.Views
{
    public class FilePropertyControl : PropertyControl
    {
        protected override bool OnFirstSelect() { return false; }

        protected override void OnPropertyReadOnlyChanged()
        {

        }

        protected override void OnPropertyValueChanged()
        {

        }

        public FilePropertyControl(PropertyControlInfo property) : base(property)
        {

        }
    }
}
