using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DinkPDN.Effects.UI.Views
{
    public class FilePropertyControl : PropertyControl
    {
        private Panel _container = new Panel { Padding = new Padding(5, 0, 5, 0), Height = 30 };
        private Button _button = new Button { Text = "Browse", Dock = DockStyle.Left };
        private Label _label = new Label { Dock = DockStyle.Fill, AutoSize = false, TextAlign = ContentAlignment.MiddleLeft };

        public FilePropertyControl(PaintDotNet.IndirectUI.PropertyControlInfo property) : base(property)
        {
            _label.Text = (string)(property.Property.Value ?? property.Property.DefaultValue ?? "");
            _container.Controls.Add(_label);
            _container.Controls.Add(_button);
            Controls.Add(_container);
            _button.Click += (s, e) => OpenFile();
        }

        protected override void OnPropertyReadOnlyChanged()
        {

        }

        protected override void OnPropertyValueChanged()
        {

        }

        private void OpenFile()
        {
            using (var dlg = new OpenFileDialog {
                Title = "Choose A File"
            }) {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                Property.Value = dlg.FileName;
                _label.Text = dlg.FileName;
            }
        }
    }
}
