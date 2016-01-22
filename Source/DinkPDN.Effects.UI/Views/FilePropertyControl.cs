using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DinkPDN.Effects.UI.Views
{
    public class FilePropertyControl : PropertyControl
    {
        public FilePropertyControl(PaintDotNet.IndirectUI.PropertyControlInfo property) : base(property)
        {
            Controls.Add(new Label { Text = "Test" });
            BackColor = Color.Black;
        }

        protected override void OnPropertyReadOnlyChanged()
        {

        }

        protected override void OnPropertyValueChanged()
        {

        }
    }
}
