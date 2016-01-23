using System;
using System.Collections.Generic;
using System.Drawing;
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
        private readonly Control _header = HeaderSourceType != null ? (Control)Activator.CreateInstance(HeaderSourceType) : new Label { AutoSize = false };
        private readonly Control _footer = new Label { AutoSize = false };

        protected override bool OnFirstSelect() { return false; }

        protected PropertyControl(PaintDotNet.IndirectUI.PropertyControlInfo property) : base(property)
        {
            AutoSize = true;

            Controls.Add(_header);
            _header.Dock = DockStyle.Top;
            _header.Text = DisplayName;

            Controls.Add(_footer);
            _footer.Dock = DockStyle.Bottom;
            _footer.Text = Description;
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            e.Control.BringToFront();
            e.Control.Dock = DockStyle.Top;
        }

        protected override void OnDisplayNameChanged()
        {
            base.OnDisplayNameChanged();
            if (_header != null) _header.Text = DisplayName;
        }

        protected override void OnDescriptionChanged()
        {
            base.OnDescriptionChanged();
            if (_footer != null) _footer.Text = Description;
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            _header.SetBounds(0, 0, this.ClientSize.Width, _header.GetPreferredSize(new Size(this.ClientSize.Width, 0)).Height);
            base.OnLayout(e);
        }
    }
}
