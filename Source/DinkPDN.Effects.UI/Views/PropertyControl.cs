using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DinkPDN.Effects.UI.Views
{
    public abstract class PropertyControl : PaintDotNet.IndirectUI.PropertyControl
    {
        private static readonly Assembly HeaderSourceAssembly = Assembly.Load("PaintDotNet.Framework");
        private static readonly Type HeaderSourceType = HeaderSourceAssembly.GetTypes().FirstOrDefault(t => t.FullName == "PaintDotNet.Controls.HeadingLabel");
        private readonly Control _header = HeaderSourceType != null ? (Control)Activator.CreateInstance(HeaderSourceType) : new Label();
        private readonly Control _footer = new Label();

        protected override bool OnFirstSelect() { return false; }

        protected PropertyControl(PaintDotNet.IndirectUI.PropertyControlInfo property) : base(property)
        {
            AutoSize = true;

            Controls.Add(_header);
            _header.Dock = DockStyle.Top;

            Controls.Add(_footer);
            _footer.Dock = DockStyle.Bottom;
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            e.Control.BringToFront();
            e.Control.Dock = DockStyle.Fill;
        }

        protected override void OnDisplayNameChanged()
        {
            base.OnDisplayNameChanged();
            _header.Text = DisplayName;
        }

        protected override void OnDescriptionChanged()
        {
            base.OnDescriptionChanged();
            _footer.Text = Description;
        }
    }
}
